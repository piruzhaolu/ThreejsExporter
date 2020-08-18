using UnityEngine;

namespace Piruzhaolu.ThreejsEditor
{
    public class ShadowSettings:MonoBehaviour
    {
        public int shadowMapSizeW = 1024;        
        public int shadowMapSizeH = 1024;        
        
        public float shadowNear = -100;
        public float shadowFar = 100;
        public int shadowCameraLeft = -100;
        public int shadowCameraRight = 100;
        public int shadowCameraTop = 100;
        public int shadowCameraBottom = -100;
    }
}