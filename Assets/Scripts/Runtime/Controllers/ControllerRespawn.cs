using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerRespawn : MonoBehaviour
{



    public EntrancePosition CurrentRespawn;

    public UnityEvent OnPlayerRepawned = new();


    public void Register(EntrancePosition respawnPosition)
    {

        if(CurrentRespawn == null || CurrentRespawn.transform.position.x < respawnPosition.transform.position.x){
            CurrentRespawn = respawnPosition;
        }
    }


    IEnumerator RespawnCoroutine;
    public void Respawn(float delay, Vector3 pos = default)
    {

        if (RespawnCoroutine != null)
        {
            return;
        }
        if (pos == default)
        {

            pos = CurrentRespawn.transform.position;
        }
        StartCoroutine(RespawnCoroutine = WaitAndRespawn(delay, pos));
      

    }

    IEnumerator WaitAndRespawn(float delay, Vector3 pos)
    {
       
        yield return new WaitForSeconds(delay);
        //move the player a bit to the right
        ControllerGame.Player.transform.position = pos + new Vector3(0.1f, 0, 0);
        OnPlayerRepawned.Invoke();
        yield return new WaitForSeconds(2f);

        RespawnCoroutine = null;
    }

}
