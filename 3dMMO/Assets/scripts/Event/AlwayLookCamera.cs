using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwayLookCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform.position);
            transform.Rotate(0, 180, 0);

        }
    }
}
