uniform sampler2D renderTexture0;
uniform sampler2D noiseTexture;
uniform vec3 camForward;
uniform vec3 camPos;
uniform vec2 resolution;

void main() 
{ 
  vec2 uv = gl_FragCoord.xy / resolution;

  vec4 noise = texture(noiseTexture, uv + ((camForward.xy + camPos.xy + camForward.yx + camPos.yz) * 100));

  gl_FragColor = texture(renderTexture0, uv) * (1 - (noise * 0.1));
}