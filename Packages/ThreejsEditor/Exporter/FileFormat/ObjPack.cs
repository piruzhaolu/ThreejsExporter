using System;
using System.Collections.Generic;

namespace Piruzhaolu.ThreejsEditor
{
    [Serializable]
    public class ObjPack
    {
        public Metadata metadata = new Metadata();
        public List<Obj> objects = new List<Obj>();
        public List<Mat> materials = new List<Mat>();
        public List<Geometrie> geometries = new List<Geometrie>();


        internal void Add(Mat mat)
        {
            foreach (var m in materials)
            {
                if (m.id == mat.id) return;
            }
            materials.Add(mat);
        }
        internal void Add(Geometrie geometrie)
        {
            foreach (var g in geometries)
            {
                if (g.id == geometrie.id) return;
            }
            geometries.Add(geometrie);
        }
        
    }
}