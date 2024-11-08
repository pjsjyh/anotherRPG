using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using TMPro;
public class StoryManager : MonoBehaviour
{
    public GameObject chatUI;
    public TextMeshProUGUI storyName;
    public TextMeshProUGUI storyDescription;
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        public string dialogue;
    }

    [System.Serializable]
    public class Scene
    {
        public string sceneName;
        public List<Dialogue> dialogues;
    }

    [System.Serializable]
    public class StoryContainer
    {
        public List<Scene> story;  // "story"에 대응
    }
    protected virtual void Awake()
    {
        if (chatUI == null)
        {
            chatUI = GameObject.Find("GameChatPanel").transform.GetChild(0).gameObject; // 기본적으로 오브젝트를 찾아 할당
            storyName = GameObject.Find("GameChatPanel").transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            storyDescription = GameObject.Find("GameChatPanel").transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        }
    }
    public async Task StartStory(string filename, string sceneName)
    {
        await RunCoroutineAsTask(StoryJSONtoRead(filename, sceneName));
    }

    private Task RunCoroutineAsTask(IEnumerator coroutine)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(WaitForCoroutine(coroutine, tcs));
        return tcs.Task;
    }

    private IEnumerator WaitForCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
        yield return StartCoroutine(coroutine);
        tcs.SetResult(true);
    }

    public IEnumerator StoryJSONtoRead(string filename, string sceneName)
    {
        string path = Path.Combine(Application.dataPath, "scripts", "Story", filename + ".json");

        // 파일의 텍스트를 string으로 저장
        string jsonData = File.ReadAllText(path);
        // 이 Json데이터를 역직렬화하여 playerData에 넣어줌
        StoryContainer storyData = JsonUtility.FromJson<StoryContainer>(jsonData);
        chatUI.SetActive(true);
        foreach (var scene in storyData.story)
        {
            if (scene.sceneName == sceneName)
            {
                Debug.Log("Scene Name: " + scene.sceneName);  // 씬 이름 출력
                foreach (var dialogue in scene.dialogues)
                {
                    yield return StartCoroutine(ShowDialogueCoroutine(dialogue.name, dialogue.dialogue)); // await 추가
                }
                break;
            }
        }
        chatUI.SetActive(false);

    }
    public IEnumerator ShowDialogueCoroutine(string name, string description)
    {
        storyName.text = name;
        storyDescription.text = description;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.5f);
    }
}