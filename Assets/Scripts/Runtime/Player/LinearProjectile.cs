using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectile : MonoBehaviour
{


    [SerializeField]
    float Speed;

    int Direction;

    [SerializeField]
    ParticleSystem OnHit;

    public void SetDirection(int d)
    {
        
        Direction = d;
        transform.localScale = new Vector3(d, 1, 1);
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(Direction * Speed * Time.fixedDeltaTime, 0, 0);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        BeforeDestroy();
        Destroy(gameObject);

    }

    public void BeforeDestroy()
    {
        if (OnHit == null)
        {
            return;
        }
        var t = Instantiate(OnHit);

        if (transform.childCount > 0)
        {
            var child = transform.GetChild(0);

            child.SetParent(null);
            child.transform.localScale = new Vector3(1, 1, 1);
            child.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            var dd = child.gameObject.AddComponent<DestroyDelayed>();
            dd.Init(10f);
        }
        t.transform.position = transform.position;
    }
}
