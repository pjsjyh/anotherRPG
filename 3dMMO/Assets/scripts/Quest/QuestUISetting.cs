using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using CharacterInfo;
using System.Threading.Tasks;
using Questsetting;

public class QuestUISetting : MonoBehaviour
{
    public static QuestUISetting Instance = null;

    public GameObject popupQuest;
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI questReward;
    public Button questOKBtn;
    public GameObject questParnet;
    private QuesetServer qs;

    public GameObject popupQuestClear;
    public TextMeshProUGUI questClearTitle;
    public TextMeshProUGUI questClearReward;
    public Button questClearOKBtn;

    private string nowOpenQuestID;
    private string[] npclist;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            qs = GetComponent<QuesetServer>();
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정

        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 파괴
        }
    }

    public void GetQuestByID(string questID, bool story)
    {
        qs.GetQuestByID(questID, story);
    }

    public void SettingQuestUI()
    {
        Quest q = QuestManager.Instance.nowquest;
        nowOpenQuestID = q.quest_id;
        popupQuest.SetActive(true);
        questTitle.text = q.name;
        questDescription.text = q.description;
        questReward.text = q.reward;
        questClearTitle.text = q.name;
        questClearReward.text = q.reward;
    }

    public void ClickQuest()
    {
        QuestManager.Instance.AddQuest();
        QuestManager.Instance.NpcSetting();
        popupQuest.SetActive(false);
        MakeQuestUI();
        //CharacterManager.Instance.AddQuest(questTitle, questType, requiredAmount);

    }
   public void OnQuestClearBtn()
    {
        popupQuestClear.SetActive(true);
    }
    public void ClickQuesteClearAccept()
    {
        QuestManager.Instance.QuestClearBtnClick();
        popupQuestClear.SetActive(false);
        CharacterManager playerManager = PlayerManager.Instance.GetMyCharacterData();
        Debug.Log(playerManager.characterPersonalinfo.nextstory_name);
        if(playerManager.characterPersonalinfo.nextstory_npc_id!=null)
            NPCManager.Instance.SetNPCState(playerManager.characterPersonalinfo.nextstory_npc_id, npcState.mainquest);
        playerManager.characterPersonalinfo.nextstory_npc_id = null;

    }
    public void MakeQuestUI()
    {
        questParnet.GetComponent<AddQuestUI>().makeQuestUI();

    }
}