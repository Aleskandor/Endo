using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private float timer;

    private Vector3 screenPos;

    public GameObject marker;
    public GameObject pointer;
    public Transform dogTransform;

    private void Start()
    {
        timer = 0f;

        screenPos = new Vector3();
    }

    private void Update()
    {
        screenPos = Camera.main.WorldToScreenPoint(dogTransform.position);

        if (Input.GetKeyDown(KeyCode.E))
            ShowDogIcon();

        if (timer >= 0)
        {
            if (screenPos.z > 0 &&
                screenPos.y > 0 && screenPos.y < Screen.height &&
                screenPos.x > 0 && screenPos.x < Screen.width)
            {
                marker.SetActive(true);
                pointer.SetActive(false);
            }
            else
            {
                marker.SetActive(false);
                pointer.SetActive(true);
            }
        }

        if (marker.activeSelf || pointer.activeSelf)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                marker.SetActive(false);
                pointer.SetActive(false);
            }
        }
    }

    public void ShowDogIcon()
    {
        if (!marker.activeSelf && !pointer.activeSelf)
        {
            timer = 4f;
        }
    }
}
