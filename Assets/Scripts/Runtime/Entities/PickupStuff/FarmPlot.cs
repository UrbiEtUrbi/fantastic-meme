using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : PickupSink
{


    protected override void HandleItem(Item item)
    {
        base.HandleItem(item);

        item.transform.SetParent(transform);
        item.transform.localPosition = default;


        spriteRenderers.Add(item.GetComponentInChildren<SpriteRenderer>());

        SetGlobalTime(ControllerGame.TimeManager.TimeZone);
    }
}
