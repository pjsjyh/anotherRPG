using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 버튼 공통 기능
public class BtnEvent : MonoBehaviour
{
    public void turnOff()
    {
        this.gameObject.SetActive(false);
    }
    public void turnOn()
    {
        this.gameObject.SetActive(true);
    }
}
