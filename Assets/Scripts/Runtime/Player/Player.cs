using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHealth
{

    [SerializeField]
    Vector3 SwordAttackPosition;

    [SerializeField]
    Vector3 SwordAttackSize;

    [SerializeField]
    Vector3 SpikeAttackSize;

    TopDownMovement movement;


    [SerializeField]
    float IFrames;

    float IFrameTime;

    [SerializeField]
    int MaxHealth;

    int currentHealth;
    public int CurrentHealth => currentHealth;

    void Awake() {
        movement = GetComponent<TopDownMovement>();
    }


    

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {

            if (IFrameTime < 0)
            {

                
                currentHealth += amount;
                currentHealth = Mathf.Max(0, currentHealth);
                if (currentHealth <= 0)
                {
                    Debug.Log("die");
                    Die();
                }
                else
                {
                    SoundManager.Instance.Play("player_die");
                    IFrameTime = IFrames;

                }


            }
        }
        else {

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth,0, MaxHealth);
        }
        ControllerGame.Instance.UpdateHealthUI();
    }

    public void Die()
    {
        ControllerGame.Instance.GameOver();
       // movement.Die();
    }

    public void SetInitialHealth(int amount)
    {
        MaxHealth = amount;
    }

    public void Revive()
    {
        movement.Revive();
    }


    float blinkOn = 0.8f;
    float frequency = 2f;
    float blinkTimer = 0;

    void Update()
    {

        IFrameTime -= Time.deltaTime;

        if (IFrameTime > 0)
        {
            movement.Sprite.enabled = blinkTimer < blinkOn * (1f / frequency);
            blinkTimer += Time.deltaTime;
            if (blinkTimer > 1f / frequency)
            {
                blinkTimer = 0;
            }
            if (IFrameTime > IFrames * 0.95f)
            {
                movement.Slowing = true;
            }
        }
        else
        {
            movement.Sprite.enabled = true;
        }

    }

    public void Knock(Vector3 direction)
    {
        if (IFrameTime < 0)
        {
          //  movement.Knock(direction);
        }
    }

    void Attack()
    {

        Debug.Log("attack");
        ControllerGame.ControllerAttack.Attack(
            transform,
            true,
            AttackType.PlayerSword,
            transform.position + new Vector3(movement.Direction * SwordAttackPosition.x, SwordAttackPosition.y, SwordAttackPosition.z)
            , SwordAttackSize,
            1,
            movement.Direction);
    }

    void Cast()
    {
        Debug.Log("cast");
        if (ControllerGame.Instance.HasSpike)
        {
            ControllerGame.ControllerAttack.Attack(
              transform,
              false,
              AttackType.IceSpike,
              transform.position + new Vector3(movement.Direction * SwordAttackPosition.x, SwordAttackPosition.y, SwordAttackPosition.z)
              , SpikeAttackSize, 1,
              movement.Direction
              );
        }
        if (ControllerGame.Instance.HasIceMelee)
        {
            ControllerGame.ControllerAttack.Attack(
             transform,
             false,
             AttackType.IceMelee,
             transform.position + new Vector3(movement.Direction * SwordAttackPosition.x, SwordAttackPosition.y, SwordAttackPosition.z)
             , SwordAttackSize, 1,
             movement.Direction
             );
        }

    }




    public void ChangeHealth(int amount, AttackType type)
    {
        ChangeHealth(amount);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + SwordAttackPosition, SwordAttackSize);
    }
}
