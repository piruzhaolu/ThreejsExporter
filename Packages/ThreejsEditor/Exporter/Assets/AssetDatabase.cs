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
        private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
        private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

        internal static void Clear()
        {
            _database = null;
        }
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
                                    case "tex2d":
                                        var tex2d = JsonUtility.FromJson<Tex2d>(fileText);
                                        _database[tex2d.id] = tex2d;
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
            if (obj == null) return null;
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
                case Texture2D texture2D:
                    returnValue = SaveTexture(FormatConvert(texture2D), guid);
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

            var tex2d = TrySave<Tex2d>(material.GetTexture(BaseMap) as Texture2D);
            if (tex2d != null) mat.map = tex2d.id;

            var metallicGlossMapTex = material.GetTexture(MetallicGlossMap) as Texture2D;
            if (metallicGlossMapTex != null)
            {
                var guid = ObjectFullID(metallicGlossMapTex);
                if (Has(guid))
                {
                    tex2d = Database[guid] as Tex2d;
                }
                else
                {
                    var mTex = FormatConvert(metallicGlossMapTex, true);
                    tex2d = SaveTexture(mTex,guid);
                }
                if (tex2d != null) mat.metalnessMap = tex2d.id;
            }

            tex2d = TrySave<Tex2d>(material.GetTexture(BumpMap) as Texture2D);
            if (tex2d != null) mat.normalMap = tex2d.id;
            

            mat.metalness = material.GetFloat(Smoothness);
            return mat;
        }
        
        
        
        

        private static Tex2d SaveTexture(Texture2D texture, string assetGuid)
        {
            // if (texture == null) return string.Empty;
            //
            // var assetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
            // var assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            // if (string.IsNullOrEmpty(assetGuid)) return string.Empty;
            // if (Database.ContainsKey(assetGuid)) return assetGuid;

            var tex2d = new Tex2d {id = assetGuid, type = "tex2d"};
            var json = JsonUtility.ToJson(tex2d);
            byte[] fileBytes;
            if (texture.format == TextureFormat.RGB24)
            {
                fileBytes = texture.EncodeToJPG();
            }
            else
            {
                fileBytes = texture.EncodeToPNG();
            }

            File.WriteAllText($"{AssetPath}/{assetGuid}.meta", json);
            File.WriteAllBytes($"{AssetPath}/{assetGuid}", fileBytes);
            // Database[assetGuid] = tex2d;
            return tex2d;
        }

        
        public static Texture2D FormatConvert(Texture2D texture2D, bool channelExchange = false)
        {
            if (texture2D == null) return null;
            var newTexture = DuplicateTexture(texture2D);
            if (channelExchange)
            {
                ChannelExchange(newTexture);
            }
            return newTexture;
        }
        

        private static void ChannelExchange(Texture2D tex2d)
        {
            for (var x = 0; x< tex2d.width; x++)
            {
                for (var y = 0; y < tex2d.height; y++)
                {
                    var color = tex2d.GetPixel(x, y);
                    var newColor = new Color(color.b,color.a,color.r,color.g);
                    tex2d.SetPixel(x,y, newColor);
                }
            }
        }
        
        private static Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Default);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            
            Texture2D readableText;
            if (source.format == TextureFormat.DXT1)
            {
                readableText = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
            }
            else
            {
                readableText = new Texture2D(source.width, source.height,TextureFormat.RGBA32,false);
            }
               
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