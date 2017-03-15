using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour {

    public Transform target;
    protected NavMeshAgent agent;
    public House house;

    protected void Awake ()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            if (agent.hasPath)
                if (agent.remainingDistance <= 2f)
                    Destroy(this.gameObject, 0.5f);
        }
        else
            agent.ResetPath();
    }

    void OnDestroy ()
    {
        Debug.Log("Destroyed");
    }

    public void SetTarget (Transform _target)
    {
        target = _target;
    }
}
