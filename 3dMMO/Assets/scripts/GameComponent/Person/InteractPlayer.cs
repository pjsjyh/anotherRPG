using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    StoryManager sm;
    public string npcCharaterID;
    private void Awake()
    {
        sm = GameObject.Find("StoryManager").GetComponent<StoryManager>();
    }
    private void OnMouseDown()
    {
        defaultChat();
    }
    public async void defaultChat()
    {
        await sm.StartStoryFromArray(defualtWord);
    }
}
