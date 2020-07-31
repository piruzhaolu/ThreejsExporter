using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

#endif

namespace Piruzhaolu.ThreejsEditor
{
    
    public class ObjectSerializer
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Run")]
        public static void Run()
        {
            // var json = Serialize(GameObject.Find("Cylinder"));
            
            //AssetDatabase.Save(GameObject.Find("Cylinder").GetComponent<MeshFilter>().sharedMesh);
           
            var objPack = new ObjPack();
            var scene = SceneManager.GetActiveScene();
            var allGameObjects = scene.GetRootGameObjects();
            Serialize(allGameObjects, objPack);
            var json = JsonUtility.ToJson(objPack);
            AssetDatabase.SaveScene(json, scene.name);
        }

       
#endif

        public static void Serialize(GameObject[] gameObjects, ObjPack objPack)
        {
            foreach (var go in gameObjects)
            {
                Serialize(go, objPack);
            }
        }
        

        public static void Serialize(GameObject gameObject, ObjPack objPack)
        {
            var obj = new Obj();
            obj.id = IDGenerate.Generate().ToString();
            var pos = gameObject.transform.localPosition;
            obj.position = new []{pos.x,pos.y,pos.z};
            
            var m = Matrix4x4.identity;
            //m.SetTRS(translation, rotation, scale);
            
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var geometrie = AssetDatabase.Save<Geometrie>(meshFilter.sharedMesh);
                objPack.geometries.Add(geometrie);
                obj.geometry = geometrie.id;
            }

            objPack.objects.Add(obj);
        }

        private static float[] MatrixToArray(Matrix4x4 m)
        {
            var f = new float[16];
            for (var i = 0; i < 16; i++)
            {
                f[i] = m[i];
            }

            return f;
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
        public float[] position;
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
        public string attr_normal;
        public string indexs;

    }
    
    
}