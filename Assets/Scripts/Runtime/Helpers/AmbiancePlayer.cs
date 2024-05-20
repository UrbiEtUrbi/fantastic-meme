using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiancePlayer : MonoBehaviour
{
    [SerializeField]
    string ToPlay;


    private void Start()
    {
        SoundManager.Instance.CancelAllLoops();

        SoundManager.Instance.PlayLooped(ToPlay);
    }
}
