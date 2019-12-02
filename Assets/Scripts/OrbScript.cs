using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public GameObject orb;
    public Transform holePivot;
    public GameObject curruptionPit;

    private bool move;
    private Vector3 startPos, targetPos;
    private float arcHeight, speed;
    // Start is called before the first frame update
    void Start()
    {
        arcHeight = 4;
        speed = 2;
        targetPos = holePivot.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            Vector2 pos = new Vector2(startPos.x, startPos.z);
            Vector2 posTarget = new Vector2(targetPos.x, targetPos.z);
            float distance = Vector2.Distance(posTarget, pos);
            Vector2 nextPlanePos = Vector2.MoveTowards(new Vector2(orb.transform.position.x, orb.transform.position.z), posTarget, speed * Time.deltaTime);

            float baseY = Mathf.Lerp(startPos.y, targetPos.y, Vector2.Distance(nextPlanePos, pos) / distance);
            float arc = arcHeight * Vector2.Distance(nextPlanePos, pos) * Vector2.Distance(nextPlanePos, posTarget) / (0.25f * distance * distance);
            Vector3 nextPos = new Vector3(nextPlanePos.x, baseY + arc, nextPlanePos.y);
            orb.transform.position = nextPos;

            if (nextPos == targetPos)
                curruptionPit.GetComponent<CurruptionHoleInteraction>().die = true;
        }
    }
    void SpawnOrb()
    {
        orb.SetActive(true);
    }
    private void MoveOrb()
    {
        move = true;
        startPos = orb.transform.position;
    }
}
