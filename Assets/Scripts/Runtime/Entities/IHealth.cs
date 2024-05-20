using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    public void SetInitialHealth(int amount);
    public void ChangeHealth(int amount);
    public void ChangeHealth(int amount, AttackType type);
    public void Die();
}
