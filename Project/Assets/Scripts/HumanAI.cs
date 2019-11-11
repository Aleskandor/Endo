using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent nav;

    public Transform targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(targetTransform.position);
    }
}
