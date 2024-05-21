using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeShiftCollider : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

            ControllerGame.TimeManager.AddToTimeshift(collision.gameObject.GetComponent<Creature>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

            ControllerGame.TimeManager.RemoveFromTimeshift(collision.gameObject.GetComponent<Creature>());
        }
    }
}
