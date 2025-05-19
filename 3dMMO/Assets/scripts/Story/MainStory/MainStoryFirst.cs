using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using CharacterInfo;


namespace StoryMain
{
    public class MainStoryFirst : StoryManager
    {
        public GameObject personOld;
        async public void Start()
        {
            GameManager.Instance.OnPlayerDataReady += async () =>
            {
                //await LoadStory();
            };
            //await MainQuestStart();

        }
        public async void StartMainQuest()
        {
            await LoadStory();
        }
        async public Task LoadStory()
        {
            var myPlayer = PlayerManager.Instance.GetMyCharacterData();
            float charaterstory = myPlayer.characterPersonalinfo.storyNum;

            int intchange = (int)Mathf.FloorToInt(charaterstory);
            Debug.Log(intchange + "스토리 시작" + myPlayer.characterPersonalinfo.storyNum);

            if (intchange == 0)
            {
                if (charaterstory == 0.0f)
                {
                    await StartStory("MainStory/MS_1", "MainFirst");
                    myPlayer.characterPersonalinfo.storyNum += 0.1f;

                }
                if (charaterstory == 0.2f)
                {
                    await StartStory("MainStory/MS_1", "MainSecond");
                    myPlayer.characterPersonalinfo.storyNum += 0.1f;

                }
                if (charaterstory == 0.4f)
                {
                    await StartStory("MainStory/MS_1", "MainThird");
                    myPlayer.characterPersonalinfo.storyNum += 0.1f;

                }
            }

            return;
        }
        async public Task MainQuestStart()
        {
            await StartStory("MainStory/MS_1", "MainFirst");
            // StartCoroutine(MoveAndLookAtTarget(new Vector3(7, 0, 1)));
            return;
        }




    }

}
