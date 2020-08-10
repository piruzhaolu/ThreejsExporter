using System;

namespace Piruzhaolu.ThreejsEditor
{
  
    [Serializable]
    public class Mat
    {
        public string id;
        public string type;
        public float[] color;
        public string map;
        public string metalnessMap;
        public string normalMap;
        public float metalness;
    }
}