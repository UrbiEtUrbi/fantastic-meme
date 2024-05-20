using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeShiftView : MonoBehaviour
{

    [SerializeField]
    AnimationCurve AnimationCurveShow, AnimationCurveHide, AnimationCurveJump;
    [SerializeField]
    float Size;

    [SerializeField]
    float TimeToShow, TimeToHide, TimeToJump, JumpTime;

    float timer, timerJump;

    bool isShowing;

    bool isAnimating;

    bool isJumping;

    AnimationCurve InverseAnimationCurveShow, InverseAnimationCurveHide;


    private void Start()
    {
        InverseAnimationCurveShow = CreateInverse(AnimationCurveShow);
        InverseAnimationCurveHide = CreateInverse(AnimationCurveHide);

    }

    AnimationCurve CreateInverse(AnimationCurve curve)
    {
        var inverseCurve = new AnimationCurve();
        for (int i = 0; i < AnimationCurveShow.length; i++)
        {
            Keyframe inverse = new Keyframe(AnimationCurveShow.keys[i].value, AnimationCurveShow.keys[i].time);
            inverseCurve.AddKey(inverse);
        }
        return inverseCurve;
    }

    public void Show()
    {
        

        Debug.Log("show");
        if (isShowing || isJumping)
        {
            return;
        }
        isShowing = true;
        if (timer <= 0)
        {
            timer = TimeToShow;
        }
        else
        {
            timer = InverseAnimationCurveHide.Evaluate(1 - timer / TimeToHide)*TimeToShow;
        }
        timerJump = TimeToJump;
        isAnimating = true;

    }

    public void Hide()
    {
        Debug.Log("hide");
        if (!isShowing || isJumping)
        {
            return;
        }
        if (timer <= 0)
        {
            timer = TimeToHide;
        }
        else
        {
            timer = InverseAnimationCurveShow.Evaluate(1 - timer / TimeToShow) * TimeToHide;
        }
        isShowing = false;
        isAnimating = true;
    }

    private void Update()
    {

        if (isAnimating)
        {
            timer -= Time.deltaTime;
            
            if (isShowing)
            {
                transform.localScale = Vector3.one * AnimationCurveShow.Evaluate(1f - timer / TimeToShow) * Size;
            }
            else
            {
                transform.localScale = Vector3.one * AnimationCurveHide.Evaluate(1f - timer / TimeToHide) * Size;
            }

            if (timer <= 0)
            {
                isAnimating = false;
            }

        }
        if (isShowing)
        {
            timerJump -= Time.deltaTime;

            if (timerJump <= 0)
            {
                Debug.Log($"start jumping");
                isAnimating = false;

                isJumping = true;
                isShowing = false;
                timer = JumpTime;
            }
        }

        if (isJumping)
        {
            timer -= Time.deltaTime;
            transform.localScale = Vector3.one * (AnimationCurveJump.Evaluate(1f - timer / JumpTime) * 20 + Size);
            if (timer <= 0)
            {
                isJumping = false;
                isShowing = false;
                transform.localScale = default;
                FindAnyObjectByType<TilemapManager>().Flip();
            }
        }
    }



}
