using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbagePlot : PickupDispenser
{
    protected override void OnTimerEnd()
    {
        if (View != null)
        {
            if (View != null)
            {
                spriteRenderers.Remove(View.GetComponent<SpriteRenderer>());
                Destroy(View.gameObject);
            }
        }

    }
}
