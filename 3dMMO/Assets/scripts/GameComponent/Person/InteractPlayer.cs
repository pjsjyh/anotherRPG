using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum npcState { idle, mainquest, domainquest, subquest, mainquestFinish, subquestFinish, gameFinish};
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
    public GameObject mainButton;
    public bool isInteractOK = true;
    [SerializeField]
    private npcState _thisState = npcState.idle;
    public npcState ThisState
    {
        get => _thisState;
        set
        {
            if (_thisState != value)
            {
                _thisState = value;
                Debug.Log("값 바뀜 "+value);
                OnStateChanged(_thisState); // 상태가 바뀔 때 실행될 메서드
            }
        }
    }


    private void Start()
    {
        sm = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        NPCManager.Instance.RegisterNPC(this);

    }
    private void OnStateChanged(npcState newState)
    {
        switch (newState)
        {
            case npcState.mainquest:
                mainButton.SetActive(true);
                break;
            case npcState.domainquest:
                isInteractOK = true;
                break;
            case npcState.mainquestFinish:

                isInteractOK = true;
                mainButton.SetActive(false);
                break;
        }
    }
    private void OnMouseDown()
    {
        mainButton.SetActive(false);
        Interact();
        if (_thisState == npcState.idle)
            defaultChat(defualtWord);
        else if (_thisState == npcState.mainquest)
        {
            isInteractOK = false;
            StoryManager.Instance.LoadMainStory();
            _thisState = npcState.domainquest;
        }
        else if(_thisState == npcState.domainquest)
        {
        }
        else if(_thisState == npcState.mainquestFinish)
        {
           
        }
        OnStateChanged(_thisState);
    }
    public async void defaultChat(Chatword[] chat)
    {
        await sm.StartStoryFromArray(chat);
    }
    public void Interact()
    {
        QuestManager.Instance.OnTalkedTo(npcCharaterID);
    }
}
