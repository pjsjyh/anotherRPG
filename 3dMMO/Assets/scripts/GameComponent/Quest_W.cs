using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_W : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject followplayer;
    void Start()
    {
        followplayer = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = followplayer.transform.position - this.transform.position;

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(-dir), Time.deltaTime * 10);
    }
    public void click()
    {
        Debug.Log("!!!!!!!!!!!");
    }
}
