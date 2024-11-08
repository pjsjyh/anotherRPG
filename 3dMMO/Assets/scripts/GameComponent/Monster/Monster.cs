using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonsterInfo
{
    // Start is called before the first frame update
    public enum Type
    {
        slime, blueSlime
    };
    public Type monType;
    public ChaInfo monInfo;

    bool ischasePlayer = false;
    bool isDead = false;
    bool isAttack = true;
    Transform toChase;
    Animator monsterAnim;
    GameObject chaseTarget;
    public MonsterArea monsterattackArea;

    public Collider meleeArea;
    public NavMeshAgent nav;

    void Start()
    {
        //nav = GetComponent<NavMeshAgent>();
        monsterAnim = GetComponent<Animator>();
        createMon();
    }

    // Update is called once per frame
    void Update()
    {
        if (ischasePlayer && !isDead)
        {
            chase();
            monsterAnim.SetBool("isFollow", true);
            if (nav.remainingDistance < 3 && isAttack)
            {
                isAttack = false;
                Invoke("AttackPlayer", 4);
            }

            if (nav.remainingDistance > 10 || isDead)
            {
                ischasePlayer = false;
                monsterAnim.SetBool("isFollow", false);
                nav.enabled = false;
            }
        }
    }

    void createMon()
    {
        switch (monType)
        {
            case Type.slime:
                monInfo._hp = 100;
                monInfo._attack = 30;
                monInfo._defence = 10;
                break;
            case Type.blueSlime:
                monInfo._hp = 150;
                monInfo._attack = 50;
                monInfo._defence = 30;
                break;
        }
    }

    void AttackPlayer()
    {
        if (!isDead)
        {
            monsterattackArea.attackStart = true;
            monsterAnim.SetTrigger("doAttack1");
            isAttack = true;

        }
    }
    IEnumerator damage()
    {
        yield return new WaitForSeconds(2);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            nav.enabled = true;
            ischasePlayer = true;
            toChase = other.gameObject.transform;
            chaseTarget = other.gameObject;
            if (other.gameObject.tag == "WeaponSword")
            {
                Weapon weapon = other.GetComponent<Weapon>();
                monInfo._hp -= weapon.getAttacknum * 10;
                Debug.Log(monInfo._hp);
                weapon.meleeArea.enabled = false;
            }
            if (monInfo._hp <= 0)
                monsterDead();

        }

    }
    void Targeting()
    {
        Vector3 dir = chaseTarget.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

    }
    void chase()
    {
        nav.SetDestination(toChase.position);
        Targeting();
    }
    void monsterDead()
    {
        isDead = true;
        monsterAnim.SetTrigger("doDie");
        Invoke("destroyMon", 2f);
    }
    void destroyMon()
    {
        Destroy(gameObject);
    }
}
