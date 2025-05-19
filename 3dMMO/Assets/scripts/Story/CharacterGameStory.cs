using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterGameStory : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.OnPlayerDataReady += async () =>
        {
            CheckAndStartMainStory();
        };
    }
    void CheckAndStartMainStory()
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();

        if (myPlayer.characterPersonalinfo.storyNum == 0f)
        {
            // 강제 시작
            myPlayer.characterPersonalinfo.storyNum += 0.1f;
            StartMainStoryFirst();
        }
    }
    public async void StartMainStoryFirst()
    {
        await StoryManager.Instance.StartStory("MainFirst", "MainFirst");

    }
    public async void LoadMainStory()
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();
        await StoryManager.Instance.StartStory(myPlayer.characterPersonalinfo.currentstory_name, myPlayer.characterPersonalinfo.nextstory_name);
    }
}
