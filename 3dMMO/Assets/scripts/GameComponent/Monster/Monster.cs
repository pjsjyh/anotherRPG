using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public enum MonsterState { Idle, Chase, Attack, Dead }
    private MonsterState currentState = MonsterState.Idle;
    protected MonsterHit monsterHitEffect;

    public int hp;
    public int attack;
    public int defence;
    public int reward;
    public string monster_id;
    public int reattackTime;

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

    public GameObject telegraphPrefab;
    protected virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        monsterAnim = GetComponent<Animator>();
        SetMonsterStats();
        monsterHitEffect = GetComponent<MonsterHit>();
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
        if (currentState == newState) return;
        currentState = newState;
    }

    protected void Idle()
    {
        monsterAnim.SetBool("isFollow", false);

        if (chaseTarget != null && nav.remainingDistance < 3 && currentState != MonsterState.Attack)
        {
            ChangeState(MonsterState.Chase);
        }
    }

    //자식클래스에서 몬스터 기본 셋팅
    protected abstract void SetMonsterStats();
    protected void Attack()
    {
        if (isAttack) return;
        monsterAnim.SetBool("isFollow", false);
        isAttack = true;
        Invoke(nameof(AttackPlayer), 0.3f);
    }
    void AttackPlayer()
    {
        if (!isDead)
        {
            BoxCollider areaCollider = monsterattackArea.colid;

            Vector3 pos = transform.position + transform.forward * 1f; 
            Quaternion rot = Quaternion.LookRotation(transform.forward);
            Vector3 size = Vector3.Scale(monsterattackArea.colid.size, monsterattackArea.colid.transform.lossyScale);
            GameObject telegraph = Instantiate(telegraphPrefab);
            var script = telegraph.GetComponent<AttackArea>();
            script.Initialize(pos, rot, size, () =>
            {
                // 여기서 실제 공격 판정
                monsterattackArea.attackStart = true;
                monsterAnim.SetTrigger("doAttack1");

                //ResetAttack();
                Invoke(nameof(ResetAttack), reattackTime);
            });
        }
        
    }
    private void ResetAttack()
    {
        isAttack = false;
        float distance = Vector3.Distance(transform.position, chaseTarget.position);

        if (chaseTarget == null || distance > detectionRange)
        {
            ChangeState(MonsterState.Idle);
        }
        else
        {
            ChangeState(MonsterState.Chase);
        }
        ChangeState(MonsterState.Chase);
        monsterAnim.SetBool("isFollow", true);

    }
    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            hp -= damage;
            if (hp <= 0)
            {
                monsterAnim.ResetTrigger("doAttack1");
                monsterAnim.ResetTrigger("isFollow");
                MonsterDead();
                var myPlayer = PlayerManager.Instance.GetMyCharacterData();
                myPlayer.myCharacter._money += reward;
            }
            else
            {
                if(monsterHitEffect)
                    monsterHitEffect.PlayHitEffect();
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
            if (other.CompareTag("Player")&&!isAttack)
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
        float dist = Vector3.Distance(transform.position, chaseTarget.position);
        float angle = Vector3.Angle(transform.forward, chaseTarget.position-transform.position);
         if (dist < 2 && !isAttack && currentState != MonsterState.Attack && angle < 3f)
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
        if (isDead) return;
        monsterAnim.ResetTrigger("doAttack1");
        monsterAnim.ResetTrigger("isFollow");
        monsterAnim.ResetTrigger("getHit");

        isDead = true;
        monsterAnim.SetTrigger("doDie");
        Invoke(nameof(DestroyMonster), 2f);
        QuestManager.Instance.OnMonsterKilled(monster_id);
    }
    protected void DestroyMonster()
    {
        Destroy(gameObject);
    }
}
