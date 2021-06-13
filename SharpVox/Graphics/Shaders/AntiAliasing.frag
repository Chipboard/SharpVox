uniform sampler2D renderTexture0;
uniform sampler2D renderTexture1;
uniform vec2 resolution;
uniform int frame;

uniform float blendFactor;

void edgeDetect(inout vec4 n[9], sampler2D tex, vec2 coord)
{
	float w = 1.0 / resolution.x;
	float h = 1.0 / resolution.y;

	n[0] = texture2D(tex, coord + vec2( -w, -h));
	n[1] = texture2D(tex, coord + vec2(0.0, -h));
	n[2] = texture2D(tex, coord + vec2(  w, -h));
	n[3] = texture2D(tex, coord + vec2( -w, 0.0));
	n[4] = texture2D(tex, coord);
	n[5] = texture2D(tex, coord + vec2(  w, 0.0));
	n[6] = texture2D(tex, coord + vec2( -w, h));
	n[7] = texture2D(tex, coord + vec2(0.0, h));
	n[8] = texture2D(tex, coord + vec2(  w, h));
}

vec4 sampleJitter(vec2 uv, int it, float spread)
{
	/*vec4[9] s;
	s[0] = texture(renderTexture0, vec2(uv.x-1,uv.y + 1)); //ULeft
	s[1] = texture(renderTexture0, vec2(uv.x,uv.y + 1)); //UMiddle
	s[2] = texture(renderTexture0, vec2(uv.x+1,uv.y + 1)); //URight
	s[3] = texture(renderTexture0, vec2(uv.x-1,uv.y)); //Left
	s[4] = texture(renderTexture0, vec2(uv.x,uv.y)); //Middle
	s[5] = texture(renderTexture0, vec2(uv.x+1,uv.y)); //Right
	s[6] = texture(renderTexture0, vec2(uv.x-1,uv.y - 1)); //DLeft
	s[7] = texture(renderTexture0, vec2(uv.x,uv.y - 1)); //DMiddle
	s[8] = texture(renderTexture0, vec2(uv.x+1,uv.y - 1)); //DRight*/

	vec4 blend = texture(renderTexture0, uv);
	for(int i = 0; i < it; i++)
	{
		blend = mix(blend, texture(renderTexture0,vec2(uv.x + (sin(i+frame*3.14)*spread), uv.y + (cos(i+frame*3.14)*spread))), 0.1f);
	}

	return blend;
}

void main() 
{ 
  vec2 uv = gl_FragCoord / resolution;

  //Edge detection
  		vec4 n[9];
	edgeDetect( n, renderTexture0, uv);

	vec4 sobel_edge_h = n[2] + (2.0*n[5]) + n[8] - (n[0] + (2.0*n[3]) + n[6]);
  	vec4 sobel_edge_v = n[0] + (2.0*n[1]) + n[2] - (n[6] + (2.0*n[7]) + n[8]);
	vec4 sobel = sqrt((sobel_edge_h * sobel_edge_h) + (sobel_edge_v * sobel_edge_v));

	float intensity = length(sobel);

	//Blending
	vec4 blend = mix(sampleJitter(uv, 10, (0.75-intensity)*0.002), texture(renderTexture0, uv), clamp(0,1,0.75-intensity));

  gl_FragColor = mix(blend, texture(renderTexture1, uv), blendFactor);
}