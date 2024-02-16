#version 330

//Zmienne jednorodne
uniform mat4 P;
uniform mat4 V;
uniform mat4 M;
uniform sampler2D textureMap0; //globalnie
uniform sampler2D textureMap1; //globalnie

out vec4 pixelColor; //Zmienna wyjsciowa fragment shadera. Zapisuje sie do niej ostateczny (prawie) kolor piksela

in vec2 i_TexCoord0; //globalnie
in vec4 i_v;
in vec4 i_n;

vec4 lp=vec4(0,0,-6,1); //wsp. światła w przestrzeni świata

void main(void) {
	vec4 l=normalize(V*lp-V*M*i_v);
	vec4 n=normalize(V*M*i_n);

	float nl=dot(n,l);
	nl=clamp(nl,0,1);

	vec4 r=reflect(-l,n); //normalizeniepotrzebne bo |l|=|n|=1
	vec4 v=normalize(vec4(0,0,0,1)-V*M*i_v);

	float rv=clamp(dot(r,v),0,1);
	rv=pow(rv,25);
	
	vec4 texColor=texture(textureMap0,i_TexCoord0);//main
	vec4 texColor2=texture(textureMap1,(n.xy+1)/2);//main
	
	vec4 La = vec4(0,0,0,1);
	vec4 Ld = vec4(1,1,1,1);
	vec4 Ls = vec4(1,1,1,1);
	
	vec4 ka = texColor;
	vec4 kd = mix(texColor,texColor2,0.4);
	vec4 ks = vec4(1,1,1,1);
	
    pixelColor = La*ka + Ld*kd*nl + Ls*ks*rv;

}
