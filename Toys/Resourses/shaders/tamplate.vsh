#version 330 core 
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexcord;
layout (location = 3) in ivec4 BoneIDs;
layout (location = 4) in vec4 Weights;

layout (std140) uniform skeleton
{
    mat4 gBones[500];
};

layout (std140) uniform space {
	mat4 model;
	mat4 pvm;
	mat4 NormalMat;
	mat4 lightSpacePos;
};

//uniform mat4 pvm;
//uniform mat4 NormalMat;
//uniform mat4 lightSpacePos;

out VS_OUT {
	vec2 Texcord;
	vec3 FragPos;
	vec3 Normal;
	vec4 lightSpace;
} vs_out;

void main()
{
	mat4 BoneTransform = gBones[BoneIDs[0]] * Weights[0];
    BoneTransform += gBones[BoneIDs[1]] * Weights[1];
    BoneTransform += gBones[BoneIDs[2]] * Weights[2];
    BoneTransform += gBones[BoneIDs[3]] * Weights[3];
	
	gl_Position =  pvm * BoneTransform * vec4(aPos, 1.0);
	
	vs_out.Texcord = aTexcord;	
	vs_out.FragPos = vec3(model * BoneTransform * vec4(aPos, 1.0));
	vs_out.Normal = mat3(NormalMat * BoneTransform)  * aNormal;
	vs_out.lightSpace = lightSpacePos * vec4(vs_out.FragPos,1.0);
	//Normal = mat3(NormalMat) * aNormal;
}