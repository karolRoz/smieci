#version 330

//Zmienne jednorodne
uniform mat4 P;
uniform mat4 V;
uniform mat4 M;

//Atrybuty
in vec4 vertex; //wspolrzedne wierzcholka w przestrzeni modelu
in vec4 normal; //wektor normalny w przestrzeni modelu
in vec4 color; //kolor skojarzony z wierzcho�kiem
in vec2 texCoord; //wsp�rz�dna teksturowana

out vec2 i_TexCoord0; //globalnie
out vec4 i_v;
out vec4 i_n;

void main(void) {
	i_v = vertex;
	i_n = normal;
	i_TexCoord0=texCoord; //w main
    gl_Position=P*V*M*vertex;
}
