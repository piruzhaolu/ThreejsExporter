using System;
using System.Collections.Generic;
using UnityEngine;

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

        //===============================================
        //                Utility
        //===============================================
        
        public static float[] Color(Color color)
        {
            return new[]
            {
                color.r,
                color.g,
                color.b
            };
        }

        public static float[] Vector3(Vector3 vector3)
        {
            return new[]
            {
                vector3.x,
                vector3.y,
                vector3.z
            };
        }
        
    }
}