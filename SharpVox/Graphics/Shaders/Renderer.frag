uniform vec2 resolution;
uniform vec3 camPos;
uniform vec3 camForward;
uniform vec3 camRight;
uniform vec3 camUp;

uniform sampler2D noiseTexture;
uniform sampler2D skyTexture;
uniform int frame;
uniform float totalDeltaTime;

const float c_goldenRatioConjugate = 0.61803398875;
const float PI = 3.14159265359;

uniform int maxIterations;
uniform int maxBounces;
uniform float epsilon;
uniform float maxDist;

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

vec4 triplanar( sampler2D sam, in vec3 p)
{
    vec4 x = texture( sam, p.yz )*0.33;
    vec4 y = texture( sam, p.zx )*0.33;
    vec4 z = texture( sam, p.xy )*0.33;
    return x+y+z;
}

vec2 rotate(vec2 v, float a) {
	return vec2(cos(a)*v.x + sin(a)*v.y, -sin(a)*v.x + cos(a)*v.y);
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

#define Scale 4.0
#define Offset vec3(0.92858,0.92858,0.32858)
float Map3(vec3 z)
{
	// Folding 'tiling' of 3D space;
	z  = abs(1.0-mod(z,2.0));

	float d = 1000.0;
	for (int n = 0; n < 7; n++) {
		z.xy = rotate(z.xy,4.0+2.0*cos(totalDeltaTime * 0.0025));		
		z = abs(z);
		if (z.x<z.y){ z.xy = z.yx;}
		if (z.x< z.z){ z.xz = z.zx;}
		if (z.y<z.z){ z.yz = z.zy;}
		z = Scale*z-Offset*(Scale-1.0);
		if( z.z<-0.5*Offset.z*(Scale-1.0))  z.z+=Offset.z*(Scale-1.0);
		d = min(d, length(z) * pow(Scale, float(-n)-1.0));
	}
	
	return d-0.001;
}

float Map4(vec3 z0){
	vec4 c = vec4(z0,1.0),z = c;
	float r = length(z.xyz),zr,zo,zi,p=8.0;
	vec4 trap=vec4(z.xyz,r);
	for (int n = 0; n < 9 && r<2.0; n++) {
		zo = asin(z.z / r) * p-totalDeltaTime;
		zi = atan(z.y, z.x) * p;
		zr = pow(r, p-1.0);
		z=zr*vec4(r*vec3(cos(zo)*vec2(cos(zi),sin(zi)),sin(zo)),z.w*p)+c;
		r = length(z.xyz);
		trap=vec4(z.xyz,r);
	}
	float d=min(0.5 * log(r) * r / z.w,z0.y+1.0);
	return d;
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
    //dist = mod(Map2(sin(pos), 1.525), 1) - 0.01;
    //dist = Map(sin(pos * 0.777));
    //dist = Map2(pos, 1.525);
    //dist = Map3(pos);
    dist = Map4(pos);
    //dist = min(Map2(pos*0.9,1.525), Map(mod(pos*0.1, 1)));

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
    vec2 epsilon = vec2(epsilon,0);
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

    //blue noise
    float blueNoise = texture(noiseTexture, uv * frame + 10000 / 1024.0f).r;
    blueNoise = fract(blueNoise + frame * c_goldenRatioConjugate)*0.00314;

	Ray camRay = CreateCamRay(uv, vec3(0,0,0), 1);
    //vec3 originalPos = camRay.pos;
    camRay.pos += camRay.dir * blueNoise;

    bool hit = false;
    float dist = maxDist;
    float minDist = maxDist;
    float totalTravelDist = 0;
	for(int i = 0; i < maxIterations; i++)
	{		
        dist = DistToScene(camRay.pos, camRay.dir);
		
        minDist = min(minDist, dist);

        //camRay.dir += sin(cos(originalPos-sin(originalPos)))*0.005;
        //camRay.dir += normalize(sin(camRay.pos*0.1)-cos(camRay.pos*2.33))*0.005;

        if(totalTravelDist >= maxDist)
            break;

		if(dist < epsilon + (totalTravelDist * 0.0015))
        {
            if(camRay.color.w > 0.001 && camRay.bounces < maxBounces)
            {
                vec3 normal = EstimateNormalCheap(camRay);
                camRay.dir = reflect(camRay.dir, normal * (1 + (blueNoise) * 0.01));
                //camRay.dir = reflect(camRay.dir, normal);
			    camRay.color = mix(camRay.color, vec4((sin(camRay.pos) + vec3(0.1,0.1,0.1)), camRay.color.w), camRay.color.w);
                //camRay.color = mix(camRay.color, vec4(triplanar(skyTexture, normal + camRay.pos).xyz, camRay.color.w), camRay.color.w);
                camRay.color.w *= 0.25;
                camRay.bounces++;

                camRay.pos += camRay.dir * (dist + epsilon);
            } else {
                hit = true;
                break;
            }
		}

        totalTravelDist += dist;
		camRay.pos += camRay.dir * dist;
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

    //Fog
    //camRay.color = mix(camRay.color, vec4(0,0,0,0), clamp(0,1, totalTravelDist * 0.01789));

	gl_FragColor = vec4(camRay.color.xyz, 1);
}