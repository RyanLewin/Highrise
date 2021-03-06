﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject gameController;
    Driving driving;

    public Camera mainCam;
    public Camera driveCam;
    public Camera mapCam;

    public Canvas driveCanvas;

    public float minZoom, maxZoom, zoomSpeed;   // the value of maxZoom will be lower than minZoom (Don't question it)
    public float rotateSpeed;
    public float camSpeed;

    bool changeCamera = false;

    private float mouseXPos;
    //bool driving = false;

    void Start ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (!controller.GetComponent<CameraController>())
                gameController = controller;
        driving = gameController.GetComponent<Driving>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        driveCam = transform.GetChild(1).GetComponent<Camera>();
        mapCam = transform.GetChild(1).GetChild(0).GetComponent<Camera>();
    }

    Quaternion currRotation;

    private void Update()
    {
        if (changeCamera)
        {
            mainCam.enabled = !mainCam.enabled;
            driveCanvas.enabled = false;
            changeCamera = false;
            if (driving.driving)
            {
                currRotation = transform.rotation;
                transform.rotation = Quaternion.Euler(0, 0, 0);

                driveCanvas.enabled = true;
                driveCam.enabled = true;
                mapCam.enabled = true;
            }
            else
            {
                transform.rotation = currRotation;

                driveCam.enabled = false;
                mapCam.enabled = false;
            }
        }

        if (driving.driving == false)
        {
            /* ---------- CAMERA ZOOMING ----------- */
            float mouseZoom = Input.GetAxisRaw("Mouse ScrollWheel");
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - (zoomSpeed * mouseZoom), maxZoom, minZoom);

            /* ---------- CAMERA ROTATION ----------- */
            if (Input.GetKey(KeyCode.Q))
                transform.Rotate(Vector3.up, rotateSpeed/* * Time.deltaTime*/, Space.World);
                //Camera.main.transform.RotateAround(transform.position, Vector3.up, rotateSpeed);

            if (Input.GetKey(KeyCode.E))
                transform.Rotate(Vector3.down, rotateSpeed/* * Time.deltaTime*/, Space.World);
                //Camera.main.transform.RotateAround(transform.position, -Vector3.up, rotateSpeed);

            /* ---------- CAMERA MOVEMENT ----------- */
            float moveVertical = Input.GetAxisRaw("Vertical");
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            Vector3 verticalMovement = new Vector3(-moveVertical, 0, -moveVertical) * 2;
            Vector3 horizontalMovement = new Vector3(-moveHorizontal, 0, moveHorizontal);
            //Vector3 movement = verticalMovement + horizontalMovement;

            transform.Translate((verticalMovement + horizontalMovement)/* * Time.deltaTime*/ * camSpeed, Space.Self);
        }
    }

    IEnumerator Shake ()
    {
        for (int shakes = 0; shakes < 15; shakes++)
        {
            transform.localPosition += new Vector3(10, 0, 0);
            yield return new WaitForSeconds(.02f);
            transform.localPosition += new Vector3(-10, 0, 0);
            yield return new WaitForSeconds(.02f);
        }
    }

    public void CameraShake ()
    {
        StartCoroutine("Shake");
    }

    public bool ChangeCamera
    {
        set
        {
            changeCamera = value;
        }
    }

    public Camera GetCurrentCam
    {
        get
        {
            if (driveCam)
                if (driveCam.enabled)
                    return driveCam;
                else
                    return mainCam;
            else
                return mainCam;
        }
    }
}
