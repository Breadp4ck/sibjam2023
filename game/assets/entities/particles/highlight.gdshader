shader_type spatial;

uniform vec3 highlight_color : source_color = vec3(1.0, 0.0, 0.0);


uniform sampler2D SCREEN_TEXTURE:  hint_screen_texture, filter_linear_mipmap;

void vertex() {
	VERTEX += NORMAL * 0.01;
}

void fragment() {
    // Output the final color
	
	float q = abs(dot(NORMAL, vec3(0.0, 0.0, 1.0)));
	
		
		
		EMISSION = highlight_color * 10.0;
		ALBEDO = highlight_color;
		ALPHA = 1.0 - pow(q, 0.1);
		DEPTH = 0.0001;	
	

}
