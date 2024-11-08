using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Sword, Defence
    };
    public Type type;
    public BoxCollider meleeArea;

    public int damage;
    // Start is called before the first frame update
    public int getAttacknum;
    public void Use(int attacknum)
    {
        getAttacknum = attacknum;
        if (type == Type.Sword)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.07f);
        meleeArea.enabled = false;
    }
}
