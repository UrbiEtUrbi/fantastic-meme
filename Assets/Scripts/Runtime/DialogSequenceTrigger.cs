using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceTrigger : MonoBehaviour
{


    [SerializeField]
    string ID;

    [SerializeField]
    bool AttachToPlayer = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {

       
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ControllerGame.ControllerDialog.TriggerDialogue(ID, AttachToPlayer ? ControllerGame.Player.transform : transform, new Vector3(0,10,0));
        }
    }
}
