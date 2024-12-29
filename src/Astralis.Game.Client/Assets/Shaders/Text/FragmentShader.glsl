#version 330 core

// Uniforms
uniform sampler2D TextureSampler;

// Inputs
in vec4 v_color;
in vec2 v_texCoords;

// Output
out vec4 FragColor;

void main()
{
    FragColor = v_color * texture(TextureSampler, v_texCoords);
}
