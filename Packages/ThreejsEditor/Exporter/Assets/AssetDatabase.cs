using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Piruzhaolu.ThreejsEditor
{
    
    public class AssetDatabase
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        public static string AssetPath = Path.Combine(Path.GetDirectoryName(Application.dataPath),"Threejs/assets");

        private static Dictionary<string, object> _database = null;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        public static Dictionary<string, object> Database
        {
            get
            {
                if (_database != null) return _database;
                _database = new Dictionary<string, object>();
                var files = Directory.GetFiles(AssetPath);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".meta")
                    {
                        try
                        {
                            var fileText = File.ReadAllText(file);
                            JObject obj = JsonConvert.DeserializeObject(fileText) as JObject;
                            if (obj != null)
                            {
                                var type = (string) obj["type"];
                                switch (type)
                                {
                                    case "mesh":
                                        var geometrie = JsonUtility.FromJson<Geometrie>(fileText);
                                        _database[geometrie.id] = geometrie;
                                        break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                        
                    }
               
                }
                return _database;
            }
        }

        public static bool Has(UnityEngine.Object obj)
        {
            var guid = ObjectFullID(obj);
            return Has(guid);
        }

        public static bool Has(string guid)
        {
            return Database.ContainsKey(guid);
        }
        
        


        public static void SaveScene(string json, string name)
        {
            File.WriteAllText($"{AssetPath}/{name}.scene", json);
        }



        public static string ObjectFullID(Object obj)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            if (all.Length > 1)
            {
                guid += $"_{obj.name}";
            }

            return guid;
        }

        
        public static T TrySave<T>(Object obj) where T : class
        {
            var guid = ObjectFullID(obj);
            T value;
            if (Has(guid))
            {
                value = Database[guid] as T;
            }
            else
            {
                value = Save<T>(obj,guid);
            }

            return value;
        }

        public static void Save(Object obj)
        {
            Save<object>(obj);
        }


        public static T Save<T>(Object obj) where T : class
        {
            var guid = ObjectFullID(obj);
            return Save<T>(obj, guid);
        }
        
        internal static T Save<T>(Object obj, string fullID) where T : class
        {
            var guid = fullID;

            object returnValue = null;
            switch (obj)
            {
                case Mesh mesh:
                    returnValue = SaveMesh(mesh, guid);
                    break;
                case Material material:
                    returnValue = SaveMaterial(material, guid);
                    break;
                default:
                    return null;
            }

            Database[guid] = returnValue;
            return returnValue as T;
        }


        private static Mat SaveMaterial(Material material, string id)
        {
            var mat = new Mat{id = id, type = "mat"};
            var c = material.GetColor(BaseColor);
            mat.color = new[] {c.r, c.g, c.b};
            mat.map = SaveTexture(material.GetTexture(BaseMap) as Texture2D);
            return mat;
        }

        
        private static string SaveTexture(Texture2D texture)
        {
            if (texture == null) return string.Empty;
            
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
            var assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(assetGuid)) return string.Empty;

            var json = JsonUtility.ToJson(new Tex2d {id = assetGuid});
            var png =  DuplicateTexture(texture).EncodeToPNG();
            File.WriteAllText($"{AssetPath}/{assetGuid}.meta", json);
            File.WriteAllBytes($"{AssetPath}/{assetGuid}", png);
            return assetGuid;
        }
        
        private static Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        
        private static Geometrie SaveMesh(Mesh mesh, string id)
        {
            var geo = new Geometrie {id = id, type = "mesh"};
            
            var binBindle = new BinBundle();
            geo.attr_position = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.vertices))}";
            geo.indexs = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.triangles))}";
            geo.attr_normal = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.normals))}";
            geo.attr_uv  = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.uv))}";
             
            var subList = new List<Geometrie.Sub>();
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var subMesh = mesh.GetSubMesh(i);
                var sub = new Geometrie.Sub {start = subMesh.indexStart, count = subMesh.indexCount, materialIndex = i};
                subList.Add(sub);
            }

            if (subList.Count > 0)
            {
                geo.groups = subList.ToArray();
            }
            
            var bytes = binBindle.ToBytes();
            var json = JsonUtility.ToJson(geo);
            File.WriteAllText($"{AssetPath}/{id}.meta", json);
            File.WriteAllBytes($"{AssetPath}/{id}", bytes);

            return geo;
        }
        

    }
    
    
    
}