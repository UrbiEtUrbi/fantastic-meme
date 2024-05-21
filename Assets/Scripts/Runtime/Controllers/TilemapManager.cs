using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField]
    List<TilemapRenderer> Past;

    [SerializeField]
    List<TilemapRenderer> Present;

   


    private void Start()
    {


        ControllerGame.TimeManager.OnSetGlobalTime.AddListener(Flip);

        for (int i = 0; i < Past.Count; i++)
        {
            Past[i].sortingOrder = i;
            Past[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        for (int i = 0; i < Present.Count; i++)
        {
            Present[i].sortingOrder = i+Past.Count;
            Present[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }

    public void Flip(TimeZone time)
    {
        if (time == TimeZone.Past)
        {
            for (int i = 0; i < Past.Count; i++)
            {
                Past[i].sortingOrder = i + Present.Count;
                Past[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }

            for (int i = 0; i < Present.Count; i++)
            {
                Present[i].sortingOrder = i;
                Present[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }
        else
        {
            for (int i = 0; i < Past.Count; i++)
            {
                Past[i].sortingOrder = i;
                Past[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            for (int i = 0; i < Present.Count; i++)
            {
                Present[i].sortingOrder = i + Past.Count;
                Present[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
        }
    }

  
}
