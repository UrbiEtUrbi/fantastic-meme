using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boner : Creature
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






    protected override void FixedUpdate()
    {

        base.FixedUpdate();


        if (returnFromFixedUpdate)
        {
            return;
        }
        var pos2 = new Vector2(transform.position.x, transform.position.y);
        var groundHit = Physics2D.OverlapBox(pos2 + new Vector2(-direction * GroundCheckPosition.x, GroundCheckPosition.y), GroundCheckSize, 0, GroundMask);
        var wallHit = Physics2D.OverlapBox(pos2+  new Vector2(-direction * WallCheckPostion.x, WallCheckPostion.y), WallCheckSize, 0, GroundMask);


       
        _Animator.SetBool("Walking", true);



        if (!groundHit || wallHit)
        {
            _Animator.speed = 0;

            PauseTimer = PauseTime;
            turningAround = true;
        }
        transform.position += new Vector3(direction * Speed * Time.fixedDeltaTime, 0, 0);
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
