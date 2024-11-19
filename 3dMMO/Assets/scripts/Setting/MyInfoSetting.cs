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
        statsTexts[0].text = CharacterManager.Instance.myCharacterOther._attack.ToString();
        statsTexts[1].text = CharacterManager.Instance.myCharacterOther._defense.ToString();
        statsTexts[2].text = CharacterManager.Instance.myCharacter._hp.ToString();
        statsTexts[3].text = CharacterManager.Instance.myCharacter._mp.ToString();
        statsTexts[4].text = CharacterManager.Instance.myCharacterOther._critical.ToString();
        statsTexts[5].text = CharacterManager.Instance.myCharacterOther._speed.ToString();
        statsTexts[6].text = CharacterManager.Instance.myCharacterOther._luck.ToString();
        statsTexts[7].text = CharacterManager.Instance.myCharacter._money.ToString();
        statsTexts[8].text = CharacterManager.Instance.myCharacterOther._gem.ToString();
        nameText.text = CharacterManager.Instance._username;
        levelText.text = "Lv."+CharacterManager.Instance.myCharacter._level.ToString();
    }

}
