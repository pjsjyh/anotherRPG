using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum npcState { idle, mainquest, subquest, mainquestFinish, subquestFinish, gameFinish};
[System.Serializable]
public class Chatword
{
    public string name;
    public string textword;
}
[System.Serializable]
public class NPCMovement
{
    public string npcID;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public float moveSpeed;
    public string animation;
    public bool lookAtTarget;
    public float waitTime;
}
public class InteractPlayer : MonoBehaviour
{
  
    public Chatword[] defualtWord;
    public Chatword[] mainQuestWord;
    public Chatword[] mainQuestFinishtWord;
    StoryManager sm;
    public string npcCharaterID;
    public npcState thisState = npcState.idle;
    private void Awake()
    {
        sm = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        NPCManager.Instance.RegisterNPC(this);
    }
    private void OnMouseDown()
    {
        if (thisState == npcState.idle)
            defaultChat(defualtWord);
        else if (thisState == npcState.mainquest)
        {
            defaultChat(mainQuestWord);
        }
            
    }
    public async void defaultChat(Chatword[] chat)
    {
        await sm.StartStoryFromArray(chat);
    }

}
