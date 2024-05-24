using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationView : MonoBehaviour
{
    [SerializeField]
    float ActivationDistance;

    [SerializeField]
    float rotationSpeed, showTime;

    float showTimer = 0;

    bool show, isAnimating;


    Vector3 startScale;

    [SerializeField]
    AnimationCurve AnimationCurve;

    public PickupHandler Handler;

    private void Awake()
    {
        startScale = transform.localScale;
        transform.localScale = default;
    }

    

    private void Update()
    {
        if (Handler.IsInteractible && Handler.CanInteract(ControllerGame.Player) && Vector3.Distance(ControllerGame.Player.transform.position, transform.position) < ActivationDistance)
        {
            Show();
        }
        else
        {
            Hide();
        }

        if (isAnimating)
        {
            var t = AnimationCurve.Evaluate(1 - showTimer / showTime);
            if (show)
            {
                showTimer -= Time.deltaTime;

                
                transform.localScale = Vector3.LerpUnclamped(default, startScale, t);
            }
            else
            {
                showTimer -= Time.deltaTime;
                transform.localScale = Vector3.LerpUnclamped(startScale, default, t);
            }
            if (showTimer <= 0) {
                isAnimating = false;
            }
        }
        if (show)
        {
            transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
        }
    }

    void Show()
    {
        if (show)
        {
            return;
        }
        show = true;
        isAnimating = true;
        showTimer = showTime;
    }

    void Hide()
    {
        if (!show)
        {
            return;
        }
        show = false;
        isAnimating = true;
        showTimer = showTime;
    }
}
