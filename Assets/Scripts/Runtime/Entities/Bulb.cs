using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulb : Creature
{


    protected override bool Enabled => StunTimer < 0 && !isFrozen;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (returnFromFixedUpdate)
        {
            return;
        }

        var direction = (ControllerGame.Player.transform.position - transform.position).normalized;

        if (direction.x > 0 && this.direction == -1 || direction.x < 0 && this.direction == 1)
        {
            turningAround = true;
            PauseTimer = 0.1f;
        }


        if (PauseTimer > 0)
        {
            return;
        }
        else
        {
            if (isActive)
            {
                PauseTimer = PauseTime + Random.value*2;
                _Animator.SetTrigger("Shot");

            }
        }

        
    }


   

    void Shoot()
    {


        SoundManager.Instance.Play("freeze_enemy");
        ControllerGame.ControllerAttack.Attack(
            transform,
            false,
            AttackType.BulbShoot,
            transform.position - new Vector3(0,2,0),
            AttackArea,
            1,
            direction,
            3);
            
           
    }

    
}
