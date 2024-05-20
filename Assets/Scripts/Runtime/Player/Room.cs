using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public int Id;

    [SerializeField,Disable]
    public List<EntrancePosition> Entrances;

    [SerializeField, Disable]
    public List<ExitTrigger> Exits;


    public bool HasEntrance(int id)
    {
        return Entrances.Any(x => x.Id == id);
    }

    public EntrancePosition GetEntrance(int id)
    {
        return Entrances.Find(x => x.Id == id);
    }

#if UNITY_EDITOR
    void OnValidate()
    {

            Entrances = GetComponentsInChildren<EntrancePosition>().ToList();

        //UnityEditor.EditorUtility.SetDirty(this);

        Exits = GetComponentsInChildren<ExitTrigger>().ToList();
    }
#endif
}
