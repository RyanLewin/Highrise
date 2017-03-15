using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllPerson : Person {
    
    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Vehicle/Ambulance")
            StartCoroutine(Delay(other));
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "Vehicle/Ambulance")
            StopCoroutine(Delay(other));
    }

    IEnumerator Delay (Collider other)
    {
        yield return new WaitForSeconds(.5f);
        house.Collected();
        other.GetComponent<Vehicle>().totalPassengers++;
        Destroy(gameObject);
    }
}
