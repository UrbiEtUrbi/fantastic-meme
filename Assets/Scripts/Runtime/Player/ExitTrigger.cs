using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{


    [SerializeField]
    public int Room;

    [SerializeField]
    public int Entrance;


    BoxCollider2D BoxCollider2D;

    private void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!ControllerGame.Instance.IsGameOver)
        {
            StartCoroutine(ControllerGame.Rooms.LoadRoom(Room, Entrance));
        }

    }
    private void OnDrawGizmos()
    {
        if (!BoxCollider2D)
        {
            BoxCollider2D = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(BoxCollider2D.offset.x , BoxCollider2D.offset.y,0), BoxCollider2D.size);
    }
}
