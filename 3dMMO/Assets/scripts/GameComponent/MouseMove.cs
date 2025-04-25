using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    public Transform objectTofollow;
    public float followSpeed = 100f;
    public float sensitivity = 150f;
    public float clapAngle = 70f;

    private float rotX;
    private float rotY;
    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10f;
    public Player player;

    private bool isInitialized = false;

    private void Start()
    {

        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
    }

    private void Update()
    {

        if (!isInitialized) return; 
        cameramove();
    }
    public void InitializeCamera(Transform target, Player playerData)
    {
        objectTofollow = target;
        player = playerData;
        isInitialized = true;
    }
    private void LateUpdate()
    {
        if (!isInitialized) return;
        transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, followSpeed*Time.deltaTime);
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if(Physics.Linecast(transform.position,finalDir,out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }
        if(!player.isJump)
            realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime*smoothness);
    }
    void cameramove()
    {
        if (Input.GetMouseButton(0)||Input.GetMouseButton(1))
        {

            rotX += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clapAngle, clapAngle);
            Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
            transform.rotation = rot;
        }
    }

}
