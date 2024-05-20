using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(DestroyDelayed))]
public class AttackObject : MonoBehaviour
{
    [SerializeField]
    LayerMask TargetLayer;
    DestroyDelayed dd;

    bool initialized = false;

    Vector2 attackSize;
    int damage;

    [SerializeField]
    bool generateImpulse;
    [SerializeField]
    float impulseAmplitude;

    AttackType type;
    private void Awake()
    {
        dd = GetComponent<DestroyDelayed>();
     
    }

    public void Init(Vector2 size, Vector3 position, int _damage, float lifetime, AttackType type)
    {
        if (lifetime > 0)
        {
            dd.Init(lifetime);
        }
        attackSize = size;
        damage = _damage;
        transform.position = position;
        initialized = true;
        this.type = type;
    }


    private void Update()
    {
        if (!initialized) {
            return;
        }
        var colliders = Physics2D.OverlapBoxAll(transform.position, attackSize, transform.rotation.eulerAngles.y, TargetLayer);

        foreach (var colliderHit in colliders)
        {
            if (colliderHit != null)
            {
               
                var h = colliderHit.GetComponent<IHealth>();
                if (h == null)
                {
                    continue;
                }


                colliderHit.GetComponent<IHealth>().ChangeHealth(-damage, type);
                CancelInvoke();
                if (generateImpulse)
                {
                    var cis = GetComponent<CinemachineImpulseSource>();
                    cis.GenerateImpulse(new Vector3(Random.Range(-1f, 1f), Random.Range(-1, 1f), 0) * impulseAmplitude);
                }

                var lp = GetComponent<LinearProjectile>();
                if (lp != null)
                {
                    lp.BeforeDestroy();
                }
                Destroy(gameObject);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }



}
