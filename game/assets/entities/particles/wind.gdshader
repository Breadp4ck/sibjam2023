shader_type spatial;

// Uniforms
uniform float wind_strength : hint_range(-1, 1) = 0.5;
uniform float wind_frequency : hint_range(0, 10) = 1.0;
uniform float wind_amplitude : hint_range(0, 1) = 0.2;

void vertex() {
    // Get the original vertex position
    vec3 vertex_position = VERTEX;

    // Calculate the wind effect based on the vertex position
    float wind_factor = sin(TIME * wind_frequency + vertex_position.x) * wind_amplitude;

    // Apply the wind effect to the vertex position
    vertex_position.z += wind_factor * wind_strength;

    // Set the new vertex position
    VERTEX = vertex_position;
}
