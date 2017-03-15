using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    void Awake ()
    {
        DontDestroyOnLoad(gameObject);
    }

	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.transform.position;
	}
}
