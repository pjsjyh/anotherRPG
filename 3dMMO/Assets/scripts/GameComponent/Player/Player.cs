using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CharacterInfo;
using TMPro;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Weapon weapon;
    [SerializeField] private playerSkill playerskilltimer;
    [SerializeField] private GameObject trailTip;
    [SerializeField] private NavMeshAgent navAgent;

    private Vector3 moveVec;
    public int attacknum = -1;
    private  float speed = 7;
    private  float gravity = -9.81f;
    private  float jumpSpeed;

    bool a1Down, a2Down, a3Down, a4Down;

    public bool isAttack = true;
    private float hAxis, vAxis;
    private bool wDown, jDown;
    public bool isJump = false;
    private bool isDead = false;
    private bool skillclickEvent = false;
    private bool isMoveNavAuto = false;
    public float stopDistance = 0.5f;
    private Vector3 sestNavTarget;


    private  Camera _camera;
    [SerializeField]
    private  Animator anim;
    private  Rigidbody rigid;

    CharacterManager myPlayer;
    PlayerControll myController;
    private void Awake()
    {

        //var charInfo = CharacterManager.Instance.characterPersonalinfo;
        GameManager.Instance.OnPlayerDataReady += () =>
        {
            myPlayer = PlayerManager.Instance.GetMyCharacterData();
            var charInfo = myPlayer.characterPersonalinfo;
            transform.position = new Vector3(charInfo.chaPosition[0], charInfo.chaPosition[1], charInfo.chaPosition[2]);
            transform.rotation = Quaternion.Euler(charInfo.chaRotation[0], charInfo.chaRotation[1], charInfo.chaRotation[2]);

            this.name = myPlayer._username;
            this.transform.Find("name/NameText").GetComponent<TextMeshProUGUI>().text = myPlayer._username;
            GameStartFade.TurnOff();
        };
    }
    private void Start()
    {
        
        if (player == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        else
        {
            anim = player.GetComponent<Animator>();
        }
        _camera = Camera.main;
        _controller = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        playerskilltimer = GameObject.Find("SkillGroup").GetComponent<playerSkill>();
        //this.name = CharacterManager.Instance._username;
        
        // this.transform.Find("name/NameText").GetComponent<TextMeshProUGUI>().text = CharacterManager.Instance._username;
    }


    //private void Update()
    //{
    //    GetInput();

    //    if (!isDead)
    //    {
    //        Move();
    //        Turn();
    //        Jump();
    //        Attack();
    //        _controller.Move(moveVec * speed * Time.deltaTime);

    //    }
    //}

    public void CustomUpdate()
    {
        GetInput();
        if (!isDead)
        {
            if (isMoveNavAuto)
            {
                checkNavMove();
            }
            else
            {
                Move();
                Turn();
                Jump();
                Attack();
                _controller.Move(moveVec * speed * Time.deltaTime);
            }
        }
    }
    private void FixedUpdate()
    {
        if (rigid != null && !rigid.isKinematic)
        {
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void LateUpdate()
    {
        SavePositionRotation();
    }
    public void SavePositionRotation()
    {
        // var charInfo = CharacterManager.Instance.characterPersonalinfo;
        if (myPlayer==null) return;
        var charInfo = myPlayer.characterPersonalinfo;
        charInfo.chaPosition = new float[] { transform.position.x, transform.position.y, transform.position.z };
        charInfo.chaRotation = new float[] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z };

    }
    private  void GetInput()
    {
        wDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        a1Down = Input.GetButtonDown("Attack1");
        a2Down = Input.GetButtonDown("Attack2");
        a3Down = Input.GetButtonDown("Attack3");
        a4Down = Input.GetButtonDown("Attack4");
        if (isMoveNavAuto && (hAxis != 0 || vAxis != 0))
        {
            isMoveNavAuto = false;
            anim.SetBool("IsRun", false);
        }
    }

    private void Move()
    {
        //if (isDead)
        //{
        //    moveVec = Vector3.zero;
        //    return;
        //}
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        moveVec = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal")).normalized;

        anim.SetBool("IsWalk", moveVec != Vector3.zero);
        anim.SetBool("IsRun", wDown);
    }
    private void Turn()
    {

        Vector3 dir = moveVec;
        if (dir.x != 0 || dir.y != 0 || dir.z != 0 && !isDead)
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);

        if (Input.GetMouseButton(0) && !isDead)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * 10f);
        }
    }
    private void Jump()
    {
        if (jDown && !isJump)
        {
            jumpSpeed = 3f;  // 점프 힘을 설정
            anim.SetTrigger("doJump");
            isJump = true;
        }

        // 중력 적용
        if (!_controller.isGrounded)
        {
            jumpSpeed += gravity * Time.deltaTime;  // 중력을 점진적으로 더해줌
        }
        else if (_controller.isGrounded && jumpSpeed < 0)
        {
            jumpSpeed = 0;  // 착지 시 속도를 0으로
            isJump = false;     // 착지 상태로 전환
        }

        moveVec.y = jumpSpeed;  // 부드러운 점프
    }
    void Attack()
    {
        if (!isAttack || isDead)
            return;
        if (a1Down || a2Down || a3Down || a4Down || skillclickEvent)
        {
            if (!skillclickEvent)
            {
                if (playerskilltimer.canUseSkill)
                {
                    if (a1Down)
                        attacknum = 1;
                    if (a2Down)
                        attacknum = 2;
                    if (a3Down)
                        attacknum = 3;
                }

                if (a4Down && playerskilltimer.canUseSkill4)
                    attacknum = 4;
            }
            else
            {
                if (attacknum != 4 && playerskilltimer.canUseSkill == false)
                    return;
                if (attacknum == 4 && playerskilltimer.canUseSkill4 == false)
                    return;
            }
            StartAttack();
            weapon.Use(attacknum);
            playerskilltimer.UseSkill(attacknum);
            anim.SetTrigger("doAttack" + attacknum);

            isAttack = false;
            Invoke(nameof(AttackOut), 0.4f);
        }
        skillclickEvent = false;
        attacknum = -1;
    }
    public void ClickSkillBtn(int atk)
    {
        GameObject clickobj = EventSystem.current.currentSelectedGameObject;
        if (clickobj != null)
        {
            skillclickEvent = true;
            attacknum = atk;
        }
    }

    private void AttackOut()
    {
        isAttack = true;
        EndAttack();
    }


    private void OnDie()
    {
        if (!isDead)
        {
            anim.SetTrigger("doDie");
            isDead = true;
            gameObject.transform.GetChild(0).tag = "DiePlayer";
        }
    }
    public void TakeDamage(int damage)
    {
        myPlayer.TakeDamage(damage);
        if (myPlayer.GetHp() <= 0) OnDie();
        else anim.SetTrigger("doGetHit");
    }
    void StartAttack()
    {
        var trail = trailTip.GetComponent<TrailRenderer>();
        trail.Clear();
        trail.emitting = true;
    }

    void EndAttack()
    {
        trailTip.GetComponent<TrailRenderer>().emitting = false;
    }

    public void GoTarget(Vector3 getTarget)
    {
        sestNavTarget = getTarget;
        navAgent.isStopped = false;
        isMoveNavAuto = true;
        navAgent.SetDestination(getTarget);
        navAgent.updatePosition = true;
        navAgent.updateRotation = true;
        anim.applyRootMotion = false;

        if (!navAgent.isOnNavMesh)
        {
            Debug.LogError("❌ NavMeshAgent가 NavMesh 위에 있지 않습니다!");
        }
       
    }

    private void checkNavMove()
    {
        float distance = Vector3.Distance(transform.position, sestNavTarget);
        anim.SetBool("IsRun", true);

        if (distance < stopDistance)
        {
            navAgent.isStopped = true;
            isMoveNavAuto = false;
            anim.SetBool("IsRun", false);
        }
        else
        {
            navAgent.SetDestination(sestNavTarget);
            if (navAgent.hasPath)
            {
                Vector3 direction = sestNavTarget - transform.position;
                direction.y = 0; // 수직 방향 제거
                direction.Normalize();
                if (direction != Vector3.zero)
                {
                    // 한 번에 방향 전환
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }
}
