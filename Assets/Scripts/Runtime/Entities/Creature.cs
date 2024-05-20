using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity, IHealth
{

    [BeginGroup("Health")]
    [EndGroup]
    [SerializeField]
    int MaxHealth;


    int currentHealth;


    [SerializeField]
    protected float Speed;

    [SerializeField]
    protected float PauseTime;

    [SerializeField]
    protected float StunTime;

    protected int direction;

    protected bool isStunned = false;
    protected bool isFrozen = false;

    [SerializeField]
    protected Animator _Animator;

    [SerializeField]
    SpriteRenderer art;


    [SerializeField]
    protected Vector2 frozenSize;

    [SerializeField]
    protected Vector2 frozenOffset;

    [SerializeField]
    string stunSound;

    [SerializeField]
    protected Vector2 AttackArea;

    protected float PauseTimer = 0;

    protected float StunTimer = 0;


    protected virtual bool Enabled => enabledCreature;

    protected bool enabledCreature => PauseTimer < 0 && StunTimer < 0 && !isFrozen;

    protected int GroundMask;

    protected Vector2 colliderOffset;
    protected Vector2 colliderSize;


    protected bool turningAround = false;

    [SerializeField]
    bool ImmuneToFreeze;

    [SerializeField]
    bool ImumneToStun;

    protected bool returnFromFixedUpdate;
    protected virtual void FixedUpdate()
    {

        if (ControllerGame.Instance.IsGameOver)
        {
            returnFromFixedUpdate = true;
            return;
        }
        returnFromFixedUpdate = false;
        _Animator.SetBool("Stunned", isStunned);
        _Animator.SetBool("Frozen", isFrozen);


        if (!Enabled)
        {
            returnFromFixedUpdate = true;
            return;
        }
        if (turningAround)
        {
            _Animator.speed = 1;
            turningAround = false;
            direction *= -1;
            art.flipX = !art.flipX;
        }

        if (isStunned)
        {
            isStunned = false;
            OnEndStun();
        }

        var overlap = Physics2D.OverlapBox(transform.position, AttackArea, 0, LayerMask.GetMask("Player"));

        if (overlap)
        {
            var f = (ControllerGame.Player.transform.position - transform.position).normalized;

            ControllerGame.Player.Knock(f);
            
            ControllerGame.Player.ChangeHealth(-1);
        }


    }



    protected virtual void Start()
    {
        GroundMask = LayerMask.GetMask("Ground");
        direction = -1;

        currentHealth = MaxHealth;
        SaveCollider();
    }

    protected virtual void SaveCollider()
    {
        var collider = GetComponent<BoxCollider2D>();
        colliderOffset = collider.offset;
        colliderSize = collider.size;
    }


    public  virtual void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
        if (amount < 0)
        {
            //spawn damage vfx
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

   

    public virtual void Die()
    {
        ControllerGame.ControllerAttack.OnEnemyDied();
        //spawn death animation
        Destroy(gameObject);
    }

    public virtual void ChangeHealth(int amount, AttackType type)
    {
        if (type == AttackType.PlayerSword && !ImumneToStun)
        {
            
            if (isFrozen)
            {
                OnRbFreezeEnd();
                isFrozen = false;
            }

            isStunned = true;
            StunTimer = StunTime;
            SoundManager.Instance.Play(stunSound);
            OnStun();
            return;
        }
        else if ((type == AttackType.IceSpike || type == AttackType.IceMelee) && !ImmuneToFreeze)
        {
            SoundManager.Instance.Play("crate_ice_block");
            isFrozen = true;
            isStunned = false;
            StunTimer = StunTime;
            gameObject.layer = LayerMask.NameToLayer("Ground");

            OnFreeze();
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.mass = 10f;
            rb.drag = 5f;
            rb.gravityScale = 10f;
            return;
        }
        ChangeHealth(amount);
    }


    protected virtual void OnRbFreezeEnd()
    {
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }


    protected virtual void OnFreeze()
    {
        var c = GetComponent<BoxCollider2D>();
        c.size = frozenSize;
        c.offset = frozenOffset;
    }

    protected virtual void OnFreezeEnd()
    {
        var c = GetComponent<BoxCollider2D>();
        if (c != null)
        {
            c.size = colliderSize;
            c.offset = colliderOffset;
        }
    }


    protected virtual void OnStun()
    {
        
    }

    protected virtual void OnEndStun()
    {

    }

    protected virtual void Update()
    {
        PauseTimer -= Time.deltaTime;
        StunTimer -= Time.deltaTime;

      
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position, ActivationDistance);

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position, DeactivationDistance);

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, AttackArea);

    }


    public void SetInitialHealth(int amount) {
        currentHealth = amount;
    }

    
}
