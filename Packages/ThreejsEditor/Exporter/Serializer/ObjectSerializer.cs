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
            CreateObjectID(allGameObjects);
            Serialize(allGameObjects, objPack);
            var json = JsonConvert.SerializeObject(objPack);// JsonUtility.ToJson(objPack);
            AssetDatabase.SaveScene(json, scene.name);
            EditorUtility.DisplayDialog("Threejs Editor","saved", "OK");
        }

        [MenuItem("Tools/Clear")]
        public static void Clear()
        {
            AssetDatabase.Clear();
        }

        [MenuItem("Tools/Normor")]
        public static void JsonTest()
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath("b993ae890161b184188e417d77e0e1a3");
            var tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            // var data = tex.GetRawTextureData();
            // var newTex = AssetDatabase.FormatConvert(tex);
            // var png = newTex.EncodeToPNG();
            // File.WriteAllBytes("Assets/a.png",png);
            
            byte[] pix = tex.GetRawTextureData();
            Texture2D readableText = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            readableText.LoadRawTextureData(pix);
            readableText.Apply();
            var png = readableText.EncodeToPNG();
            File.WriteAllBytes("Assets/a.png",png);
        }
        
#endif

        private static void CreateObjectID(GameObject[] gameObjects)
        {
            foreach (var o in gameObjects)
            {
                CreateObjectID(o);
            }
        }
        
        private static void CreateObjectID(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<ObjectID>(out _))
            {
                var objectID = gameObject.AddComponent<ObjectID>();
                objectID.ID = IDGenerate.Generate();
            }

            if (gameObject.transform.childCount > 0)
            {
                for (var i = 0; i < gameObject.transform.childCount; i++)
                {
                    CreateObjectID(gameObject.transform.GetChild(i).gameObject);
                }
            }
            
        }
        

        public static void Serialize(GameObject[] gameObjects, ObjPack objPack)
        {
            foreach (var go in gameObjects)
            {
                Serialize(go, objPack);
            }
        }
        

        public static void Serialize(GameObject gameObject, ObjPack objPack, string parent = "")
        {
            if (!gameObject.TryGetComponent<ObjectID>(out var objectID))
            {
                return;
            }
            var obj = new Obj();
            obj.type = "group";
            obj.id = objectID.ID.ToString();
            var pos = gameObject.transform.localPosition;
            obj.position = new []{pos.x,pos.y,pos.z};
            var q = gameObject.transform.localRotation;
            obj.quaternion = new[] {q.x, q.y, q.z, q.w};
            var s = gameObject.transform.localScale;
            obj.scale = new[] {s.x, s.y, s.z};
            obj.parent = parent;
            
            //var m = Matrix4x4.identity;
            //m.SetTRS(translation, rotation, scale);

            if (gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                var geometrie = AssetDatabase.TrySave<Geometrie>(meshFilter.sharedMesh);
                if (geometrie != null)
                {
                    obj.type = "mesh";
                    objPack.Add(geometrie);
                    obj.geometry = geometrie.id;
                }
            }
            if (gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                if (meshRenderer.sharedMaterials.Length > 1)
                {
                    var list = new List<string>();
                    for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                    {
                        var mat = AssetDatabase.TrySave<Mat>(meshRenderer.sharedMaterials[i]);
                        if (mat != null)
                        {
                            objPack.Add(mat);
                            list.Add(mat.id);
                        }
                    }
                    obj.materials = list.ToArray();
                }
                else
                {
                    var mat = AssetDatabase.TrySave<Mat>( meshRenderer.sharedMaterial);
                    if (mat != null)
                    {
                        objPack.Add(mat);
                        obj.material = mat.id;
                    }
                }
            }

            SaveData(gameObject, obj);
            objPack.objects.Add(obj);

            var childCount = gameObject.transform.childCount;
            if (childCount > 0)
            {
                for (var i = 0; i < childCount; i++)
                {
                    var childGo = gameObject.transform.GetChild(i).gameObject;
                    Serialize(childGo, objPack, obj.id);
                }
            }
        }
        

        private static void SaveData(GameObject gameObject, Obj obj)
        {
            if (gameObject.TryGetComponent<Light>(out var light))
            {
                if (obj.datas == null) obj.datas= new List<object>();
                if (light.type == LightType.Directional)
                {
                    obj.datas.Add(new ObjDataDirectionalLight(light));
                }
            }

            if (gameObject.TryGetComponent<Camera>(out var camera))
            {
                if (obj.datas == null) obj.datas= new List<object>();
                obj.datas.Add(new ObjDataCamera(camera));
            }
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
    
    
    

   

    
    
    

    

    
    
    
    
    
}