using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    public Transform dogTransform;

    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(dogTransform.position);
    }
}
