using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Piruzhaolu.ThreejsEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        return;
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        var binBindle = new BinBundle();
        binBindle.Add(BytesUtility.ToBytes(mesh.vertices));
        binBindle.Add(BytesUtility.ToBytes(mesh.triangles));

        var bytes = binBindle.ToBytes();
        
        // var list = new List<float>();
        // foreach (var v in mesh.vertices)
        // {
        //     list.Add(v.x);
        //     list.Add(v.y);
        //     list.Add(v.z);
        // }
        //
        // var json = new Json()
        // {
        //     tr = list.ToArray(),
        //     triangles = mesh.triangles
        // };
        File.WriteAllBytes("D://t.bin", bytes);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }

}

public class Json
{
    public float[] tr;
    public int[] triangles;
}