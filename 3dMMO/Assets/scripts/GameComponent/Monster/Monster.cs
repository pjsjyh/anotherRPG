using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public int hp;
    public int attack;
    public int defence;

    protected bool ischasePlayer = false;
    protected bool isDead = false;
    protected bool isAttack = true;
    protected  Transform chaseTarget;
    protected  Animator monsterAnim;
   // protected  GameObject chaseTarget;
    public MonsterArea monsterattackArea;

    public Collider meleeArea;
    public NavMeshAgent nav;

    protected virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        monsterAnim = GetComponent<Animator>();
        SetMonsterStats();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (ischasePlayer && !isDead)
        {
            ChaseTarget();
            monsterAnim.SetBool("isFollow", true);
            if (nav.remainingDistance < 3 && isAttack)
            {
                isAttack = false;
                Invoke(nameof(AttackPlayer), 4);

            }

            if (nav.remainingDistance > 10 || isDead)
            {
                StopChase();
            }
        }
    }
    //자식클래스에서 몬스터 기본 셋팅
    protected abstract void SetMonsterStats();
    void AttackPlayer()
    {
        if (!isDead)
        {
            monsterattackArea.attackStart = true;
            monsterAnim.SetTrigger("doAttack1");
            isAttack = true;

        }
    }
    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            hp -= damage;

            if (hp <= 0)
            {
                MonsterDead();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.CompareTag("Player"))
            {
                StartChase(other.transform);
            }
            if (other.CompareTag("WeaponSword"))
            {
                //무기 종류에 따른 데미지
                Weapon weapon = other.GetComponent<Weapon>();
                TakeDamage(weapon.getAttacknum * 10);
                weapon.meleeArea.enabled = false;
            }

        }

    }
    protected  void Targeting()
    {
        Vector3 dir = chaseTarget.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

    }
    protected  void ChaseTarget()
    {
        nav.SetDestination(chaseTarget.position);
        Targeting();
    }
    protected void StartChase(Transform player)
    {
        chaseTarget = player;
        ischasePlayer = true;
        nav.enabled = true;
    }
    protected void StopChase()
    {
        ischasePlayer = false;
        monsterAnim.SetBool("isFollow", false);
        nav.enabled = false;
    }
    protected virtual void MonsterDead()
    {
        isDead = true;
        monsterAnim.SetTrigger("doDie");
        Invoke(nameof(DestroyMonster), 2f);
    }
    protected void DestroyMonster()
    {
        Destroy(gameObject);
    }
}
