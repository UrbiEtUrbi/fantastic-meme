using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjectTimed : PoolObject
{

    [SerializeField]
    bool autoKill;
    [SerializeField]
    protected float duration;



    public override void Reuse()
    {
        if (autoKill)
        {
            StartTicking();
        }
        base.Reuse();
    }

    public void StartTicking()
    {

        Invoke(nameof(SelfDestruct), duration);

    }

    public void StartTicking(float _duration)
    {
        duration = _duration;
        StartTicking();
    }


    void SelfDestruct()
    {
        PoolManager.Despawn(this);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
