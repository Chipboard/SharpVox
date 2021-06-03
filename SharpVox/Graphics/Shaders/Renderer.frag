uniform vec2 resolution;
uniform vec3 camPos;
uniform vec3 camForward;
uniform vec3 camRight;
uniform vec3 camUp;

struct ray {
    vec3 pos;
    vec3 dir;
};

vec3 projection (vec2 p) {
  return normalize(vec3(p.x, p.y, 0));
}

float sdSphere(vec3 p, vec3 center, float radius)
{
  return length(center - p) - radius;
}

ray createCamRay(vec2 uv, vec3 lookAt, float zoom){
    vec3 c=camPos+camForward*zoom;
    vec3 i=c+uv.x*camRight+uv.y*camUp;
    vec3 dir=i-camPos;
    return ray(camPos,dir);
}

float distToScene(vec3 p){
    return sdSphere(p, vec3(0,0,0), 1);
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

void main()
{
    float widthDifference = resolution.x - resolution.y;

	vec2 uv = (gl_FragCoord.xy - 0.5 * resolution) / resolution.y;

	vec4 color = vec4(0.5,0.6,0.8,1);

	ray camRay = createCamRay(uv, vec3(0,0,0), 1);
	for(int i = 0; i < 100; i++)
	{
		float dist = distToScene(camRay.pos);
		
		if(dist < 0.001){
			color = vec4(estimateNormal(camRay.pos),1);
			break;
		}

		camRay.pos += camRay.dir * dist;
	}

	gl_FragColor = color;
}