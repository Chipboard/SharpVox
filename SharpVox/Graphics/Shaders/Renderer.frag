uniform vec2 resolution;
uniform vec3 camPos;
uniform vec3 camForward;
uniform vec3 camRight;
uniform vec3 camUp;

uniform sampler2D noiseTexture;
uniform sampler2D skyTexture;
uniform int frame;

const float c_goldenRatioConjugate = 0.61803398875;
const float PI = 3.14159265359;
const float maxDist = 500;
const int maxBounces = 3;
const float epsilon = 0.001;

struct Ray {
    vec3 pos;
    vec3 dir;
    vec4 color;
    int bounces;
};

struct RayHit {
    vec3 dist;
    vec3 position;
    vec3 normal;
    vec3 albedo;
    vec3 specular;
};

struct Sphere {
    vec3 pos;
    vec4 color;
    float radius;
    bool used;
};

struct ShapeData {
    Sphere[10000] spheres;
};
uniform ShapeData shapeHolder;

vec3 Projection (vec2 p) {
  return normalize(vec3(p.x, p.y, 0));
}

float SdSphere(vec3 p, vec3 center, float radius, vec3 dir)
{
    if(dot(normalize(center - p), dir) > radius)
        return maxDist;

    return length(center - p) - radius;
}

float OptSphere(vec3 pos, vec3 dir, float radius)
{
    vec3 d = pos - vec3(0,0,0);
    float p1 = -dot(dir, d);
    float p2sqr = p1 * p1 - dot(d, d) + radius * radius;
    if (p2sqr < 0)
        return maxDist;

        return p2sqr;
}

float FracSphere(vec3 p, float r)
{
    //normalize(pos - center) - radius
    p = (mod(abs(p), 1.0)) - vec3(0.5, 0.5, 0.5);
    return length(p) - r;
}

#define SCALE 2.8
#define MINRAD2 .25
float minRad2 = clamp(MINRAD2, 1.0e-9, 1.0);
#define scale (vec4(SCALE, SCALE, SCALE, abs(SCALE)) / minRad2)
float absScalem1 = abs(SCALE - 1.0);
float AbsScaleRaisedTo1mIters = pow(abs(SCALE), float(1-10));
float Map(vec3 pos) 
{
	
	vec4 p = vec4(pos,1);
	vec4 p0 = p;  // p.w is the distance estimate

	for (int i = 0; i < 9; i++)
	{
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;

		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(minRad2/r2, minRad2), 0.0, 1.0);

		// scale, translate
		p = p*scale + p0;
	}
	return ((length(p.xyz) - absScalem1) / p.w - AbsScaleRaisedTo1mIters);
}

float Map2( vec3 p, float s )
{
	float cscale = 1;
	
	for( int i=0; i<8;i++ )
	{
		p = -1.0 + 2.0*fract(0.5*p+0.5);

		float r2 = dot(p,p);
		
		float k = s/r2;
		p *= k;
		cscale *= k;
	}
	
	return 0.25*abs(p.y)/cscale;
}


Ray CreateCamRay(vec2 uv, vec3 lookAt, float zoom)
{
    vec3 c=camPos+camForward*zoom;
    vec3 i=c+uv.x*camRight+uv.y*camUp;
    vec3 dir=normalize(i-camPos);
    return Ray(camPos,dir,vec4(0,0,0,1), 0);
}

float DistToScene(vec3 pos, vec3 dir)
{
    float dist = maxDist;

    /*for(int x = 0; x < 2; x++)
    {
        for(int y = 0; y < 2; y++)
        {
            for(int z = 0; z < 2; z++)
            {
                dist = min(dist, SdSphere(pos, vec3(x*4,y*4,z*4), 1, dir));
            }
        }
    }*/

    //dist = mod(Map(sin(pos)), 1) - 0.01;

    //dist = min(dist, SdSphere(pos, vec3(0,0,0), 1, dir));
    //dist = min(dist, FracSphere(pos, 0.25));
    //dist = min(dist, OptSphere(pos, dir, 1));
    //dist = Map2(pos, 1.525);
    dist = Map(pos);

    return dist;
}

vec3 EstimateNormal(Ray camRay){
    float xPl=DistToScene(vec3(camRay.pos.x+epsilon,camRay.pos.y,camRay.pos.z), camRay.dir);
    float xMi=DistToScene(vec3(camRay.pos.x-epsilon,camRay.pos.y,camRay.pos.z), camRay.dir);
    float yPl=DistToScene(vec3(camRay.pos.x,camRay.pos.y+epsilon,camRay.pos.z), camRay.dir);
    float yMi=DistToScene(vec3(camRay.pos.x,camRay.pos.y-epsilon,camRay.pos.z), camRay.dir);
    float zPl=DistToScene(vec3(camRay.pos.x,camRay.pos.y,camRay.pos.z+epsilon), camRay.dir);
    float zMi=DistToScene(vec3(camRay.pos.x,camRay.pos.y,camRay.pos.z-epsilon), camRay.dir);
    float xDiff=xPl-xMi;
    float yDiff=yPl-yMi;
    float zDiff=zPl-zMi;
    return normalize(vec3(xDiff,yDiff,zDiff));
}

vec3 EstimateNormalCheap(Ray camRay)
{
    float d0 = DistToScene(camRay.pos, camRay.dir);
    const vec2 epsilon = vec2(epsilon,0);
    vec3 d1 = vec3(
        DistToScene(camRay.pos-epsilon.xyy, camRay.dir),
        DistToScene(camRay.pos-epsilon.yxy, camRay.dir),
        DistToScene(camRay.pos-epsilon.yyx, camRay.dir));
    return normalize(d0 - d1);
}

vec4 GetSky(Ray camRay)
{
    //Sample Skybox
    float theta = acos(camRay.dir.y) / -PI;
    float phi = atan(camRay.dir.x, -camRay.dir.z) / -PI * 0.5;
    return texture(skyTexture, vec2(phi, -theta), 0);
}

void main()
{
    float widthDifference = resolution.x - resolution.y;
	vec2 uv = (gl_FragCoord.xy - 0.5 * resolution) / resolution.y;

    // blue noise
    float startRayOffset = texture(noiseTexture, uv * frame / 1024.0f).r;
    startRayOffset = fract(startRayOffset + frame * c_goldenRatioConjugate)*0.0314;

	Ray camRay = CreateCamRay(uv, vec3(0,0,0), 1);
    vec3 originalPos = camRay.pos;
    camRay.pos += camRay.dir * (startRayOffset + epsilon);

    bool hit = false;
    float dist = maxDist;
    float minDist = maxDist;
	for(int i = 0; i < 250; i++)
	{		
        dist = DistToScene(camRay.pos, camRay.dir);
		
        minDist = min(minDist, dist);

        //camRay.dir += sin(cos(originalPos-sin(originalPos)))*0.005;

        if(dist >= maxDist)
            break;

        float optimizedDist = epsilon + (i*0.00025);

		if(dist < optimizedDist)
        {
            if(camRay.color.w > 0.001 && camRay.bounces < maxBounces)
            {
                camRay.dir = reflect(camRay.dir, EstimateNormalCheap(camRay));
			    camRay.color = mix(camRay.color, vec4((sin(camRay.pos) + vec3(0.1,0.1,0.1)),1), camRay.color.w);
                camRay.color.w *= 0.25;
                camRay.bounces++;

                camRay.pos += camRay.dir * (dist + epsilon);
            } else {
                hit = true;
                break;
            }
		}

		camRay.pos += camRay.dir * max(dist, optimizedDist);
	}

    //distance based fog
   /* float distFactor = max(0, 0.9 - (minDist * 0.1));
    camRay.color = mix(camRay.color, vec4(vec3(0.5,0.6,0.7), 1), distFactor);
    camRay.color.w -= distFactor;*/

    if(!hit)
    {
        //sky
        camRay.color = mix(camRay.color, GetSky(camRay), camRay.color.w);
    }
	
	//AO
	camRay.color *= clamp(0.1,1, 0.9 + (minDist*10));

	gl_FragColor = vec4(camRay.color.xyz, 1);
}