using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

   
    public string Id;

    
    public bool OneTime;

    [SerializeField]
    float Frequency;

    [SerializeField]
    float Amplitude;

    Vector3 startPos;

    float timer = 0;

    [SerializeField]
    string sound;

    private void Start()
    {
        if (ControllerGame.ControllerPickups.HasPickedUp(this))
        {
            gameObject.SetActive(false);
            return;
        }
        startPos = transform.position;

    }

    public PickupType PickupType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")){

            if (ControllerGame.Player.CurrentHealth > 0)
            {
                ControllerGame.ControllerPickups.Pickup(this);
                SoundManager.Instance.Play(sound);
                Destroy(this.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position = startPos + new Vector3(0,Mathf.Sin(timer * Frequency) * Amplitude, 0);
        timer += Time.fixedDeltaTime;
    }

}

[System.Serializable]
public enum PickupType
{
    MaxHealth,
    Heart,
    MaxStamina,
    StaminaCharge,
    Spike,
    Stick,
    IceMelee
}
