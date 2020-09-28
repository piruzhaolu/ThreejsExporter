#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED


CBUFFER_START(_CustomLight)
    float3 _DirectionalLightColor;
    float3 _DirectionalLightDirection;
CBUFFER_END

struct Light
{
    float3 color;
    float3 direction;
};


Light GetDirectionalLight()
{
    Light light;
    light.color = _DirectionalLightColor;//float3(1.0,0.0,0.0);
    light.direction = _DirectionalLightDirection;//float3(0.5,0.5,0.0);
    return light;
}


#endif