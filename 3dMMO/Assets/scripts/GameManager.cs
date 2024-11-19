using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterInfo;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }


    public GameObject gamePanel;
    public GameObject player;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerMpText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI playerCoinText;
    public RectTransform playerHealthBar;
    public RectTransform playerMpBar;
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
        if (CharacterManager.Instance._username == "")
        {
            ChaInfoOther managerInfo = new ChaInfoOther
            {
                 _attack=9999,
                _defense=9999,
                _critical=9999,
                _speed=100,
                _luck=9999,
                _gem=0
            };

            CharacterManager.Instance.InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999);
        }
    }

    void LateUpdate()
    {

        playerHealthText.text = CharacterManager.Instance.myCharacter._hp + " / " + "100";
        playerLevelText.text = CharacterManager.Instance.myCharacter._level.ToString();
        // playerNameText.text = CharacterManager.Instance._username;
        player.transform.Find("name/NameText").GetComponent<TextMeshProUGUI>().text = CharacterManager.Instance._username;
        playerCoinText.text = string.Format("{0:n0}", CharacterManager.Instance.myCharacter._money);
        if (player != null)
        {
            if ((float)CharacterManager.Instance.myCharacter._hp / 100 >= 0)
                playerHealthBar.localScale = new Vector3((float)CharacterManager.Instance.myCharacter._hp / 100, 1, 1);
        }

    }

}
