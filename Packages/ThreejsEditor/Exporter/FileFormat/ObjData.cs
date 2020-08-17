using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Piruzhaolu.ThreejsEditor
{
    [Serializable]
    public class ObjData
    {
        public string type;
    }

    [Serializable]
    public class ObjDataDirectionalLight : ObjData
    {
        public float[] position;
        public bool castShadow;
        public float[] color;
        public float intensity;
        public float shadowNear ;
        public float shadowFar = 100;
        public int shadowMapSizeW = 1024;
        public int shadowMapSizeH = 1024;
        public int shadowCameraLeft = -100;
        public int shadowCameraRight = 100;
        public int shadowCameraTop = 100;
        public int shadowCameraBottom = -100;

        public ObjDataDirectionalLight(Light light)
        {
            type = TypeConst.DirectionalLight;
            castShadow = light.shadows != LightShadows.None;
            intensity = light.intensity;
            color = ObjPack.Color(light.color);
            var v3 = light.transform.rotation * Vector3.back*2;
            position = ObjPack.Vector3(v3);
            shadowNear = light.shadowNearPlane;
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset asset)
            {
                shadowMapSizeW = shadowMapSizeH = asset.mainLightShadowmapResolution;
            }

        }
    }

    [Serializable]
    public class ObjDataCamera : ObjData
    {
        public float fov;
        public float aspect;
        public float near;
        public float far;
        
        public ObjDataCamera(Camera camera)
        {
            type = TypeConst.PerspectiveCamera;
            fov = camera.fieldOfView;
            aspect = camera.aspect;
            near = camera.nearClipPlane;
            far = camera.farClipPlane;
        }
    }
    
}