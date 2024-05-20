using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{

    [SerializeField]
    float Height;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ControllerGame.Instance.IsGameOver && ControllerGame.Player.transform.position.y < Height)
        {
            ControllerGame.Player.ChangeHealth(-10);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector3(-1000f, Height, 0), new Vector3(1000f, Height, 0));
    }
}
