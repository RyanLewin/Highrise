using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    void Start () { Destroy(gameObject, 5f); }
    
    void Update ()
    {
        GetComponent<Rigidbody>().AddForce(transform.right, ForceMode.Impulse);
    }

    void OnTriggerEnter (Collider other)
    {
        //If the collider is a trigger, check its not a buildings trigger else do nothing
        if (other.isTrigger)
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildings"))
                return;
        Destroy(gameObject); //Destroy the bullet if it hits something
    }
}
