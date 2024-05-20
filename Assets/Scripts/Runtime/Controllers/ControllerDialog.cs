using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerDialog : MonoBehaviour
{

    [ReorderableList]
    public List<DialogSequence> Sequences;


    [HideInInspector]

    public List<string> Triggered = new();
    public List<string> Enqueued = new();

    [SerializeField]
    GameObject Prefab, CanvasPrefab;

    [SerializeField]
    Canvas c;

    List<(Transform, Queue<DialogData>)> Targets = new();

    List<DialogSequenceInstance> instances = new();

    public void Init()
    {
        Triggered.Clear();
        ResetTargets();
        instances.Clear();
        ControllerGame.ControllerRespawn.OnPlayerRepawned.AddListener(ResetTargets);

        
    }

    public void ResetTargets()
    {
       
        Targets.Clear();
        for (int i = instances.Count - 1; i >= 0; i--)
        {
            instances[i].StopAllCoroutines();
            Destroy(instances[i].gameObject);
            instances.RemoveAt(i);
        }
        instances.Clear();
    }

    public void TriggerDialogue(string ID, Transform Target, Vector3 offset = default)
    {



        var seq = Sequences.Find(x => x.ID == ID);
        if (seq == null || (!seq.Repeatable && (Triggered.Contains(ID) || Enqueued.Contains(ID))))
        {
            return;
        }

       
            var data = new DialogData
        {
            ID = ID,
            Offset = offset
        };

        var idx = Targets.FindIndex(x => x.Item1 == Target);


        if (idx == -1)
        {
            Triggered.Add(ID);
            ControllerGame.Instance.Save();
            Targets.Add((Target, new Queue<DialogData>()));
            Spawn(seq, data.Offset, Target);
            
        }
        else
        {
            Enqueued.Add(data.ID);
            Targets[idx].Item2.Enqueue(data);
        }

    }


    void OnRemoved(Transform target)
    {
        var index = instances.FindIndex(x => x.target == target);
        if (index != -1)
        {
            instances.RemoveAt(index);
        }
        if (target == null)
        {
            for (int i = Targets.Count - 1; i >= 0; i--)
            {
                if (Targets[i].Item1 == null)
                {
                    Targets.RemoveAt(i);
                }
            }
        }
        else
        {
            var idx = Targets.FindIndex(x => x.Item1 == target);
            if (idx != -1)
            {
                if (Targets[idx].Item2.Count > 0)
                {
                    var data = Targets[idx].Item2.Dequeue();

                    Enqueued.Remove(data.ID);
                    var seq = Sequences.Find(x => x.ID == data.ID);
                    if (seq != null && (seq.Repeatable || !Triggered.Contains(data.ID)))
                    {
                        if (!seq.Repeatable)
                        {
                            Triggered.Add(data.ID);
                            ControllerGame.Instance.Save();
                        }
                        Spawn(seq, data.Offset, target);
                    }
                }
                else
                {
                    Targets.RemoveAt(idx);
                }
            }
            

        }
    }

    private void Spawn(DialogSequence seq, Vector3 offset, Transform target)
    {
        var prefab = Prefab;
        if (seq.CustomPrefab != null)
        {
            prefab = seq.CustomPrefab;
        }
        

        Transform parent = null;
        if (target != ControllerGame.Player.transform)
        {
            parent = ControllerGame.Rooms.CurrentRoom.transform;
        }
        else
        {
            parent = c.transform;
            prefab = CanvasPrefab;
        }
        var go = Instantiate(prefab, parent);

        if (prefab == CanvasPrefab)
        {
            go.transform.SetAsFirstSibling();
            (go.transform as RectTransform).anchoredPosition = new Vector2(0, -190);
        }

        var instance = go.GetComponent<DialogSequenceInstance>();
        instance.Init(seq, offset, target, OnRemoved);
        instances.Add(instance);
    }





}

public class DialogData
{
    public string ID;
    public Vector3 Offset;
}



[System.Serializable]
public class DialogSequence{

    public string ID;
    public float DefaultDelay = 1;
    public float DefaultDuration = 1;
    public float DefaultSpeed = 1;
    public bool Repeatable;
    public GameObject CustomPrefab;

    [ReorderableList]
    public List<DialogSegment> Segments;
}


[System.Serializable]
public class DialogSegment
{
    public string Text;
    [ShowIf(nameof(useCustomSettings),true)]
    public float CustomDelay = -1;
    [ShowIf(nameof(useCustomSettings), true)]
    public float CustomDuration = -1;
    [ShowIf(nameof(useCustomSettings), true)]
    public float CustomSpeed = -1;
    public bool useCustomSettings;

}