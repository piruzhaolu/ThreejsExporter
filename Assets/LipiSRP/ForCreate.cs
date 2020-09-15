using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForCreate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Screen.dpi);
        var cube = GameObject.Find("Cube");
        for (var i = 0; i < 50; i++)
        {
            var c = Object.Instantiate(cube);
            c.transform.position = new Vector3(Random.Range(-10f,10f),0, Random.Range(-10f,10f));
            
           // c.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Random.ColorHSV());
            
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", Random.ColorHSV());
            c.GetComponent<MeshRenderer>().SetPropertyBlock(block);
        }

        var sphere = GameObject.Find("Sphere");
        for (var i = 0; i < 50; i++)
        {
            var c = Object.Instantiate(sphere);
            c.transform.position = new Vector3(Random.Range(-10f,10f),0, Random.Range(-10f,10f));
           // c.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Random.ColorHSV());
            
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", Random.ColorHSV());
            c.GetComponent<MeshRenderer>().SetPropertyBlock(block);
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
