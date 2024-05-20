using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceEnemy : MonoBehaviour
{
    [SerializeField]
    string ID;

    [SerializeField]
    float Distance;

    [SerializeField]
    float Offset;

    [SerializeField]
    bool TriggerOnDeath;

    private void Update()
    {

        if (TriggerOnDeath)
        {
            return;
        }
        if (Vector3.Distance(ControllerGame.Player.transform.position, transform.position) < Distance)
        {
            ControllerGame.ControllerDialog.TriggerDialogue(ID, transform, new Vector3(0, Offset, 0));
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            if (TriggerOnDeath)
            {
                ControllerGame.ControllerDialog.TriggerDialogue(ID, ControllerGame.Player.transform, new Vector3(0, 2, 0));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Distance);
    }
}
