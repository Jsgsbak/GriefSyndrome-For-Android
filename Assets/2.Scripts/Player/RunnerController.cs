using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class RunnerController : MonoBehaviour
{

    //public Variable
    public enum Direction
    {
        Forward = 90,
        Backward = 270,
    }
    public float maxSpeed;
    public float MoveSpeed = 0;
    public float Z;
    public float JumpForce;
    public float Gravity;
    public bool PlayerDead;


    //private variable

    private CharacterController Controller;
    private Animator animator;
    private Direction direction = Direction.Forward;
    private float H;
    private float V;
    private bool OnGround = true;
    private bool Jump;
    private float JSpeed;

    void Awake()
    {
        //rigidbody = this.GetComponent<Rigidbody>();
        Controller = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (!PlayerDead)
        {
            if (this.transform.position.z != Z)
            {
                Vector3 Position = this.transform.position;
                Position.z = Z;
                this.transform.position = Position;
            }
            H = Input.GetAxis("Horizontal");
            V = Input.GetAxis("Vertical");

            //AniamtorState();
            if (Controller.isGrounded)
            {
                OnGround = true;
                Jump = false;
                animator.SetBool("Jump", false);
                if (Input.GetKeyDown(KeyCode.J))
                {
                    if (V > 0.3)
                    {
                        JSpeed = 1.3f * JumpForce;
                    }
                    else
                    {
                        JSpeed = JumpForce;
                    }
                    Jump = true;
                    animator.SetBool("Jump", true);
                }
            }
            else
            {
                OnGround = false;
                if (!OnGround)
                {
                    Controller.Move(-Vector3.up * Gravity * Time.deltaTime);
                }
            }
        }
        else
        {
            animator.SetBool("Dead", true);
            animator.SetFloat("speed", 0);
            animator.SetFloat("Slider", 0);
            animator.SetBool("Jump", false);
        }
    }

    void FixedUpdate()
    {
        JumpUp();
        Move();

    }

    void Move()
    {

        //先计算朝向
        if (H >= 0.3)
        {
            if (MoveSpeed == 0)
            {
                SetFacingDirection(Direction.Forward);

            }
            if (direction == Direction.Forward)
            {
                if (MoveSpeed < maxSpeed)
                {
                    MoveSpeed += 1.0f;
                    //state = State.Walk;
                }
                else
                {
                    MoveSpeed = maxSpeed;
                    //state = State.Run;
                }
            }
        }
        else if (H <= -0.3)
        {
            if (MoveSpeed == 0)
            {
                SetFacingDirection(Direction.Backward);

            }
            if (direction == Direction.Backward)
            {
                if (MoveSpeed < maxSpeed)
                {
                    MoveSpeed += 1.0f;
                    //state = State.Walk;
                }
                else
                {
                    MoveSpeed = maxSpeed;
                    //state = State.Run;
                }
            }
        }
        if ((direction == Direction.Forward && H < -0.3 && MoveSpeed != 0)
           || (direction == Direction.Backward && H > 0.3 && MoveSpeed != 0))
        {
            MoveSpeed -= 1.0f;
            if (MoveSpeed <= 0)
            {
                MoveSpeed = 0;
                //state = State.Idle;
            }
        }
        if (Mathf.Abs(H) < 0.3 && MoveSpeed != 0)
        {
            MoveSpeed -= 0.5f;
            if (MoveSpeed <= 0)
            {
                MoveSpeed = 0;
                //state = State.Run;
            }
        }
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
        animator.SetFloat("Speed", MoveSpeed);
        animator.SetFloat("Slider", V);


    }

    void JumpUp()
    {
        if (Jump)
        {
            JSpeed -= 2 * Gravity * Time.deltaTime;
            Controller.Move(Vector3.up * Time.deltaTime * JSpeed);
        }
    }

    void SetFacingDirection(Direction dir)
    {
        if (direction != dir)
        {
            transform.Rotate(Vector3.up * (direction - dir));
            direction = dir;
        }
    }

    void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.tag == "Enemy")
        {
            PlayerDead = true;
        }
    }
}