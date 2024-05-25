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

    [SerializeField]
    float MusicVolume = 0.5f;

    private void Start()
    {
        TimeZone = TimeZone.Present;
        MusicPresent.Play();
        MusicPresent.volume = MusicVolume;
        MusicPast.volume = 0;
        MusicPast.Play();
    }

    [SerializeField]
    AudioSource MusicPresent, MusicPast;

    public void Flip()
    {


        if (TimeZone == TimeZone.Present) {
            TimeZone = TimeZone.Past;
           // MusicPlayer.Instance.PlayMusic("past_theme", loop: true, fadeIn:0.5f);
            //MusicPlayer.Instance.PlayPlaylist("past", fadeIn: 0.5f);
        } else {
            TimeZone = TimeZone.Present;

            //MusicPlayer.Instance.PlayPlaylist("main", fadeIn: 0.5f);
        }
        Music();
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
        SoundManager.Instance.Play("jump_time", ControllerGame.Player.transform);
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

    public void PlayMusic(bool play)
    {

        if (play)
        {
            MusicPresent.Play();
            MusicPast.Play();
        }
        else
        {
            MusicPresent.Stop();
            MusicPast.Stop();
        }
    }

    public void Music()
    {
        if (TimeZone == TimeZone.Present)
        {
            MusicPresent.volume = MusicVolume;
            MusicPast.volume = 0;
        }
        else
        {
            MusicPresent.volume = 0;
            MusicPast.volume = MusicVolume;
        }
      
    }

    public void UnShiftEnemies()
    {
        foreach (var c in CreaturesInTimeShift)
        {
            c.UpdateLayer(IsTimeShiftActive);
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
            c.UpdateMaskBeforeJump(true);
        }
    }

    public void AddToTimeshift(Creature c)
    {
        if (!CreaturesInTimeShift.Contains(c) && !IsJumping)
        {
            c.UpdateLayer(IsTimeShiftActive);
            CreaturesInTimeShift.Add(c);
        }
    }

    public void Reset()
    {
        CreaturesInTimeShift.Clear();
        IsJumping = false;
        IsTimeShiftActive = false;
        TimeZone = TimeZone.Present;
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
