using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class playerSkill : MonoBehaviour
{
    public Image skillFilter1;
    public TextMeshProUGUI coolTimeCounter1;
    public Image skillFilter2;
    public TextMeshProUGUI coolTimeCounter2;
    public Image skillFilter3;
    public TextMeshProUGUI coolTimeCounter3;
    public Image skillFilter4;
    public TextMeshProUGUI coolTimeCounter4;

    public float coolTime, coolTime4;
    public bool skill1cool, skill2cool, skill3cool, skill4cool = true;
    private float currentCoolTime, currentCoolTime4; //남은 쿨타임을 추적 할 변수

    public bool canUseSkill = true, canUseSkill4 = true; //스킬을 사용할 수 있는지 확인하는 변수
    Player player;
    void start()
    {
        Debug.Log("시작");
        skillFilter1.fillAmount = 0; //처음에 스킬 버튼을 가리지 않음
        skillFilter2.fillAmount = 0;
        skillFilter3.fillAmount = 0;
        skillFilter4.fillAmount = 0;
    }

    public void UseSkill(int attacknum)
    {

        if (attacknum == 1)
        {
            if (skill1cool)
            {
                SkillCool(attacknum);
                skill1cool = false;
            }
        }
        else if (attacknum == 2)
        {
            if (skill2cool)
            {
                SkillCool(attacknum);
                skill2cool = false;
            }
        }
        else if (attacknum == 3)
        {
            if (skill3cool)
            {
                SkillCool(attacknum);
                skill3cool = false;
            }
        }
        else if (attacknum == 4)
        {
            if (skill4cool)
            {
                SkillCool(attacknum);
                skill4cool = false;
            }
        }
    }

    void SkillCool(int attacknum)
    {
        switch (attacknum)
        {
            case 1:
                skillFilter1.fillAmount = 1;
                currentCoolTime = coolTime;
                coolTimeCounter1.text = "" + currentCoolTime;
                canUseSkill = false;
                break;
            case 2:
                skillFilter2.fillAmount = 1;
                currentCoolTime = coolTime;
                coolTimeCounter2.text = "" + currentCoolTime;
                canUseSkill = false;
                break;
            case 3:
                skillFilter3.fillAmount = 1;
                currentCoolTime = coolTime;
                coolTimeCounter3.text = "" + currentCoolTime;
                canUseSkill = false;
                break;
            case 4:
                skillFilter4.fillAmount = 1;
                currentCoolTime4 = coolTime4;
                coolTimeCounter4.text = "" + currentCoolTime4;
                canUseSkill4 = false;
                break;
        }

        StartCoroutine("Cooltime", attacknum);


        StartCoroutine("CoolTimeCounter", attacknum);

        //스킬을 사용하면 사용할 수 없는 상태로 바꿈
    }
    IEnumerator Cooltime(int num)
    {
        switch (num)
        {
            case 1:
                while (skillFilter1.fillAmount > 0)
                {
                    skillFilter1.fillAmount -= 1 * Time.smoothDeltaTime / coolTime;

                    yield return null;
                }
                break;
            case 2:
                while (skillFilter2.fillAmount > 0)
                {
                    skillFilter2.fillAmount -= 1 * Time.smoothDeltaTime / coolTime;

                    yield return null;
                }
                break;
            case 3:
                while (skillFilter3.fillAmount > 0)
                {
                    skillFilter3.fillAmount -= 1 * Time.smoothDeltaTime / coolTime;

                    yield return null;
                }
                break;
            case 4:
                while (skillFilter4.fillAmount > 0)
                {
                    skillFilter4.fillAmount -= 1 * Time.smoothDeltaTime / coolTime4;

                    yield return null;
                }
                break;
        }
        switch (num)
        {
            case 1:
                skill1cool = true;
                canUseSkill = true;
                break;
            case 2:
                skill2cool = true;
                canUseSkill = true;
                break;
            case 3:
                skill3cool = true;
                canUseSkill = true;
                break;
            case 4:
                skill4cool = true;
                canUseSkill4 = true;
                break;
        }
        //스킬 쿨타임이 끝나면 스킬을 사용할 수 있는 상태로 바꿈

        yield break;
    }

    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator CoolTimeCounter(int num)
    {
        switch (num)
        {
            case 1:
                while (currentCoolTime > 0)
                {
                    yield return new WaitForSeconds(1.0f);

                    currentCoolTime -= 1.0f;
                    coolTimeCounter1.text = "" + currentCoolTime;
                }

                if (currentCoolTime <= 0)
                    coolTimeCounter1.text = "";
                break;
            case 2:
                while (currentCoolTime > 0)
                {
                    yield return new WaitForSeconds(1.0f);

                    currentCoolTime -= 1.0f;
                    coolTimeCounter2.text = "" + currentCoolTime;
                }

                if (currentCoolTime <= 0)
                    coolTimeCounter2.text = "";
                break;
            case 3:
                while (currentCoolTime > 0)
                {
                    yield return new WaitForSeconds(1.0f);

                    currentCoolTime -= 1.0f;
                    coolTimeCounter3.text = "" + currentCoolTime;
                }

                if (currentCoolTime <= 0)
                    coolTimeCounter3.text = "";
                break;
            case 4:
                while (currentCoolTime4 > 0)
                {
                    yield return new WaitForSeconds(1.0f);

                    currentCoolTime4 -= 1.0f;
                    coolTimeCounter4.text = "" + currentCoolTime4;
                }

                if (currentCoolTime4 <= 0)
                    coolTimeCounter4.text = "";
                break;
        }

        yield break;
    }

}
