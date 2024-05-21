using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rusher : Creature
{



    [SerializeField]
    float Acceleration;


    Vector2 m_Velocity;

    Rigidbody2D rb;

    [SerializeField]
    float RushTime;
    float m_RushTimer;

    bool waiting;


    protected override void Awake()
    {
        base.Awake();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (returnFromFixedUpdate)
        {
            return;
            
        }

        if (!ControllerGame.TimeManager.IsTimeShiftActive && ControllerGame.TimeManager.TimeZone != CurrentTimeZone)
        {
            rb.velocity = default;

            return;
        }


        if (m_RushTimer <= 0 && !waiting)
        {
            PauseTimer = PauseTime;
            waiting = true;
            m_Velocity = default;
            rb.velocity = default;
            rb.simulated = false;
        }


        var dir = (ControllerGame.Player.transform.position - transform.position).normalized;

        if (direction != Mathf.Sign(dir.x))
        {
            turningAround = true;
        }

        if (PauseTimer >= 0)
        {

            return;
        }

        if (waiting)
        {
            waiting = false;
            rb.simulated = true;
            m_RushTimer = RushTime;
        }

      
        var dir2 = new Vector2(dir.x, dir.y);
        m_Velocity += dir2 * Acceleration * Time.fixedDeltaTime;
        m_Velocity = m_Velocity.normalized * Mathf.Min(m_Velocity.magnitude, Speed);

        rb.velocity = m_Velocity;
    }

    protected override void Update()
    {
        base.Update();
        m_RushTimer -= Time.deltaTime;
    }
}
