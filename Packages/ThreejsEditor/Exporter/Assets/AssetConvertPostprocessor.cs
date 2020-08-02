using System.IO;
using UnityEditor;
using UnityEngine;

namespace Piruzhaolu.ThreejsEditor
{
    public class AssetConvertPostprocessor:AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                var ext = Path.GetExtension(str);
                if (ext == ".cs" || ext == ".unity")
                {
                    continue;
                }
                var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(str);
                foreach (var item in all)
                {
                    Save(item);
                }
            }
            
            // foreach (string str in deletedAssets)
            // {
            //     Debug.Log("Deleted Asset: " + str);
            // }

            // for (int i = 0; i < movedAssets.Length; i++)
            // {
            //     Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            // }
        }


        static void Save(Object obj)
        {
            if (AssetDatabase.Has(obj))
            {
                AssetDatabase.Save<object>(obj);
            }
        }
    }
}