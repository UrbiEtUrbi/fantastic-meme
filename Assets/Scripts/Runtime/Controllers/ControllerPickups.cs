using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPickups : MonoBehaviour
{
    public List<string> PickedUp = new();


    public void Pickup(Pickup pickup)
    {


        if (pickup.OneTime)
        {
            PickedUp.Add(pickup.Id);
        }

        switch (pickup.PickupType)
        {
            case PickupType.MaxHealth:
                ControllerGame.Instance.IncreaseMax(1);
                ControllerGame.Player.ChangeHealth(1);
                break;

            case PickupType.Heart:
                ControllerGame.Player.ChangeHealth(1);
                break;

            case PickupType.MaxStamina:
                ControllerGame.Instance.Win();
                break;

            case PickupType.StaminaCharge:
                break;

            case PickupType.Spike:
                ControllerGame.Instance.HasSpike = true;
                break;

            case PickupType.Stick:
                ControllerGame.Instance.HasStick = true;
                break;

            case PickupType.IceMelee:
                ControllerGame.Instance.HasIceMelee = true;
                break;

        }

        ControllerGame.Instance.Save();

    }

    public bool HasPickedUp(Pickup pickup)
    {
        return pickup.OneTime && PickedUp.Contains(pickup.Id);
    }
}
