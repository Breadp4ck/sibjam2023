shader_type spatial;

uniform float rotation_speed : hint_range(0, 10) = 1.0;

void vertex() {
	
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(vec4(normalize(cross(vec3(0.0, 1.0, 0.0), INV_VIEW_MATRIX[2].xyz)), 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(normalize(cross(INV_VIEW_MATRIX[0].xyz, vec3(0.0, 1.0, 0.0))), 0.0), MODEL_MATRIX[3]);

    MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
}