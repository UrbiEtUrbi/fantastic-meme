using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelayed : MonoBehaviour
{

    [SerializeField]
    float DestroyAfter;
    [SerializeField]
    bool AutoDestroy;

    [SerializeField]
    bool FromAnimation;

    public void DestroyEvent()
    {
        var lp = GetComponentInChildren<LinearProjectile>();
        if (lp != null)
        {
            lp.BeforeDestroy();
        }
        Destroy(gameObject);
    }

    public void Start()
    {
        if (AutoDestroy)
        {
            ScheduleDestroy(DestroyAfter);
        }
    }

    void ScheduleDestroy(float after)
    {
        Invoke(nameof(DestroyEvent), after);

    }

    public void Init(float after)
    {
        ScheduleDestroy(after < 0 ? DestroyAfter : after);
    }
}
