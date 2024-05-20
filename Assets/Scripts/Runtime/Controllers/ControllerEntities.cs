using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEntities : MonoBehaviour
{

    [Disable]
    [SerializeField]
    List<Entity> Entities = new();




    public void RegisterEntity(Entity entity)
    {
        Entities.Add(entity);
    }

    public void UnregisterEntity(Entity entity)
    {
        Entities.Remove(entity);
    }


    private void FixedUpdate()
    {

        if (!ControllerGame.Initialized || ControllerGame.Player == null)
        {
            return;
        }
        var playerPos = ControllerGame.Player.transform.position;
        foreach (var e in Entities)
        {
            CheckEntityDistance(playerPos, e);
        }
    }

    // check if we need to activate or deactivate the entity
    void CheckEntityDistance(Vector3 playerPosition, Entity e)
    {
        if (!e.IsActive)
        {

            //if (e.ActivationDistance < 0 || Vector3.Distance(playerPosition, transform.position) <= e.ActivationDistance)
            if (e.ActivationDistance < 0 || Vector3.Distance(playerPosition, e.transform.position) <= e.ActivationDistance)
            {
                e.ToggleActive(true);
                return;
            }
        }
        else
        {
            //if (Vector3.Distance(playerPosition, transform.position) >= e.DeactivationDistance)
            if (Vector3.Distance(playerPosition, e.transform.position) >= e.DeactivationDistance)
            {
                e.ToggleActive(false);
            }
        }
    }
}
