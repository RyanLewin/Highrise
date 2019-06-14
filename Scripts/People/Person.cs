using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Person : MonoBehaviour {

    public Transform target;
    protected NavMeshAgent agent;
    public House house;
    public ParticleSystem smoke;

    bool inrange = false;

    protected virtual void Awake ()
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
                if (agent.remainingDistance <= 1f)
                    StartCoroutine(InRange());
                else
                {
                    if (inrange)
                    {
                        inrange = false;
                        StopCoroutine(InRange());
                    }
                }
        }
        else
            agent.ResetPath();
    }

    IEnumerator InRange ()
    {
        inrange = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void SetTarget (Transform _target)
    {
        target = _target;
    }
}
