using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterInfo;
using System.Threading.Tasks;
using UniRx;

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

    private CompositeDisposable disposables = new CompositeDisposable();
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
       
    }
    private void Start()
    {
        GameManager.Instance.OnPlayerDataReady += () =>
        {
            Debug.Log("UI 셋팅 시작!");
            myPlayer = PlayerManager.Instance.GetMyCharacterData();
            Debug.Log(myPlayer);
            if (myPlayer._username == "")
            {
                myPlayer.ManagerSetting();
            }
            player = myPlayer.playerObj;
            disposables.Clear();

            // 자동 UI 바인딩 시작
            BindUI();
        };
      
    }

   
    private void BindUI()
    {
        // HP 텍스트와 HP 바
        myPlayer.myCharacter._hp.Value
            .Subscribe(hp =>
            {
                playerHealthText.text = $"{hp} / 100";
                float percent = Mathf.Clamp01((float)hp / 100f);
                playerHealthBar.localScale = new Vector3(percent, 1f, 1f);
            }).AddTo(disposables);

        // 레벨
        myPlayer.myCharacter._level.Value
            .Subscribe(level =>
            {
                playerLevelText.text = level.ToString();
            }).AddTo(disposables);

        // 코인
        myPlayer.myCharacter._money.Value
            .Subscribe(money =>
            {
                playerCoinText.text = string.Format("{0:n0}", money);
            }).AddTo(disposables);
    }
    [RuntimeInitializeOnLoadMethod]
    static void InitQuitHandler()
    {
        Application.wantsToQuit += () =>
        {
            Debug.Log("종료 전 동기 저장 처리 중...");
            GameDataManager.Instance.GetSignal();
            return true;
        };
    }

    private void OnApplicationQuit()
    {
        Debug.Log("게임 종료 감지됨! 데이터 저장 중...");
        GameDataManager.Instance.GetLastSignal();
    }




}
