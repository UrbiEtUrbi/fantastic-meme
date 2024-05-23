using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class PickupHandler : MonoBehaviour
{

    public PickupType PickupType;
    string sound;

    [SerializeField]
    float PickupTime;
    float timer;

    [SerializeField]
    float RespawnTime;

    [SerializeField]
    float CollectTime;

    bool isPickingUp;

    protected IPickupCollector CurrentlyAttending;

    [SerializeField]
    bool DestroyOnPickup;


    [SerializeField]
    Image Bar;

    public bool IsInteractible = true;


    [SerializeField]
    TimeShiftBehaviour TimeShiftBehaviour;


    [SerializeField]
    TimeZone StartTimeZone;

    [SerializeField]
    protected List<SpriteRenderer> spriteRenderers;


    public UnityEvent OnInteracted = new UnityEvent();

    [HideInInspector]
    public GameObject View;

    private void Awake()
    {
        Bar.gameObject.SetActive(false);
    }

    public virtual void Spawn()
    {

        IsInteractible = true;
        var item = ControllerGame.ControllerPickups.Pickup(PickupType);
        item.transform.SetParent(transform);
        item.transform.localPosition = default;
        spriteRenderers.Add(item.GetComponentInChildren<SpriteRenderer>());
        View = item.gameObject;
        SetGlobalTime(ControllerGame.TimeManager.TimeZone);
    }

    private void Start()
    {
        ControllerGame.TimeManager.OnSetGlobalTime.AddListener(SetGlobalTime);
        SetGlobalTime(ControllerGame.TimeManager.TimeZone);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurrentlyAttending != null)
        {
            return;
        }
        CheckAndStartPickup(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CurrentlyAttending == null)
        {
            return;
        }
        var pickerExited = collision.gameObject.GetComponent<IPickupCollector>();
        if (pickerExited == CurrentlyAttending)
        {
            StopPickingUp();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CurrentlyAttending != null)
        {
            return;
        }
        CheckAndStartPickup(collision);
    }

    void CheckAndStartPickup(Collider2D collision)
    {

        var candidate = collision.gameObject.GetComponent<IPickupCollector>();

        if (candidate == null)
        {
            return;
        }
        bool timeConditionOk = false;

        switch (TimeShiftBehaviour)
        {
            case TimeShiftBehaviour.None:
                timeConditionOk = true;
                break;

            case TimeShiftBehaviour.OnlyOne:

                bool sameTimeZone = StartTimeZone == candidate.GetTimeZone;
                timeConditionOk = sameTimeZone ^ ControllerGame.TimeManager.IsTimeShiftActive;
                break;
        }
        
        if (timeConditionOk && candidate != null && CanInteract(candidate))
        {
            CurrentlyAttending = candidate;
            if (IsInteractible)
            {
                if (CanInteract(candidate))
                {
                    timer = PickupTime;
                    isPickingUp = true;
                    Bar.fillAmount = 0;
                    Bar.gameObject.SetActive(true);
                }
            }
        }
    }

  
    protected virtual bool CanInteract(IPickupCollector pickupCollector)
    {

        return false;
    }

    protected virtual void OnTimerEnd()
    {
        OnInteracted.Invoke();
        if (View != null)
        {
            spriteRenderers.Remove(View.GetComponent<SpriteRenderer>());
            Destroy(View.gameObject);
        }
    }

    protected void SetGlobalTime(TimeZone zone)
    {
        if (StartTimeZone != zone)
        {
            StopPickingUp();
        }
        Bar.fillAmount = 0;
        switch (TimeShiftBehaviour)
        {
            case TimeShiftBehaviour.None:
                break;

            case TimeShiftBehaviour.OnlyOne:

                foreach (var s in spriteRenderers)
                {
                    s.maskInteraction = StartTimeZone == zone ? SpriteMaskInteraction.VisibleOutsideMask : SpriteMaskInteraction.VisibleInsideMask;
                }
                break;
        }
    }


    void StopPickingUp()
    {
        isPickingUp = false;
        CurrentlyAttending = null;
    }

    private void Update()
    {
        if (isPickingUp && IsInteractible)
        {
            timer -= Time.deltaTime;

            if (!((StartTimeZone == ControllerGame.TimeManager.TimeZone) ^ ControllerGame.TimeManager.IsTimeShiftActive))
            {
                isPickingUp = false;
                CurrentlyAttending = null;
                return;
            }


            Bar.fillAmount = 1 - timer / PickupTime;
            if (timer <= 0)
            {


                isPickingUp = false;
                IsInteractible = false;
                timer = RespawnTime;

                OnTimerEnd();

                SoundManager.Instance.Play(sound);
                if (DestroyOnPickup)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        if (!IsInteractible)
        {
            timer -= Time.deltaTime;
            Bar.fillAmount = timer / RespawnTime;
            if (timer <= 0 && RespawnTime > 0)
            {
                IsInteractible = true;
            }
        }
    }
}

public enum TimeShiftBehaviour
{
    None,
    OnlyOne
}