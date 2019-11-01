using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    private Camera cam;

    void Awake ()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // rotate the object to face the camera
        transform.eulerAngles = cam.transform.eulerAngles;
    }
}