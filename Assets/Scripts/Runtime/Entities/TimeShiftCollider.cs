using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeShiftCollider : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Enemy"))
        {
            ControllerGame.TimeManager.AddToTimeshift(collision.gameObject.GetComponent<Creature>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Enemy"))
        {
            ControllerGame.TimeManager.RemoveFromTimeshift(collision.gameObject.GetComponent<Creature>());
        }
    }
}
