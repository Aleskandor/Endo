using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent nav;

    public Transform targetTransform;

    [HideInInspector]
    public bool pathAvailable;
    public NavMeshPath navMeshPath;

    void Start()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
    }

    void Update()
    {
        if (CalculateNewPath())
            nav.SetDestination(targetTransform.position);

        if (nav.velocity.magnitude > 0 && !animator.GetBool("Running"))
            animator.SetBool("Running", true);
        else if (nav.velocity.magnitude == 0 && animator.GetBool("Running"))
            animator.SetBool("Running", false);
    }

    bool CalculateNewPath()
    {
        nav.CalculatePath(targetTransform.position, navMeshPath);

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
            return false;
        else
            return true;
    }
}
