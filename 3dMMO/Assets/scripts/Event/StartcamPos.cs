using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;

public class StartcamPos : MonoBehaviour
{
    public Vector3 cameraOffset = new Vector3(0.2f, 7.236f, -6.177f); // Y 높이와 Z 거리만 반영
    public Vector3 fixedRotation = new Vector3(34.241f, 0.963f, 0f); // 고정 회전값

    private void Awake()
    {
        var charInfo = GameManager.Instance.myDataSetting.characterPersonalinfo;
        var chaPos = new Vector3(charInfo.chaPosition[0], charInfo.chaPosition[1], charInfo.chaPosition[2]);
        var chaRot = Quaternion.Euler(charInfo.chaRotation[0], charInfo.chaRotation[1], charInfo.chaRotation[2]);

        // 회전에 따라 offset 방향 조정
        Vector3 rotatedOffset = chaRot * cameraOffset;
        transform.position = chaPos + rotatedOffset;

        // 회전은 고정
        transform.rotation = Quaternion.Euler(fixedRotation);

        Debug.Log($"📷 카메라 최종 위치: {transform.position}");
    }

    private void Start()
    {
        if (Camera.main != null)
            Camera.main.gameObject.SetActive(false);
    }
}
