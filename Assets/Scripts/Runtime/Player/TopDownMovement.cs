using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{


    //fields
    [BeginGroup("Movement")]
    [SerializeField]
    float MaxSpeed;

    [SerializeField]
    float MaxSpeedAbility;

    [SerializeField]
    float Acceleration;

    [EndGroup, SerializeField]
    float Drag;


    //private fields
    int m_GroundLayer;
    SpriteRenderer m_Sprite;
    Vector2 m_Velocity;
    Vector2 m_Speed;
    Rigidbody2D m_Rb;

    //properties
    public int Direction => m_Sprite.flipX ? -1 : 1;
    public SpriteRenderer Sprite => m_Sprite;

    //public fields
    [HideInInspector]
    public bool lockOrientation;

    [SerializeField]
    TimeShiftView Mask;

    Vector2 KnockForce;

    [SerializeField]
    float knockForce;



    [HideInInspector]
    public bool Slowing;

    void OnEnable()
    {
        m_GroundLayer = LayerMask.GetMask("Ground");
        m_Sprite = GetComponentInChildren<SpriteRenderer>();
        m_Rb = GetComponent<Rigidbody2D>();

        ControllerInput.Instance.Horizontal.AddListener(OnHorizontal);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
        ControllerInput.Instance.Jump.AddListener(OnJump);
        ControllerInput.Instance.Attack.AddListener(OnAttack);
        ControllerInput.Instance.Cast.AddListener(OnCast);
    }

    void OnDisable()
    {
        ControllerInput.Instance.Horizontal.RemoveListener(OnHorizontal);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance.Jump.RemoveListener(OnJump);
        ControllerInput.Instance.Attack.RemoveListener(OnAttack);
        ControllerInput.Instance.Cast.RemoveListener(OnCast);
    }

    bool playingRun;



    void FixedUpdate()
    {
        if (ControllerGame.Instance.IsGameOver)
        {
            m_Velocity = default;
            m_Rb.velocity = default;
            return;
        }

        if (!ControllerGame.Instance.IsGamePlaying)
        {
           
            m_Speed = default;
            m_Velocity = default;
            m_Rb.velocity = default;
            return;
        }

       

        if (m_Speed == default)
        {
            m_Velocity *= Drag;
        }
        else
        {

            m_Velocity += m_Speed;
        }

        var maxSpeed = Mask.isShowing ? MaxSpeedAbility : MaxSpeed;

        m_Velocity = m_Velocity.normalized * Mathf.Min((m_Velocity).magnitude, maxSpeed + KnockForce.magnitude);



        m_Rb.velocity = m_Velocity;

        KnockForce *= 0.9f;

        m_Sprite.flipX = m_Velocity.x < 0;

        if (!playingRun)
        {
            if (m_Rb.velocity.magnitude > 0.1f)
            {
                playingRun = true;
                SoundManager.Instance.PlayLooped("duck_run", source: gameObject, transform);
            }
        }
        if (playingRun)
        {
            if (m_Rb.velocity.magnitude <= 0.1f)
            {
                playingRun = false;
                SoundManager.Instance.CancelLoop(gameObject);
            }
            
        }
    }

        public void Revive()
    {
        //Animator.SetBool("IsDead", false);
        m_Velocity = default;
        m_Speed = default;
    }

    void OnHorizontal(float amount)
    {
        m_Speed = new Vector2(amount * Acceleration, m_Speed.y);   
    }


    void OnVertical(float amount)
    {
        m_Speed = new Vector2(m_Speed.x, amount * Acceleration);
        // Animator.SetBool("IsSitting", amount < 0);
    }

    void OnCast()
    {
        //if (AttackTimer > 0)
        //{
        //    return;
        //}
        //if (!ControllerGame.Instance.HasIceMelee && !ControllerGame.Instance.HasSpike)
        //{
        //    return;
        //}
        //AttackTimer = AttackCooldown;


        //SoundManager.Instance.Play("spike_shoot");
        //Animator.SetBool("IsCasting", true);

    }

    public void Knock(Vector3 direction)
    {
        KnockForce = direction * knockForce;
        m_Rb.velocity += KnockForce;
        m_Velocity += KnockForce;
    }

    void OnAttack()
    {


        //if (!ControllerGame.Instance.HasStick)
        //{
        //    return;
        //}
        //if (AttackTimer > 0)
        //{
        //    return;
        //}




        //AttackTimer = AttackCooldown;

        //SoundManager.Instance.Play("melee_attack");
        //Animator.SetBool("IsAttacking", true);


    }

    void OnEndAttack()
    {
        //Animator.SetBool("IsAttacking", false);
        //Animator.SetBool("IsCasting", false);
    }

    void OnJump(bool jump)
    {
        if (jump)
        {
            Mask.Show();
        }
        else
        {
            Mask.Hide();

        }
        //jumped = jump && onGround;
        //if (jumped)
        //{

        //    jumpApplied = false;

        //}

    }
}
