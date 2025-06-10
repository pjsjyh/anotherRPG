using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterInfo;
using UniRx;
public class MyInfoSetting : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] statsTexts; // Inspector에서 TextMeshProUGUI 배열 관리
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]

    private TextMeshProUGUI levelText;
    private CompositeDisposable disposables = new CompositeDisposable();
    private enum Stats
    {
        Attack,
        Defense,
        HP,
        MP,
        Critical,
        Speed,
        Luck,
        Gold,
        Gem
    }
    private void OnEnable()
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();

        // 기본 능력치 바인딩
        myPlayer.myCharacterOther._attack.Subscribe(value => statsTexts[0].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacterOther._defense.Subscribe(value => statsTexts[1].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacter._hp.Subscribe(value => statsTexts[2].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacter._mp.Subscribe(value => statsTexts[3].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacterOther._critical.Subscribe(value => statsTexts[4].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacterOther._speed.Subscribe(value => statsTexts[5].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacterOther._luck.Subscribe(value => statsTexts[6].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacter._money.Subscribe(value => statsTexts[7].text = value.ToString()).AddTo(disposables);
        myPlayer.myCharacterOther._gem.Subscribe(value => statsTexts[8].text = value.ToString()).AddTo(disposables);

        nameText.text = myPlayer._username;
        myPlayer.myCharacter._level.Subscribe(value => levelText.text = "Lv." + value.ToString()).AddTo(disposables);
    }
    private void OnDisable()
    {
        disposables.Clear(); // 바인딩 해제
    }
}
