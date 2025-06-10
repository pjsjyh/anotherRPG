using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public Vector3 target_pos;
    public float stopDistance = 1.5f;
    public void OnClickQuestUI()
    {
        Debug.Log(target_pos);
        PlayerControll myPlayer = PlayerManager.Instance.GetMyPlayerControll();
        myPlayer.StartMoveToTargetNav(target_pos);
    }
}
