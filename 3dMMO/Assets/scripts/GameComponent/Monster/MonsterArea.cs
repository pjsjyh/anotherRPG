using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;

public class MonsterArea : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider colid;
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
            colid.enabled = true;
            Invoke(nameof(TurnOffAttack), 0.4f);
        }
    }

    void TurnOffAttack()
    {
        colid.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator playerAnim = other.GetComponent<Animator>();
            var character = CharacterManager.Instance.myCharacter;
            if (character._hp > 0)
                StartCoroutine(GetHit(playerAnim, other));

            colid.enabled = false;
        }
        else
        {
            TurnOffAttack();
        }
    }
    IEnumerator GetHit(Animator playerAnim, Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        var character = other.transform.parent.GetComponent<Player>();
        var monsterDMG = transform.parent.parent.GetComponent<Monster>();
        character.TakeDamage(monsterDMG.attack);
        //player로 넘겨 데미지 차감 및 애니메이션 실행
    }
}
