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


        public static bool Has(string guid)
        {
            return Database.ContainsKey(guid);
        }


        public static void SaveScene(string json, string name)
        {
            File.WriteAllText($"{AssetPath}/{name}.scene", json);
        }
        


        public static T Save<T>(Object obj) where T : class
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            if (all.Length > 0)
            {
                guid += $"_{obj.name}";
            }

            object returnValue = null;
            switch (obj)
            {
                case Mesh mesh:
                    returnValue = SaveMesh(mesh, guid);
                    break;
            }
            return returnValue as T;
        }

        
        private static Geometrie SaveMesh(Mesh mesh, string id)
        {
            var geo = new Geometrie {id = id, type = "mesh"};
            
            var binBindle = new BinBundle();
            geo.attr_position = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.vertices))}";
            geo.indexs = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.triangles))}";
            geo.attr_normal = $"{id}#{binBindle.Add(BytesUtility.ToBytes(mesh.normals))}";
            
            var bytes = binBindle.ToBytes();
            var json = JsonUtility.ToJson(geo);
            File.WriteAllText($"{AssetPath}/{id}.meta", json);
            File.WriteAllBytes($"{AssetPath}/{id}", bytes);

            return geo;
        }
        

    }
    
    
    
}