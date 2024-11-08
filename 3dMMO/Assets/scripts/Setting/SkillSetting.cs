using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Skill
{
    string skill_name;
    string skill_id;
    string skill_type;
    string skill_discription;
}
public class SkillSetting
{
    public List<Skill> skillList = new List<Skill>();

    public void InitialSkill()
    {

    }
}
