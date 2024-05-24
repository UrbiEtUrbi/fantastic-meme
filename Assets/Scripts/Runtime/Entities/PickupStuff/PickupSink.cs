using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupSink : PickupHandler
{




    protected override void OnTimerEnd()
    {
        base.OnTimerEnd();
        var item = CurrentlyAttending.Place();
        HandleItem(item);
    }


    protected virtual void HandleItem(Item item)
    {



    }



    public override bool CanInteract(IPickupCollector pickupCollector)
    {
        return pickupCollector.CanPlace(PickupType);
    }


}
