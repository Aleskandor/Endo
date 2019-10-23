using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowerAI : MonoBehaviour
{
    private NavMeshAgent nav;

    public Transform targetTransform;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        nav.SetDestination(targetTransform.position);
    }
}
