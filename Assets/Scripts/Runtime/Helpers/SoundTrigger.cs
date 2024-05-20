using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{

    [SerializeField]
    string soundName;


    [SerializeField]
    bool PlayOnAwake;

    [SerializeField]
    bool KeepAlive;



    void Start()
    {
        if (PlayOnAwake)
        {
            Play();
        }
    }

    void Play() {
        SoundManager.Instance.Play(soundName);
        if (!KeepAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")){
            Play();
        }
    }

}
