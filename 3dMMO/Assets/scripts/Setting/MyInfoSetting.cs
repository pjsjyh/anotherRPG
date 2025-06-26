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
        var statBindings = new (ReactiveProperty<int> stat, TextMeshProUGUI text)[]
        {
            (myPlayer.myCharacterOther._attack.Value,   statsTexts[0]),
            (myPlayer.myCharacterOther._defense.Value,  statsTexts[1]),
            (myPlayer.myCharacter._hp.Value,            statsTexts[2]),
            (myPlayer.myCharacter._mp.Value,            statsTexts[3]),
            (myPlayer.myCharacterOther._critical.Value, statsTexts[4]),
            (myPlayer.myCharacterOther._speed.Value,    statsTexts[5]),
            (myPlayer.myCharacterOther._luck.Value,     statsTexts[6]),
            (myPlayer.myCharacter._money.Value,         statsTexts[7]),
            (myPlayer.myCharacterOther._gem.Value,      statsTexts[8]),
        };

        foreach (var (stat, text) in statBindings)
        {
            stat.Subscribe(value => text.text = value.ToString()).AddTo(disposables);
        }
        nameText.text = myPlayer._username;
        myPlayer.myCharacter._level.Value.Subscribe(value => levelText.text = "Lv." + value.ToString()).AddTo(disposables);
    }
    private void OnDisable()
    {
        disposables.Clear(); // 바인딩 해제
    }
}
