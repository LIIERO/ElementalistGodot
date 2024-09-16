shader_type canvas_item;

uniform vec3 shirtColor = vec3(0.16f, 0.8f, 0.16f);
uniform vec3 sleeveColor = vec3(0.12f, 0.6f, 0.12f);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 pixelColor = texture(TEXTURE, UV);

	if (abs(pixelColor.b - 1.0f) < 0.0001) {
		COLOR = vec4(sleeveColor.r, sleeveColor.g, sleeveColor.b, COLOR.a);
	}

	if (abs(pixelColor.g - 1.0f) < 0.0001) {
		COLOR = vec4(shirtColor.r, shirtColor.g, shirtColor.b, COLOR.a);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
