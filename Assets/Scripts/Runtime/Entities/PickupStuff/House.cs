using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : PickupSink
{

    protected override void HandleItem(Item item)
    {
        base.HandleItem(item);
        Destroy(item.gameObject);

        //collect cabbag
        ControllerGame.Instance.CollectCabbage();

        //heal player
        ControllerGame.Player.ChangeHealth(1);
    }
}
