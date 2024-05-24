using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHealth, IPickupCollector
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

    
    PickupType CurrentPickup;
    Item CurrentItem;



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
        SoundManager.Instance.Play("duck_die", ControllerGame.Player.transform);
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


    public void Pickup(PickupType pickupType)
    {
        switch (pickupType)
        {
            case PickupType.Seed:

                break;

            case PickupType.Plant:

                break;

        }

    }

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

        if (CurrentItem != null)
        {
            CurrentItem.transform.localPosition = new Vector3(-0.4f * movement.Direction, 0.4f, 0);
        }

    }

    public void Knock(Vector3 direction)
    {
        if (IFrameTime < 0)
        {
            movement.Knock(direction);
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


    //interface implementation IPickupCollector
    public bool CanPickup(PickupType pickupType)
    {
        return CurrentPickup == PickupType.None;
    }

    public bool CanPlace(PickupType pickupType)
    {
        return CurrentPickup == pickupType;
    }

    public void PickUp(PickupType pickupType)
    {
        CurrentItem = ControllerGame.ControllerPickups.Pickup(pickupType);
        CurrentPickup = CurrentItem.PickupType;
        CurrentItem.transform.SetParent(transform);
        CurrentItem.transform.localPosition = new Vector3(-0.4f * movement.Direction, 0.4f, 0);
    }


    public PickupType GetPickupType => CurrentPickup;

    public TimeZone GetTimeZone => ControllerGame.TimeManager.TimeZone;

    public Item Place()
    {
        CurrentPickup = PickupType.None;
        var item = CurrentItem;
        CurrentItem = null;
        return item;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + SwordAttackPosition, SwordAttackSize);
    }
}
