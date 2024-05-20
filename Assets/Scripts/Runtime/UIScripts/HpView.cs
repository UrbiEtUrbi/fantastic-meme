using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpView : MonoBehaviour
{
    [SerializeField]
    GameObject HeartViewPrefab;


    List<Animator> Hearts = new List<Animator>();


    List<bool> HealthStatus = new();

    [EditorButton(nameof(SetMax))]
    [SerializeField]
    int Max;


    [EditorButton(nameof(SetCurrent))]
    [SerializeField]
    int Current;


    bool firstSetMax = true;
    public void SetMaxHealth(int max)
    {

        if (HeartViewPrefab == null)
        {
            return;
        }
        for (int i = 0; i < max; i++)
        {
            if (Hearts.Count-1 < i)
            {
                if (firstSetMax)
                {
                    Hearts.Add(Instantiate(HeartViewPrefab, transform).GetComponent<Animator>());
                    HealthStatus.Add(true);
                }
                else
                {
                    Hearts.Insert(0,Instantiate(HeartViewPrefab, transform).GetComponent<Animator>());
                    Hearts[0].transform.SetAsFirstSibling();
                    HealthStatus.Insert(0,true);
                }
            }
        }
        firstSetMax = false;
    }

    public void UpdateHealth(int current)
    {
        Debug.Log($"update health");
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (HealthStatus[i] && current <= i)
            {
                HealthStatus[i] = false;
                Hearts[i].SetTrigger("LoseHeart");
            }
            else if (!HealthStatus[i] && current > i)
            {
                HealthStatus[i] = true;
                Hearts[i].SetTrigger("GainHeart");

            }

        }

    }

    void SetCurrent() {
        UpdateHealth(Current);
    }

    void SetMax()
    {
        SetMaxHealth(Max);
    }
}
