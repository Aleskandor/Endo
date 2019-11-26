using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private float timer;
    private bool larvaIconActive;

    private Vector3 screenPos;

    public GameObject marker;
    public GameObject pointer;
    public GameObject exclamationMark;
    public Transform dogTransform;
    public Transform larvaTransform;

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

        screenPos = Camera.main.WorldToScreenPoint(larvaTransform.position);

        if (screenPos.z > 0 &&
            screenPos.y > 0 && screenPos.y < Screen.height &&
            screenPos.x > 0 && screenPos.x < Screen.width &&
            larvaIconActive)
            exclamationMark.SetActive(true);
        else
            exclamationMark.SetActive(false);

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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Human")
            larvaIconActive = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Human")
            larvaIconActive = false;
    }
}
