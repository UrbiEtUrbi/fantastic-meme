using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
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

    Transform target;
    CabbagePlot targetCabbagePlot;


    List<CabbagePlot> cabbagePlots;

    public PickupType GetPickupType => PickupType.Plant;

    public TimeZone GetTimeZone => CurrentTimeZone;

    Vector3 starpos;
    [SerializeField]
    float StopDistance;


    bool pathSet;


    [SerializeField]
    Seeker seeker;

    [SerializeField]
    float UpdatePathTime;
    float UpdatePathTimer;

    protected override void Awake()
    {
        starpos = transform.position;
        base.Awake();
        rb = gameObject.GetComponent<Rigidbody2D>();
        
    }

    protected override void Start()
    {
        base.Start();
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

       


        if (RushTime > 0 && m_RushTimer <= 0 && !waiting)
        {
            PauseTimer = PauseTime;
            waiting = true;
            m_Velocity = default;
            rb.velocity = default;

        }

        bool canTargetCabbage = targetCabbage && CurrentTimeZone == TimeZone.Present;
        bool isChasingCabbage = targetCabbage && cabbagePlots.Any(x => x.transform == target);
        bool canTargetPlayer =  Vector3.Distance(ControllerGame.Player.transform.position, transform.position) < ActivationDistance &&
            (ControllerGame.TimeManager.IsTimeShiftActive || ControllerGame.TimeManager.TimeZone == CurrentTimeZone);

        if (canTargetCabbage && !isChasingCabbage)
        {

            cabbagePlots = cabbagePlots.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
            foreach (var c in cabbagePlots)
            {
                if (c.View != null)
                {
                    pathSet = false;
                    target = c.transform;
                    targetCabbage = c;
                    break;
                }
            }
        }

        if (canTargetPlayer && !isChasingCabbage && target == null)
        {
            pathSet = false;
            target = ControllerGame.Player.transform;
        }
        if (!isChasingCabbage && !canTargetPlayer && target != null)
        {
            pathSet = false;
            target = null;
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
        
        CheckPath();
        MovePath();
    }

    Path _path;
    int _currentPathpoint;
    bool _reachedEndOfPath = false;

    void CheckPath()
    {
        if (UpdatePathTimer <= 0 || !pathSet && seeker.IsDone())
        {
            if (target)
            {
                seeker.StartPath(transform.position, target.position, OnPathComplete);
            }
            else
            {
                seeker.StartPath(transform.position, starpos, OnPathComplete);
            }
            UpdatePathTimer = UpdatePathTime;
            pathSet = true;

        }
    }

    Vector3 normalizedDir;

    void MovePath()
    {
        if (_path == null)
        {
            return;
        }

        _reachedEndOfPath = _currentPathpoint >= _path.vectorPath.Count;

        // handle this that after one attack of the player there is a timeout where the harpy loses aggro
        try
        {
            if (_reachedEndOfPath)
            {
                rb.velocity = default;
                m_Velocity = default;
                return;
            }
            Vector2 dir = ((Vector2)_path.vectorPath[_currentPathpoint] - rb.position).normalized;
            m_Velocity += dir * Acceleration * Time.fixedDeltaTime;
            m_Velocity = m_Velocity.normalized * Mathf.Min(m_Velocity.magnitude, Speed);
            normalizedDir = dir;


            var dirToTarget = (target != null ? (target.position - transform.position) : starpos - transform.position);
            if (direction != Mathf.Sign(dirToTarget.x))
            {
                turningAround = true;
            }

            rb.velocity = m_Velocity;
        }
        catch
        {
            pathSet = false;
            return;
        }




        float distance = Vector2.Distance(rb.position, _path.vectorPath[_currentPathpoint]);
        if (distance < StopDistance)
        {
            _currentPathpoint++;
        }
    }


    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        _path = p;
        _currentPathpoint = 0;
    }

    protected override void Update()
    {
        base.Update();
        m_RushTimer -= Time.deltaTime;
        UpdatePathTimer -= Time.deltaTime;
    }

    public bool CanPickup(PickupType pickupType)
    {
        
        return targetCabbage && pickupType == GetPickupType && CurrentTimeZone == TimeZone.Present;
    }

    public bool CanPlace(PickupType pickupType)
    {

        return false;
    }

    public void PickUp(PickupType pickupType)
    {
        target = null;
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
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + normalizedDir);
        
    }
}
