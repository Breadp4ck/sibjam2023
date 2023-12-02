shader_type canvas_item;

render_mode unshaded;

#define MAXCOLORS 16

uniform bool enabled = true;
uniform bool dithering = true;
uniform int colors : hint_range(1, MAXCOLORS) = 12;
uniform int dither_size: hint_range(1, 8) = 2;
uniform float coeff : hint_range(0, .5);


float dithering_pattern(ivec2 fragcoord) {
	const float pattern[] = {
		0.00, 0.50, 0.10, 0.65, 
		0.75, 0.25, 0.90, 0.35, 
		0.20, 0.70, 0.05, 0.50, 
		0.95, 0.40, 0.80, 0.30
	};
	
	int x = fragcoord.x % 4;
	int y = fragcoord.y % 4;
	
	return pattern[y * 4 + x];
}

float reduce_color(float raw, float dither, int depth) {
	float div = 1.0 / float(depth);
	float val = 0.0;
	int i = 0;

	while (i <= MAXCOLORS)
	{
		if (raw > div * (float(i + 1))) {
			i = i + 1;
			continue;
		}

		if (raw * float(depth) - float(i) <= dither * 0.999)
		{
			val = div * float(i);
		} 
		else
		{
			val = div * float(i + 1);
		}
		return val;

		i = i+1;
	}

	return val;
}

vec2 fish_eye(vec2 uv) {
	vec2 suv = uv;
	
	//side maps 0.0>1.0 into -1.0>1.0
	//side as in "-1.0 is the left side, 1.0 is the right one"
	float side = (uv.y * 2.0) - 1.0;
	
	//mountain maps 0.0>1.0 into a 0.0>0.0, where the mid-value is 1.0.
	float mountain = -abs((uv.x * 2.0) - 1.0) + 1.0;
	
	//maps mountain into a sine-wave's first ramp
	mountain = mountain * PI/2.0;
	
	//newv says 'how much should the pixel be moved based in its position?'
	//mountain defines the amount, coeff scales it and 'sin' smooths it out.
	//the multiplication with PI/2.0 is mandatory for sin to work
	float newv = coeff * sin(mountain);
	
	//modifies the screen uv saved before
	//(newv * side) applies the effect on both left and right.
	//if 'side' wasn't here, the effect would be applied only one way.
	//even more important is the subtraction with 'coeff*size'.
	//this scales the shader up and down so that you don't end up with borders.
	suv.y += ((newv * side) - (coeff*side));
	
	//updates the texture
	return suv;
}

void fragment() {
	
	
	vec4 raw = texture(TEXTURE, fish_eye(SCREEN_UV));
	ivec2 uv = ivec2(FRAGCOORD.xy / float(dither_size));

	if (enabled == true){
		float dithering_value = 1.0;
		if (dithering)
		{
			dithering_value = dithering_pattern(uv);
		}
		
		COLOR.r = reduce_color(raw.r, (dithering_value - 0.5) * dithering_value + 0.5, colors - 1);
		COLOR.g = reduce_color(raw.g, (dithering_value - 0.5) * dithering_value + 0.5, colors - 1);
		COLOR.b = reduce_color(raw.b, (dithering_value - 0.5) * dithering_value + 0.5, colors - 1);

	} else {
		COLOR.rgb = raw.rgb;
	}
}