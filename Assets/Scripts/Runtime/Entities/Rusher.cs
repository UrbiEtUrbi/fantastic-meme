using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rusher : Creature, IPickupCollector
{



    [SerializeField]
    float Acceleration;


    Vector2 m_Velocity;

    Rigidbody2D rb;

    [SerializeField]
    float RushTime;
    float m_RushTimer;

    bool waiting;

    public bool targetCabbage;

    CabbagePlot target;


    List<CabbagePlot> cabbagePlots;

    public PickupType GetPickupType => PickupType.Plant;

    Vector3 starpos;
    float StopDistance;

    protected override void Awake()
    {
        starpos = transform.position;
        base.Awake();
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (targetCabbage)
        {
            cabbagePlots = FindObjectsByType<CabbagePlot>(sortMode: FindObjectsSortMode.None).ToList();
        }
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


        if (RushTime > 0 && m_RushTimer <= 0 && !waiting)
        {
            PauseTimer = PauseTime;
            waiting = true;
            m_Velocity = default;
            rb.velocity = default;
            rb.simulated = false;
        }

        var dir = (ControllerGame.Player.transform.position - transform.position).normalized;
        var distance = Vector3.Distance(ControllerGame.Player.transform.position, transform.position);
        if (targetCabbage)
        {
            if (CurrentTimeZone == TimeZone.Present)
            {
                if (target == null)
                {
                    cabbagePlots = cabbagePlots.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
                    foreach (var c in cabbagePlots)
                    {

                       
                        if (c.View != null)
                        {
                            target = c;
                            break;

                        }
                    }
                }
                if (target != null && target.View != null)
                {
                    distance = float.MinValue;
                    if (Vector3.Distance(target.transform.position, transform.position) < StopDistance)
                    {
                        dir = default;
                    }
                    dir = (target.transform.position - transform.position).normalized;
                }
            }
            else
            {
                target = null;
            }
        }

        if (distance > ActivationDistance)
        {
            dir = (starpos - transform.position).normalized;
        }

        if (target != null && distance < StopDistance)
        {
            dir = default;
        }



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

    public bool CanPickup(PickupType pickupType)
    {
        return pickupType == GetPickupType;
    }

    public void PickUp(PickupType pickupType)
    {
        
    }

    public Item Place()
    {
        return default;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (targetCabbage && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
        base.OnDrawGizmosSelected();
    }
}
