#version 330 core
// Inputs
in vec3 a_position;
in vec4 a_color;
in vec2 a_texCoords0;

// Uniforms
uniform mat4 MatrixTransform;

// Outputs
out vec4 v_color;
out vec2 v_texCoords;

void main()
{
    v_color = a_color;
    v_texCoords = a_texCoords0;
    gl_Position = MatrixTransform * vec4(a_position, 1.0);
}
