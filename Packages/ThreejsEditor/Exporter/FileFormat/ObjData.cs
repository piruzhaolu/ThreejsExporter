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
        public float shadowNear = -100;
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


            if (light.transform.TryGetComponent<ShadowSettings>(out var settings))
            {
                shadowNear = settings.shadowNear;
                shadowFar = settings.shadowFar;
                shadowMapSizeW = settings.shadowMapSizeW;
                shadowMapSizeH = settings.shadowMapSizeH;
                
                shadowCameraLeft = settings.shadowCameraLeft;
                shadowCameraRight = settings.shadowCameraRight;
                shadowCameraTop = settings.shadowCameraTop;
                shadowCameraBottom = settings.shadowCameraBottom;
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

    [Serializable]
    public class ObjDataAmbientLight : ObjData
    {
        public ObjDataAmbientLight()
        {
            type = TypeConst.AmbientLight;
        }
        
        public float[] color;
    }

    [Serializable]
    public class ObjDataHemisphereLight : ObjData
    {
        public ObjDataHemisphereLight()
        {
            type = TypeConst.HemisphereLight;
        }
        
        public float[] skyColor;
        public float[] groundColor;
    }

    [Serializable]
    public class ObjDataSkybox : ObjData
    {
        public ObjDataSkybox(Material skymat)
        {
            type = TypeConst.Skybox;

            px = TexId(skymat, "_LeftTex");
            nx = TexId(skymat, "_RightTex");
            
            py = TexId(skymat, "_UpTex");
            ny = TexId(skymat, "_DownTex");
            
            pz = TexId(skymat, "_FrontTex");
            nz = TexId(skymat, "_BackTex");
        }

        private string TexId(Material skymat, string texName)
        {
            var tpx = AssetDatabase.TrySave<Tex2d>(skymat.GetTexture(texName) as Texture2D);
            if (tpx != null) return tpx.id;
            return "";
        }

        public string px;
        public string nx;
        public string py;
        public string ny;
        public string pz;
        public string nz;


    }
    
}