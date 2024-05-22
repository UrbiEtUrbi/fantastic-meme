using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbagePlot : PickupDispenser
{
    protected override void OnTimerEnd()
    {
        if (View != null)
        {
            Destroy(View.gameObject);
        }

    }
}
