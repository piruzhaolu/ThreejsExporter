using System;

namespace Piruzhaolu.ThreejsEditor
{
    [Serializable]
    public class Geometrie
    {
        public string id;
        public string type;
        public string attr_position;
        public string attr_normal;
        public string indexs;
        public Sub[] groups;

        [Serializable]
        public class Sub
        {
            public int start;
            public int count;
            public int materialIndex;
        }
    }
    
}