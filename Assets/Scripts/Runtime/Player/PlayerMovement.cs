using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    float MaxSpeed;

    [SerializeField]
    float HorizontalAcc;

    [SerializeField]
    float Drag;

    [SerializeField]
    float JumpVelocity;

    SpriteRenderer m_Sprite;


    public bool Slowing;

    float HorizontalSpeed;
    float HorizontalVelocity;

    bool jumped;
    bool jumpApplied = true;
    [SerializeField]
    bool onGround = true;


    [SerializeField]
    Vector2 groundCheckSize;
    [SerializeField]
    Vector2 groundCheckPosition;

    [SerializeField]
    Animator Animator;

    int GroundLayerMask;

    public int Direction => m_Sprite.flipX ? -1 : 1;
    public SpriteRenderer Sprite => m_Sprite;

    [SerializeField]
    float AttackCooldown;

    float AttackTimer = 0;

    Vector2 KnockForce;

    void OnEnable()
    {
        jumpApplied = true;
        GroundLayerMask = LayerMask.GetMask("Ground");
        ControllerInput.Instance.Horizontal.AddListener(OnHorizontal);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
        ControllerInput.Instance.Jump.AddListener(OnJump);
        ControllerInput.Instance.Attack.AddListener(OnAttack);
        ControllerInput.Instance.Cast.AddListener(OnCast);
        m_Sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void OnDisable()
    {
        ControllerInput.Instance.Horizontal.RemoveListener(OnHorizontal);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance.Jump.RemoveListener(OnJump);
        ControllerInput.Instance.Attack.RemoveListener(OnAttack);
        ControllerInput.Instance.Cast.RemoveListener(OnCast);
    }

    private void Update()
    {
        AttackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {

        if (ControllerGame.Instance.IsGameOver)
        {
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);
            return;
        }
        if (jumpApplied && rb.velocity.y > 20 && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
      
        if (!ControllerGame.Instance.IsGamePlaying)
        {
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                rb.velocity = new Vector2();
                Animator.SetBool("IsWalking", false);
                Animator.SetBool("Falling", false);
                Animator.SetBool("Jumping", !onGround);
                HorizontalVelocity = 0;
                HorizontalSpeed = 0;
                rb.bodyType = RigidbodyType2D.Static;
            }
            return;
        }
        else if (!jumped &&
            Mathf.Abs(HorizontalVelocity) < 0.1f &&
            (Mathf.Abs(rb.velocity.y) < 2f && onGround) &&
            Mathf.Abs(HorizontalSpeed) < 0.1f
            && KnockForce.magnitude < 0.1f)
        {
            Animator.SetBool("IsWalking", false);
            Animator.SetBool("Falling", false);
            Animator.SetBool("Jumping", !onGround);
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (Slowing)
        {
            HorizontalVelocity *= Drag;
        }
        else
        {

            HorizontalVelocity += HorizontalSpeed;
        }
        HorizontalVelocity = Mathf.Clamp(HorizontalVelocity, -MaxSpeed - Mathf.Abs(KnockForce.x), MaxSpeed + Mathf.Abs(KnockForce.x));;

        rb.velocity = new Vector2(HorizontalVelocity, rb.velocity.y);
        KnockForce *= 0.9f;




        if (jumped)
        {
            SoundManager.Instance.Play("jump");
            jumped = false;
            rb.velocity = new Vector2(rb.velocity.x, JumpVelocity);
        }

        if (!jumpApplied && !onGround)
        {
            jumpApplied = true;
        }

        if (HorizontalVelocity != 0 && !Slowing && !lockOrientation)
        {
            m_Sprite.flipX = HorizontalVelocity < 0;
        }


        bool falling = !onGround;

        Animator.SetBool("IsWalking", Mathf.Abs(HorizontalVelocity) > 10f && !Animator.GetBool("IsAttacking"));
        if (rb.velocity.y == 0 && onGround)
        {
            Animator.SetBool("Falling", false);
            Animator.SetBool("Jumping", false);
        }
        else
        {
            Animator.SetBool("Falling", falling && rb.velocity.y < -10);
            Animator.SetBool("Jumping", falling && rb.velocity.y > -10);
        }

        onGround = Physics2D.OverlapBox(rb.position + groundCheckPosition, groundCheckSize, 0, GroundLayerMask) != null;
        if (falling && onGround)
        {
            SoundManager.Instance.Play("land");
        }

      
    }

    [SerializeField]
    float knockForce;


    public void Knock(Vector3 direction)
    {
        jumpApplied = false;


        direction = new Vector3(Mathf.Sign(direction.x), 0.2f, 0).normalized;
        
        KnockForce = direction *knockForce;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity += KnockForce;
        HorizontalVelocity += KnockForce.x;
      

    }

    public void Die()
    {

        foreach (var p in Animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name != "IsDead")
            {
                Animator.SetBool(p.nameHash, false);
            }
        }
        Animator.SetBool("IsDead", true);
    }

    public void Revive()
    {
        Animator.SetBool("IsDead", false);
        HorizontalVelocity = 0;
        HorizontalSpeed = 0;
        rb.velocity = default;
    }

    void OnHorizontal(float amount) {

        HorizontalSpeed = amount*HorizontalAcc;

        Slowing = Mathf.Abs(amount) <0.1f;
    }

    public bool lockOrientation;

    void OnVertical(float amount)
    {
        lockOrientation = amount > 0;
        Animator.SetBool("IsSitting", amount < 0);
    }

    void OnCast()
    {
        if (AttackTimer > 0)
        {
            return;
        }
        if (!ControllerGame.Instance.HasIceMelee && !ControllerGame.Instance.HasSpike)
        {
            return;
        }
        AttackTimer = AttackCooldown;


        SoundManager.Instance.Play("spike_shoot");
        Animator.SetBool("IsCasting", true);
        
    }

    void OnAttack()
    {

        if (!ControllerGame.Instance.HasStick)
        {
            return;
        }
        if (AttackTimer > 0)
        {
            return;
        }



       
        AttackTimer = AttackCooldown;

        SoundManager.Instance.Play("melee_attack");
        Animator.SetBool("IsAttacking", true);
        
      
    }

    void OnEndAttack()
    {
        Animator.SetBool("IsAttacking", false);
        Animator.SetBool("IsCasting", false);
    }

    void OnJump(bool jump)
    {
        
        jumped = jump && onGround;
        if (jumped)
        {
            
            jumpApplied = false;
            
        }

    }

    public void ResetJump()
    {
        lockOrientation = false;
        onGround = Physics2D.OverlapBox(rb.position + groundCheckPosition, groundCheckSize, 0, GroundLayerMask) != null;
        Animator.SetBool("Jumping", !onGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rb.position+ groundCheckPosition, groundCheckSize);
    }
}
