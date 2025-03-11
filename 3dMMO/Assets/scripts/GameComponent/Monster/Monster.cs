using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public enum MonsterState { Idle, Chase, Attack, Dead }
    private MonsterState currentState = MonsterState.Idle;


    public int hp;
    public int attack;
    public int defence;

    protected bool ischasePlayer = false;
    protected bool isDead = false;
    protected bool isAttack = false;
    protected  Transform chaseTarget;
    protected  Animator monsterAnim;
   // protected  GameObject chaseTarget;
    public MonsterArea monsterattackArea;

    public Collider meleeArea;
    public NavMeshAgent nav;
    public float detectionRange = 10f; // 플레이어 감지 범위
    public float attackRange = 2f;
    protected virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        monsterAnim = GetComponent<Animator>();
        SetMonsterStats();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isDead) return;
        switch (currentState)
        {
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Chase:
                StartChase();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.Dead:
                MonsterDead();
                break;
        }
    }
    protected void ChangeState(MonsterState newState)
    {
        currentState = newState;
    }

    protected void Idle()
    {
        monsterAnim.SetBool("isFollow", false);

        if (chaseTarget != null && nav.remainingDistance < 3)
        {
            ChangeState(MonsterState.Chase);
        }
    }

    //자식클래스에서 몬스터 기본 셋팅
    protected abstract void SetMonsterStats();
    protected void Attack()
    {
        if (isAttack) return;
        isAttack = true;
        Invoke(nameof(AttackPlayer), 2);
        Invoke(nameof(ResetAttack), 2);
    }
    void AttackPlayer()
    {
        if (!isDead)
        {
            monsterattackArea.attackStart = true;
            monsterAnim.SetTrigger("doAttack1");
        }
        
    }
    private void ResetAttack()
    {
        isAttack = false;
        ChangeState(MonsterState.Chase);
    }
    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            Debug.Log("아야");
            hp -= damage;
            if (hp <= 0)
            {
                monsterAnim.ResetTrigger("doAttack1");
                monsterAnim.ResetTrigger("isFollow");
                MonsterDead();
            }
            else
            {
                monsterAnim.ResetTrigger("doAttack1");
                monsterAnim.ResetTrigger("isFollow");

                monsterAnim.SetTrigger("getHit");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.CompareTag("Player"))
            {
                chaseTarget = other.transform;
                ChangeState(MonsterState.Chase);
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
    protected void StartChase()
    {
        nav.enabled = true;

        ChaseTarget();
        monsterAnim.SetBool("isFollow", true);
        ischasePlayer = true;
        if (nav.remainingDistance < 3 && !isAttack)
        {
            ChangeState(MonsterState.Attack);

        }
        if (nav.remainingDistance > 10)
        {
            ChangeState(MonsterState.Idle);
        }
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
