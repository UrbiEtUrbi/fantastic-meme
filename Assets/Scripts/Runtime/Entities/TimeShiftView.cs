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
    float rotationSpeed;

    [SerializeField]
    float TimeToShow, TimeToHide, TimeToJump, JumpTime;

    float timer, timerJump;

    bool isShowing;

    bool isAnimating;

    bool isJumping;

    [SerializeField]
    GameObject colliderObject;

    

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
        if (isShowing || isJumping)
        {
            return;
        }
        isShowing = true;
        ControllerGame.TimeManager.IsTimeShiftActive = true;
        if (timer <= 0)
        {
            timer = TimeToShow;
        }
        else
        {
            ControllerGame.TimeManager.UnShiftEnemies();
            timer = InverseAnimationCurveHide.Evaluate(1 - timer / TimeToHide)*TimeToShow;
        }
        timerJump = TimeToJump;
        isAnimating = true;

    }

    public void Hide()
    {
        if (!isShowing || isJumping)
        {
            return;
        }
        if (timer <= 0)
        {
            ControllerGame.TimeManager.ShiftEnemies();
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
                var t = 1f - timer / TimeToShow;
                transform.Rotate(new Vector3(0, 0, t*Time.deltaTime * rotationSpeed));
                transform.localScale = Vector3.one * AnimationCurveShow.Evaluate(1f - timer / TimeToShow) * Size;
            }
            else
            {
                var t = 1f - timer / TimeToHide;
                transform.Rotate(new Vector3(0, 0, -(1-t)*Time.deltaTime * rotationSpeed));
                transform.localScale = Vector3.one * AnimationCurveHide.Evaluate(1f - timer / TimeToHide) * Size;
            }


            if (timer <= 0)
            {
                if (!isShowing)
                {
                   
                    ControllerGame.TimeManager.IsTimeShiftActive = false;
                }
                isAnimating = false;
            }

        }
        if (isShowing)
        {
            transform.Rotate(new Vector3(0, 0, (timerJump / TimeToJump) *Time.deltaTime * rotationSpeed));
            timerJump -= Time.deltaTime;

            if (timerJump <= 0)
            {

                isAnimating = false;

                isJumping = true;
                isShowing = false;
                timer = JumpTime;
                ControllerGame.TimeManager.BeforeJump();
               
                colliderObject.SetActive(false);
            }
           
        }

        if (isJumping)
        {
            transform.Rotate(new Vector3(0, 0, -2*Time.deltaTime * rotationSpeed));
            timer -= Time.deltaTime;
            transform.localScale = Vector3.one * (AnimationCurveJump.Evaluate(1f - timer / JumpTime) * 20 + Size);
            if (timer <= 0)
            {
                isJumping = false;
                isShowing = false;
                transform.localScale = default;
                ControllerGame.TimeManager.IsTimeShiftActive = false;
                ControllerGame.TimeManager.Flip();
                colliderObject.SetActive(true);
            }
        }
    }



}
