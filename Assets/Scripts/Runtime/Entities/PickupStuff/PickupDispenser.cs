using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupDispenser : PickupHandler
{

    protected override void OnTimerEnd()
    {
        base.OnTimerEnd();
        CurrentlyAttending.PickUp(PickupType);
    }



    public override bool CanInteract(IPickupCollector pickupCollector)
    {
        return pickupCollector.CanPickup(PickupType);
    }
}
