using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Youki Iimori
/// This class exists with the purpose of allowing the user to change from camera to camera
/// No known errors
/// </summary>
public class CameraManager : MonoBehaviour {

    // Use this for initialization
    public Camera[] cameras; //camera array that holds a reference to every camera in the scene

    private int currentCameraIndex;
    void Start()
    {

        currentCameraIndex = 0;
        for (int i = 1; i < cameras.Length; i++)
        {
            //sets cameras to active by default
            cameras[i].gameObject.SetActive(false);
        }

        //If any cameras were added to the controller, enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //press the 'c' key to cycle through cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Cycle to the next camera
            currentCameraIndex++;
        }

        //If cameraIndex is in bounds, set this camera active and last one inactive
        if (currentCameraIndex < cameras.Length)
        {
            if (currentCameraIndex != 0)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
            }
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
        //If last camera, cycle back to first camera
        else
        {
            cameras[currentCameraIndex - 1].gameObject.SetActive(false);
            currentCameraIndex = 0;
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }
}
