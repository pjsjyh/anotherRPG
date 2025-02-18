using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Chatword
{
    public string name;
    public string textword;
}

public class InteractPlayer : MonoBehaviour
{
  
    public Chatword[] defualtWord;
    StoryManager sm;
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
