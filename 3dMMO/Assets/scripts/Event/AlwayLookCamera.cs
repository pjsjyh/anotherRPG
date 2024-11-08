using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwayLookCamera : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform.position);
            transform.Rotate(0, 180, 0);

        }
    }
}
