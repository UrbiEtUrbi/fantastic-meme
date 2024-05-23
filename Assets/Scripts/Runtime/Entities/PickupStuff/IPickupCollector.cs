using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupCollector 
{

    bool CanPickup(PickupType pickupType);
    void PickUp(PickupType pickupType);
    Item Place();
    PickupType GetPickupType
    {
        get;
    }
    TimeZone GetTimeZone
    {
        get;
    }

}
