using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrollspeed : MonoBehaviour
{
    public float ScrollX;
    public float ScrollY;

    private float offsetX;
    private float offsetY;

    // Update is called once per frame
    void Update()
    {
        offsetX = Time.time * ScrollX;
        offsetY = Time.time * ScrollY;

        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
