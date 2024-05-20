using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phillip : Creature
{


    [SerializeField]
    Vector3 CurrentSpeed;

    [SerializeField]
    float MaxSpeed;

    protected override bool Enabled => base.Enabled & isActive;

    public override void ToggleActive(bool _isActive)
    {
        base.ToggleActive(_isActive);

        _Animator.SetBool("IsFlying", true);
        _Animator.SetBool("IsSleeping", false);
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (returnFromFixedUpdate)
        {
            return;
        }

        if (!IsActive)
        {
            return;
        }


        var direction = (ControllerGame.Player.transform.position - transform.position).normalized;


        CurrentSpeed += direction * Speed * Time.fixedDeltaTime;

        if (CurrentSpeed.magnitude > MaxSpeed)
        {
            CurrentSpeed = CurrentSpeed.normalized * MaxSpeed;
        }

        transform.position += CurrentSpeed;

        if (direction.x > 0 && this.direction == -1 || direction.x < 0 && this.direction == 1)
        {
            CurrentSpeed = default;
            turningAround = true;
            PauseTimer = PauseTime;
        }
    }


    protected override void OnRbFreezeEnd()
    {
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.mass = 10f;
        rb.drag = 5f;
        rb.gravityScale = 10f;
        gameObject.layer = LayerMask.NameToLayer("Particles");
    }
    protected override void OnStun()
    {
        base.OnStun();
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.freezeRotation = true;
        rb.mass = 10f;
        rb.drag = 5f;
        rb.gravityScale = 10f;
        gameObject.layer = LayerMask.NameToLayer("Particles");
        OnFreezeEnd();
    }

    protected override void OnEndStun()
    {
        base.OnEndStun();
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        gameObject.layer = LayerMask.NameToLayer("Enemy");

    }

    

}
