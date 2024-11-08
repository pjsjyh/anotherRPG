using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CharacterInfo;

public class Player : MonoBehaviour
{
    float vAxis;
    float hAxis;
    float speed = 7;
    float gravity = -9.81f;
    Vector3 moveVec;
    public int _playerHP;
    public int attacknum = -1;
    float jumpSpeed;


    bool wDown;
    bool a1Down, a2Down, a3Down, a4Down;
    bool jDown;

    public bool isAttack = true;

    bool isWalk;
    bool isRun;
    public bool isJump = false;
    bool isDead;
    bool skillclickEvent = false;

    [SerializeField]
    private GameObject player;
    public CharacterController _controller;
    public Weapon weapon;
    //public ChaInfo playerInfo;
    //public ChaInfo playerInfo;

    Camera _camera;
    Animator anim;
    Rigidbody rigid;

    playerSkill playerskilltimer;
    private void Awake()
    {
        //playerInfo = new ChaInfo();
        //createPlayer();

    }
    void Start()
    {
        anim = player.GetComponent<Animator>();
        _camera = Camera.main;
        // _controller = player.GetComponent<CharacterController>();
        _controller = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        playerskilltimer = GameObject.Find("SkillGroup").GetComponent<playerSkill>();
    }


    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        CheckDie();
        // if (_controller.isGrounded == false && !isJump)
        // {
        //     moveVec.y += gravity * Time.deltaTime;
        // }
        if (!isDead)
            _controller.Move(moveVec * speed * Time.deltaTime);
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        a1Down = Input.GetButtonDown("Attack1");
        a2Down = Input.GetButtonDown("Attack2");
        a3Down = Input.GetButtonDown("Attack3");
        a4Down = Input.GetButtonDown("Attack4");
    }

    void Move()
    {
        if (isDead)
        {
            moveVec = Vector3.zero;
            return;
        }
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        moveVec = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal")).normalized;


        anim.SetBool("IsWalk", moveVec != Vector3.zero);
        anim.SetBool("IsRun", wDown);
    }
    void Turn()
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
    void Jump()
    {
        if (jDown && !isJump && !isDead)
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

            weapon.Use(attacknum);
            playerskilltimer.UseSkill(attacknum);

            switch (attacknum)
            {
                case 1:
                    anim.SetTrigger("doAttack" + attacknum);
                    break;
                case 2:
                    anim.SetTrigger("doAttack" + attacknum);
                    break;
                case 3:
                    anim.SetTrigger("doAttack" + attacknum);
                    break;
                case 4:
                    anim.SetTrigger("doAttack" + attacknum);
                    break;
            }
            //AttackAnim(attacknum);

            isAttack = false;
            Invoke("AttackOut", 0.4f);
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
    void CheckDie()
    {
        if (CharacterManager.Instance.myCharacter._hp <= 0 && !isDead)
            OnDie();
    }
    void AttackOut()
    {
        isAttack = true;
    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    private void FixedUpdate()
    {
        FreezeRotation();
    }
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
    }

}
