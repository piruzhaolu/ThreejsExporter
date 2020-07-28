using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Piruzhaolu.ThreejsEditor
{
    
    public class ObjectSerializer
    {
        [MenuItem("Tools/Run")]
        public static void Run()
        {
            var json = Serialize(GameObject.Find("Cylinder"));
            Debug.Log(json);
        }
        
        
        public static string CreateID()
        {
            return "";
        }

        public static string Serialize(GameObject gameObject)
        {
            var objPack = new ObjPack();
            objPack.objects.Add(SerializeImpl(gameObject));
            objPack.geometries.Add(new Geometrie
            {
                id = "ad",
                attr_position = "./t.bin#0",
                indexs = "./t.bin#1"
            });
            return JsonUtility.ToJson(objPack);
        }

        private static Obj SerializeImpl(GameObject gameObject)
        {
            var obj = new Obj {geometry = "ad"};
            return obj;
        }
    }
    
    
    

    [Serializable]
    public class ObjPack
    {
        public Metadata metadata = new Metadata();
        public List<Obj> objects = new List<Obj>();
        public List<Mat> materials = new List<Mat>();
        public List<Geometrie> geometries = new List<Geometrie>();
    }

    
    [Serializable]
    public class Metadata
    {
        public string type = "object";
    }
    

    [Serializable]
    public class Obj
    {
        public string id;
        public string type;
        public float[] matrix;
        public string geometry;
        public string material;
        public List<string> children;
    }

    [Serializable]
    public class Mat
    {
        public string id;
        public string type;
        public string color;
    }
    
    [Serializable]
    public class Geometrie
    {
        public string id;
        public string type;
        public string attr_position;
        public string indexs;

    }
    
    
}