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
const int maxBounces = 2;
const float epsilon = 0.001;

struct ray {
    vec3 pos;
    vec3 dir;
    vec4 color;
    int bounces;
};

vec3 projection (vec2 p) {
  return normalize(vec3(p.x, p.y, 0));
}

float sdSphere(vec3 p, vec3 center, float radius)
{
  return length(center - p) - radius;
}

ray createCamRay(vec2 uv, vec3 lookAt, float zoom)
{
    vec3 c=camPos+camForward*zoom;
    vec3 i=c+uv.x*camRight+uv.y*camUp;
    vec3 dir=normalize(i-camPos);
    return ray(camPos,dir,vec4(0,0,0,1), 0);
}

float distToScene(vec3 p)
{
    float dist = 90000;
    for(int x = 0; x < 2; x++)
    {
        for(int y = 0; y < 2; y++)
        {
            for(int z = 0; z < 2; z++)
            {
                dist = min(dist, sdSphere(p, vec3(x*4,y*4,z*4), 1));
            }
        }
    }

    return dist;
}

const float EPS=0.001;
vec3 estimateNormal(vec3 p){
    float xPl=distToScene(vec3(p.x+EPS,p.y,p.z));
    float xMi=distToScene(vec3(p.x-EPS,p.y,p.z));
    float yPl=distToScene(vec3(p.x,p.y+EPS,p.z));
    float yMi=distToScene(vec3(p.x,p.y-EPS,p.z));
    float zPl=distToScene(vec3(p.x,p.y,p.z+EPS));
    float zMi=distToScene(vec3(p.x,p.y,p.z-EPS));
    float xDiff=xPl-xMi;
    float yDiff=yPl-yMi;
    float zDiff=zPl-zMi;
    return normalize(vec3(xDiff,yDiff,zDiff));
}

vec4 GetSky(ray camRay)
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
    startRayOffset = fract(startRayOffset + frame * c_goldenRatioConjugate)*0.314;

    bool hit;
	ray camRay = createCamRay(uv, vec3(0,0,0), 1);
    camRay.pos += camRay.dir * startRayOffset;

	for(int i = 0; i < 100; i++)
	{
		float dist = distToScene(camRay.pos);
		
        if(dist > maxDist)
            break;

		if(dist < 0.001){
            camRay.dir = reflect(camRay.dir, estimateNormal(camRay.pos));
			camRay.color = vec4(1,1,1,1) * camRay.color.w;
            camRay.color.w -= 0.2;

            if(camRay.bounces > maxBounces)
                break;

            camRay.pos += camRay.dir * (dist + epsilon);
		}

		camRay.pos += camRay.dir * dist;
	}

    if (!hit)
    {
        camRay.color += GetSky(camRay) * camRay.color.w;
    }

	gl_FragColor = vec4(camRay.color.xyz, 1);
}