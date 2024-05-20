using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbProjectile : MonoBehaviour, IHealth
{
    public void ChangeHealth(int amount)
    {
        Die();
    }

    public void ChangeHealth(int amount, AttackType type)
    {
        Die();
    }

   

    public void Die()
    {
        var lp = GetComponent<LinearProjectile>();
        if (lp != null)
        {
            lp.BeforeDestroy();
        }
        Destroy(gameObject);
    }

    public void SetInitialHealth(int amount)
    {
       
    }
}
