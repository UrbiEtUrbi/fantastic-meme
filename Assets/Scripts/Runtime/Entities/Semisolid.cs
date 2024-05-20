using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semisolid : MonoBehaviour
{

    Collider2D Collider;
    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
    }

    float DisableTimer;
    float DisableTime = 0.25f;
    [SerializeField]
    Transform point;

    private void FixedUpdate()
    {
        Collider.enabled = DisableTimer >0 || ControllerGame.Player.transform.position.y > point.position.y;
    }

    public void DisableTmp()
    {
        DisableTimer = DisableTime;
    }
}
