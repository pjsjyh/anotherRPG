﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using TMPro;

using CharacterInfo;
public class StoryManager : MonoBehaviour
{
    public GameObject chatUI;
    public TextMeshProUGUI storyName;
    public TextMeshProUGUI storyDescription;
    public GameObject npcGroup;
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        public string dialogue;
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

    [System.Serializable]
    public class Scene
    {
        public string sceneName;
        public List<Dialogue> dialogues;
        public string quest;
        public List<NPCMovement> npcMovements;
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
        string questid="";
        foreach (var scene in storyData.story)
        {
            if (scene.sceneName == sceneName)
            {
                questid = scene.quest;
                Debug.Log("Scene Name: " + scene.sceneName);  // 씬 이름 출력
                foreach (var dialogue in scene.dialogues)
                {
                    yield return StartCoroutine(ShowDialogueCoroutine(dialogue.name, dialogue.dialogue)); // await 추가
                }
                RunCoroutineAsTask(MoveNPCs(scene));
                break;
            }

        }
        chatUI.SetActive(false);

        if (questid != "")
        {
            QuestUISetting.Instance.GetQuestByID(questid);

        }
    }
    public IEnumerator ShowDialogueCoroutine(string name, string description)
    {
        storyName.text = name;
        storyDescription.text = description;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.5f);
    }
    public async Task StartStoryFromArray(Chatword[] story)
    {
        await RunCoroutineAsTask(ShowDialogueFromArray(story));
    }

    private IEnumerator ShowDialogueFromArray(Chatword[] story)
    {
        chatUI.SetActive(true);

        for (int i = 0; i < story.Length; i++)
        {
            storyName.text = story[i].name;
            storyDescription.text = story[i].textword;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // 클릭 대기
            yield return new WaitForSeconds(0.5f); // 딜레이
        }

        chatUI.SetActive(false);
    }
    public IEnumerator MoveNPCs(Scene scene)
    {
        foreach (var movement in scene.npcMovements)
        {
            Debug.Log(movement.targetPosition);
            if (npcGroup == null)
            {
                npcGroup = GameObject.Find("Scene1");
            }

            List<GameObject> npcs = new List<GameObject>();

            foreach (Transform child in npcGroup.transform) 
            {
                if (child.CompareTag("NPC")) 
                {
                    npcs.Add(child.gameObject);
                }
            }

            GameObject npc = null;

            foreach (var obj in npcs)
            {
                Debug.Log(obj.name);
                if (obj != null && obj.GetComponent<InteractPlayer>().npcCharaterID == movement.npcID)
                {
                    npc = obj;
                    break;
                }
            }

            if (npc == null)
            {
                Debug.LogError($"❌ NPC {movement.npcID}를 찾을 수 없음!");
                continue;
            }
            if(movement!=null)
                RunCoroutineAsTask(MoveAndLookAtTarget(npc, movement));
            yield return null;
        }
    }
    private IEnumerator MoveAndLookAtTarget(GameObject objnpc, NPCMovement movement)
    {
        Animator animator = objnpc.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(movement.animation))
        {
            animator.SetBool(movement.animation, true);
        }


        while (Vector3.Distance(objnpc.transform.position, movement.targetPosition) > 0.1f)
        {
            

            // 목표를 향해 천천히 회전
            Vector3 direction = (movement.targetPosition - objnpc.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            objnpc.transform.rotation = Quaternion.Slerp(objnpc.transform.rotation, lookRotation, Time.deltaTime * 2.0f);

            // 목표를 향해 이동
            objnpc.transform.position = Vector3.MoveTowards(objnpc.transform.position, movement.targetPosition, 2.0f * Time.deltaTime);

            yield return null;  // 다음 프레임까지 대기
        }
        objnpc.transform.position = movement.targetPosition;
        if (animator != null && !string.IsNullOrEmpty(movement.animation))
        {
            animator.SetBool(movement.animation, false);
        }

        Quaternion targetRotation = Quaternion.Euler(objnpc.transform.eulerAngles.x, objnpc.transform.eulerAngles.y - 90f, objnpc.transform.eulerAngles.z);
        objnpc.transform.position = movement.targetPosition;
        while (Quaternion.Angle(objnpc.transform.rotation, targetRotation) > 0.1f)
        {
            objnpc.transform.rotation = Quaternion.Slerp(objnpc.transform.rotation, targetRotation, Time.deltaTime * 2.0f);

            yield return null;  // 다음 프레임까지 대기
        }
    }

}