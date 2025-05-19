using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSlime : Monster
{
    
    protected override void SetMonsterStats()
    {
        hp = 100;
        attack = 10;
        defence = 10;
        reward = 100;
        reattackTime = 1;
    }
}
