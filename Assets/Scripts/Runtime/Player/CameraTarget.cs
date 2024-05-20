using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (ControllerGame.Initialized && ControllerGame.Player != null)
        {
            transform.position = new Vector3(Mathf.Round(ControllerGame.Player.transform.position.x), Mathf.Round(ControllerGame.Player.transform.position.y), Mathf.Round(ControllerGame.Player.transform.position.z));
        
        }
    }
}
