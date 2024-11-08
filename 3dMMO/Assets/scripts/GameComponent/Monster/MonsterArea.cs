using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;

public class MonsterArea : Monster
{
    // Start is called before the first frame update
    Collider colid;
    public bool attackStart = false;
    void Start()
    {
        colid = gameObject.GetComponent<Collider>();
    }
    void Update()
    {
        if (attackStart)
        {
            attackStart = false;
            // Debug.Log("공격켜짐");
            colid.enabled = true;
            Invoke("TurnOffAttack", 0.4f);
        }
    }

    void TurnOffAttack()
    {
        colid.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("monster     " + other.gameObject.name);

            Player player = other.GetComponentInParent<Player>();
            Animator playeranim = other.GetComponent<Animator>();
            CharacterManager.Instance.myCharacter._hp -= 20;
            if (CharacterManager.Instance.myCharacter._hp > 0)
                StartCoroutine(getHit(playeranim));
            colid.enabled = false;
        }
    }
    IEnumerator getHit(Animator playeranim)
    {
        yield return new WaitForSeconds(0.2f);
        playeranim.SetTrigger("doGetHit");
    }
}
