using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergencies : MonoBehaviour {

    Building buildings;
    public CameraController cameraController;
    public GameObject earthquake;

    [Range(0, 10000)]
    public int chanceOfIllness = 1, chanceOfRobbery = 1, chanceOfEarthquake = 1, chanceOfFire = 1;

    float timer = 30;
    int chanceTime = 15;

    void Start ()
    {
        buildings = this.gameObject.GetComponent<Building>();
    }

    void Update ()
    {
        //timer = Mathf.Infinity;
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            int rand = Random.Range(0, 10000);
            
            if (rand <= chanceOfEarthquake)
            {
                int randX = Random.Range(0, buildings.size.x);
                int randZ = Random.Range(0, buildings.size.z);

                GameObject _earthquake = Instantiate(earthquake);
                _earthquake.transform.position = new Vector3(-randX * 10, 0, -randZ * 10);

                cameraController.CameraShake();
                Debug.Log("Earthquake " + randX + ", " + randZ);
                Destroy(_earthquake, 4f);
            }

            timer = chanceTime;
        }
    }
}
