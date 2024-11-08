using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public void turnOff()
    {
        this.gameObject.SetActive(false);
    }
    public void turnOn()
    {
        this.gameObject.SetActive(true);
    }
}
