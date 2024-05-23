using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TimeManager : MonoBehaviour
{
    public TimeZone TimeZone;


    public bool IsTimeShiftActive = false;

    List<Creature> CreaturesInTimeShift = new();

    public UnityEvent<TimeZone> OnSetGlobalTime = new UnityEvent<TimeZone>();

    bool IsJumping;

    private void Start()
    {
        TimeZone = TimeZone.Present;
    }

    public void Flip()
    {
        if (TimeZone == TimeZone.Present) {
            TimeZone = TimeZone.Past;
        } else {
            TimeZone = TimeZone.Present;
        }
        OnSetGlobalTime.Invoke(TimeZone);

        var allEntities = ControllerGame.ControllerEntities.GetEntities;

        foreach (var entity in allEntities)
        {
            var c = entity as Creature;
            if (CreaturesInTimeShift.Contains(c))
            {
                c.SetTime(TimeZone);
            }
            c.UpdateMaskAfterJump();

        }
        CreaturesInTimeShift.Clear();
        OnSetGlobalTime.Invoke(TimeZone);
        IsJumping = false;
    }

    public void BeforeJump()
    {
        IsJumping = true;
        var allEntities = ControllerGame.ControllerEntities.GetEntities;

        foreach (var entity in allEntities)
        {
            var c = entity as Creature;
            if (!CreaturesInTimeShift.Contains(c))
            {
                c.UpdateMaskBeforeJump();   
            }

        }

    }

    public void UnShiftEnemies()
    {
        foreach (var c in CreaturesInTimeShift)
        {

            c.SetTime(TimeZone);
            c.UpdateMaskAfterJump();
        }
    }

    public void ShiftEnemies()
    {

        var otherTime = TimeZone == TimeZone.Present ? TimeZone.Past : TimeZone.Present;

        foreach (var c in CreaturesInTimeShift)
        {
            if (c.CurrentTimeZone == otherTime)
            {
                continue;
            }
            c.SetTime(otherTime);
            c.UpdateMaskBeforeJump();
        }
    }

    public void AddToTimeshift(Creature c)
    {
        if (!CreaturesInTimeShift.Contains(c) && !IsJumping)
        {
            //          Debug.Log($"add {c.name} to timeshift");
            c.UpdateLayer(IsTimeShiftActive);
            CreaturesInTimeShift.Add(c);
        }
    }

    public void RemoveFromTimeshift(Creature c)
    {
        if (CreaturesInTimeShift.Contains(c) && !IsJumping)
        {
            //            Debug.Log($"remove {c.name} from timeshift");
            c.UpdateLayer();
            CreaturesInTimeShift.Remove(c);
        }
    }
}

public enum TimeZone{

    Present,
    Past

}
