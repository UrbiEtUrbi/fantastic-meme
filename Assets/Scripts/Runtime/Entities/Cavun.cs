using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavun : Creature
{
    [SerializeField]
    Vector3 GroundCheckPosition;

    [SerializeField]
    Vector3 GroundCheckSize;

    [SerializeField]
    Vector3 WallCheckPostion;

    [SerializeField]
    Vector3 WallCheckSize;


    protected override bool Enabled => StunTimer < 0 && !isFrozen;




    [SerializeField]
    ParticleSystem spawnParticles;

    [SerializeField]
    Pickup winPickup;

    float defaultSpeed = 0;
    protected override void Start()
    {
        base.Start();
        defaultSpeed = Speed;
    }


    protected override void FixedUpdate()
    {

        base.FixedUpdate();


        if (returnFromFixedUpdate)
        {
            return;
        }
        var pos2 = new Vector2(transform.position.x, transform.position.y);
        var groundHit = Physics2D.OverlapBox(pos2 + new Vector2(-direction * GroundCheckPosition.x, GroundCheckPosition.y), GroundCheckSize, 0, GroundMask);
        var wallHit = Physics2D.OverlapBox(pos2 + new Vector2(-direction * WallCheckPostion.x, WallCheckPostion.y), WallCheckSize, 0, GroundMask);



        _Animator.SetBool("Walking", true);

        if (hitTimer > 0)
        {
            Speed = 0;
        }
        else
        {
            Speed = defaultSpeed;
        }

        if (!groundHit || wallHit)
        {
            _Animator.speed = 0;

            PauseTimer = PauseTime;
            turningAround = true;
        }
        transform.position += new Vector3(direction * Speed * Time.fixedDeltaTime, 0, 0);
    }

    float hitTimer = 0;
    float hitTime = .3f;
    public override void Die()
    {
        SoundManager.Instance.Play("crate_ice_block");
        isFrozen = true;
        Invoke(nameof(DestroyCavun), 3f);
    }

    public override void ChangeHealth(int amount)
    {
        SoundManager.Instance.Play("boner_die");
        defaultSpeed += 0.5f;
        hitTimer = hitTime;
        base.ChangeHealth(amount);
    }

    protected override void Update()
    {
        base.Update();
        hitTimer -= Time.deltaTime;
    }

    void DestroyCavun() {

        for (int i = 0; i < 10; i++)
        {
            var sp = Instantiate(spawnParticles, GetComponentInParent<Room>().transform);
            sp.transform.position = transform.position + Random.insideUnitSphere * 5f;
        }
        SoundManager.Instance.Play("boner_die");

        ControllerGame.ControllerDialog.TriggerDialogue("magic", ControllerGame.Player.transform);
        var p  = Instantiate(winPickup, GetComponentInParent<Room>().transform);
        p.transform.position = transform.position;
        base.Die();
        
    }

    protected override void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + GroundCheckPosition, GroundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + WallCheckPostion, WallCheckSize);
        base.OnDrawGizmosSelected();
    }
}
