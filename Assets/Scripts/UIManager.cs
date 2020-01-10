using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private float timer;
    private bool larvaIconActive;

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

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButton("YButton"))
            ShowDogIcon();

        if (timer >= 0)
        {
            if (screenPos.z > 0 &&
                screenPos.y > 0 && screenPos.y < Screen.height &&
                screenPos.x > 0 && screenPos.x < Screen.width)
            {
                if (marker && pointer)
                {
                    if (marker.TryGetComponent<Image>(out Image markerImage))
                    {
                        markerImage.enabled = true;
                    }

                    
                    if (pointer.TryGetComponent<Image>(out Image pointerImage))
                    {
                        pointerImage.enabled = false;   
                    }
                }
            }
            else
            {
                if (marker && pointer)
                {
                    
                    if (marker.TryGetComponent<Image>(out Image markerImage))
                    {
                        markerImage.enabled = false;
                    }

                    
                    if (pointer.TryGetComponent<Image>(out Image pointerImage))
                    {
                        pointerImage.enabled = false;   
                    }
                }
            }
        }

        if (marker.GetComponent<Image>().enabled || pointer.GetComponent<Image>().enabled)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                if (marker.TryGetComponent<Image>(out Image markerImage))
                {
                    markerImage.enabled = false;
                }


                if (pointer.TryGetComponent<Image>(out Image pointerImage))
                {
                    pointerImage.enabled = false;
                }
                //marker.SetActive(false);
                //pointer.SetActive(false);
            }
        }    
    }

    public void ShowDogIcon()
    {
        if (!marker.GetComponent<Image>().enabled && !pointer.GetComponent<Image>().enabled)
        {
            timer = 4f;
        }
    }
}