using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryMain;

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
    public MainStoryFirst storyStarter;
    public npcState thisState = npcState.idle;
    private void Awake()
    {
        sm = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        NPCManager.Instance.RegisterNPC(this);
        if (storyStarter != null)
        {
            storyStarter = GameObject.Find("QuestUISetting/MainQuest/First").GetComponent<MainStoryFirst>();
        }
        Debug.Log(storyStarter);
    }
    private void OnMouseDown()
    {
        Interact();
        if (thisState == npcState.idle)
            defaultChat(defualtWord);
        else if (thisState == npcState.mainquest)
        {
            var myPlayer = PlayerManager.Instance.GetMyCharacterData();
            //defaultChat(mainQuestWord);
            if (storyStarter == null)
            {
                storyStarter = GameObject.Find("QuestUISetting/MainQuest/First").GetComponent<MainStoryFirst>();
                storyStarter.StartMainQuest();
                Debug.Log(storyStarter);
            }
            else
            {
                Debug.Log(storyStarter);
                storyStarter.StartMainQuest();
            }
            thisState = npcState.domainquest;
        }
        else if(thisState == npcState.domainquest)
        {

        }
        else if(thisState == npcState.mainquestFinish)
        {

        }
            
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
