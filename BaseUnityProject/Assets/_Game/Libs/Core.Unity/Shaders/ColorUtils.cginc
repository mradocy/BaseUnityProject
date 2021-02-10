#ifndef COLOR_UTILS
#define COLOR_UTILS

// To use, insert #include "Assets/_Game/Libs/Core.Unity/Shaders/ColorUtils.cginc" in the CPROGRAM of the shader

// HSL functions from https://www.shadertoy.com/view/4dKcWK

const float EPSILON = 1e-10;

fixed3 HUEtoRGB(float hue) {
    // Hue [0..1] to RGB [0..1]
    // See http://www.chilliant.com/rgb2hsv.html
    fixed3 rgb = abs(hue * 6. - fixed3(3, 2, 4)) * fixed3(1, -1, -1) + fixed3(-1, 2, 2);
    return clamp(rgb, 0., 1.);
}

fixed3 RGBtoHCV(fixed3 rgb) {
    // RGB [0..1] to Hue-Chroma-Value [0..1]
    // Based on work by Sam Hocevar and Emil Persson
    float4 p = (rgb.g < rgb.b) ? float4(rgb.bg, -1., 2. / 3.) : float4(rgb.gb, 0., -1. / 3.);
    float4 q = (rgb.r < p.x) ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);
    float c = q.x - min(q.w, q.y);
    float h = abs((q.w - q.y) / (6. * c + EPSILON) + q.z);
    return fixed3(h, c, q.x);
}

fixed3 HSVtoRGB(fixed3 hsv) {
    // Hue-Saturation-Value [0..1] to RGB [0..1]
    fixed3 rgb = HUEtoRGB(hsv.x);
    return ((rgb - 1.) * hsv.y + 1.) * hsv.z;
}

fixed3 HSLtoRGB(fixed3 hsl) {
    // Hue-Saturation-Lightness [0..1] to RGB [0..1]
    fixed3 rgb = HUEtoRGB(hsl.x);
    float c = (1. - abs(2. * hsl.z - 1.)) * hsl.y;
    return (rgb - 0.5) * c + hsl.z;
}

fixed3 RGBtoHSV(fixed3 rgb) {
    // RGB [0..1] to Hue-Saturation-Value [0..1]
    fixed3 hcv = RGBtoHCV(rgb);
    float s = hcv.y / (hcv.z + EPSILON);
    return fixed3(hcv.x, s, hcv.z);
}

fixed3 RGBtoHSL(fixed3 rgb) {
    // RGB [0..1] to Hue-Saturation-Lightness [0..1]
    fixed3 hcv = RGBtoHCV(rgb);
    float z = hcv.z - hcv.y * 0.5;
    float s = hcv.y / (1. - abs(z * 2. - 1.) + EPSILON);
    return fixed3(hcv.x, s, z);
}

#endif // COLOR_UTILS