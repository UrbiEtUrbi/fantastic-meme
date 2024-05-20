using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntrancePosition : MonoBehaviour
{

    public int Id;
    public void Register()
    {
        ControllerGame.ControllerRespawn.Register(this);
    }


    private void FixedUpdate()
    {

        if (ControllerGame.Player == null)
        {
            return;
        }
        if (ControllerGame.Player.transform.position.x > transform.position.x)
        {
            Register();
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
