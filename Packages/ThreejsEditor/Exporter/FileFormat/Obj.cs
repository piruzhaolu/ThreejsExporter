using System;
using System.Collections.Generic;

namespace Piruzhaolu.ThreejsEditor
{
    [Serializable]
    public class Obj
    {
        public string id;
        public string type;
        public float[] position;
        public float[] quaternion;
        public float[] matrix;
        public string geometry;
        public string material;
        public string[] materials;
        public string parent;
    }
}