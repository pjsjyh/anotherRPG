using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterInfo;
using System.Threading.Tasks;

public class CharacterUISetting : MonoBehaviour
{
    // Start is called before the first frame update
    // 싱글톤 인스턴스
    public static CharacterUISetting Instance { get; private set; }


    public GameObject gamePanel;
    public GameObject player;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerMpText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI playerCoinText;
    public RectTransform playerHealthBar;
    public RectTransform playerMpBar;
    CharacterManager myPlayer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
        Debug.Log("thisisgame");
       
    }
    private void Start()
    {
        GameManager.Instance.OnPlayerDataReady += () =>
        {
            Debug.Log("🎉 UI 셋팅 시작!");
            myPlayer = PlayerManager.Instance.GetMyCharacterData();
            Debug.Log(myPlayer);
            if (myPlayer._username == "")
            {
                myPlayer.ManagerSetting();
            }
            player = myPlayer.playerObj;
        };
      
    }

    void LateUpdate()
    {

        // playerNameText.text = CharacterManager.Instance._username;
       
        if (player != null)
        {
            playerHealthText.text = myPlayer.myCharacter._hp + " / " + "100";
            playerLevelText.text = myPlayer.myCharacter._level.ToString();
            playerCoinText.text = string.Format("{0:n0}", myPlayer.myCharacter._money);
            if ((float)myPlayer.myCharacter._hp / 100 >= 0)
                playerHealthBar.localScale = new Vector3((float)myPlayer.myCharacter._hp / 100, 1, 1);
        }

    }
    [RuntimeInitializeOnLoadMethod]
    static void InitQuitHandler()
    {
        Application.wantsToQuit += () =>
        {
            Debug.Log("🔒 종료 전 동기 저장 처리 중...");
            GameDataManager.Instance.GetSignal();
            return true;
        };
    }

    private void OnApplicationQuit()
    {
        Debug.Log("🚀 게임 종료 감지됨! 데이터 저장 중...");
        GameDataManager.Instance.GetLastSignal();
    }




}
