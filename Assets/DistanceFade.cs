using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFade : MonoBehaviour
{
    Shader shader;

    [Range(0,0.15f)]
    float distance;
    float density = 0;
    public Material mat;
    public GameObject human;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //distance = Vector3.Distance(human.transform.position, transform.position);
        //density += 0.001f;
        //mat.SetFloat("_Density", density);
        //shader.

    }
}
