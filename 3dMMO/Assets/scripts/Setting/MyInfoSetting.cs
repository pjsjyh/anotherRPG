using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterInfo;
public class MyInfoSetting : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] statsTexts; // Inspector에서 TextMeshProUGUI 배열 관리
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]

    private TextMeshProUGUI levelText;

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
    public void openPanel()
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();
        statsTexts[0].text = myPlayer.myCharacterOther._attack.ToString();
        statsTexts[1].text = myPlayer.myCharacterOther._defense.ToString();
        statsTexts[2].text = myPlayer.myCharacter._hp.ToString();
        statsTexts[3].text = myPlayer.myCharacter._mp.ToString();
        statsTexts[4].text = myPlayer.myCharacterOther._critical.ToString();
        statsTexts[5].text = myPlayer.myCharacterOther._speed.ToString();
        statsTexts[6].text = myPlayer.myCharacterOther._luck.ToString();
        statsTexts[7].text = myPlayer.myCharacter._money.ToString();
        statsTexts[8].text = myPlayer.myCharacterOther._gem.ToString();
        nameText.text = myPlayer._username;
        levelText.text = "Lv."+ myPlayer.myCharacter._level.ToString();
    }

}
