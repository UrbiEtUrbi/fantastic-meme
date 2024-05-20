using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fade;
    private bool fadingToBlack = false;
    private bool faded = false;
    private bool fadingToTransparent;
    [SerializeField]
    public float TimeToFade;

    float delay;

    float timer;
    float timerDelay;



    public void StartFade(float _delay, bool autoUnfade = false, float unfadeDelay = 0)
    {
        Debug.Log($"start fade");
        fadingToBlack = true;
        fadingToTransparent = false;
        timer = 0;
        timerDelay = 0;
        faded = false;
        delay = _delay;

        if (autoUnfade)
        {
            StartCoroutine(WaitAndAutoUnfade(_delay+ TimeToFade, unfadeDelay));
        }
    }

    public void StartUnFade(float _delay)
    {
        Debug.Log($"start unfade");
        fadingToBlack = false;
        fadingToTransparent = true;
        timer = 0;
        timerDelay = 0;
        faded = false;
        delay = _delay;
    }

    IEnumerator WaitAndAutoUnfade(float time, float unfadeDelay)
    {
        yield return new WaitForSeconds(time);
        StartUnFade(unfadeDelay);
    }

    void Update()
    {
        if (faded)
        {
            return;
        }
        if (fadingToBlack || fadingToTransparent)
        {

            if (timerDelay < delay)
            {
                timerDelay += Time.deltaTime;
                return;
            }
            timer += Time.deltaTime;

            if (fadingToBlack)
            {
                fade.color = new Color(fade.color.r, fade.color.b, fade.color.g, Mathf.Lerp(0, 1, timer / TimeToFade));

            }
            else if (fadingToTransparent)
            {
                fade.color = new Color(fade.color.r, fade.color.b, fade.color.g, Mathf.Lerp(1, 0, timer / TimeToFade));
            }

            if (timer >= TimeToFade)
            {
                faded = true;
            }

            
        }
    }
}
