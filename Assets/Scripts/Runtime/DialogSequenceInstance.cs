using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogSequenceInstance : MonoBehaviour
{


    [SerializeField]
    Vector3 offset;
    DialogSequence sequence;
    public Transform target;

    int currentSegment = 0;
    float currentSpeed = 0;
    float currentDelay = 0;
    float currentDuration = 0;
    bool currentSegmentDone = false;

    [SerializeField]
    TMP_Text Text;

    UnityAction<Transform> Action;

    [SerializeField]
    RectTransform Container;

    float startSize;

    bool invoked = false;

    string currentText;
    public void Init(DialogSequence _sequence, Vector3 _offset, Transform _target, UnityAction<Transform> action)
    {



        if (_target != ControllerGame.Player.transform)
        {
            transform.position = _target.position + _offset;
        }
        offset = _offset;
        sequence = _sequence;
        target = _target;
        Action = action;

        currentSegmentDone = false;

        PopuplateValues();
        startSize = Container.sizeDelta.x;
        Container.sizeDelta = new Vector2(0, Container.sizeDelta.y);
        StartCoroutine(SequenceHandler());

    }

    private void Update()
    {


        return;

       

        var pos = Camera.main.WorldToScreenPoint(transform.position);




        var remX = pos.x % 10f;
        var mid = 5f;
        if (remX >= 4.66f && remX <= 5.33f)
        {
            if (remX < mid)
            {
                transform.position = new Vector3((int)transform.position.x + 0.466f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3((int)transform.position.x + 0.5328f, transform.position.y, transform.position.z);
            }
        }
        
        var remY = pos.y % 10f;
        if (remY >= 4.66f && remY <= 5.33f)
        {
            if (remY < 5)
            {
                transform.position = new Vector3(transform.position.x, (int)transform.position.y + 0.466f, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, (int)transform.position.y + 0.5328f, transform.position.z);
            }
        }
    }

    void PopuplateValues()
    {
        if (sequence.Segments[currentSegment].useCustomSettings)
        {
            currentSpeed = sequence.Segments[currentSegment].CustomSpeed;
            currentDuration = sequence.Segments[currentSegment].CustomDuration;
            currentDelay = sequence.Segments[currentSegment].CustomDelay;
        }
        else
        {
            currentSpeed = sequence.DefaultSpeed;
            currentDuration = sequence.DefaultDuration;
            currentDelay = sequence.DefaultDelay;
        }

    }

    IEnumerator SequenceHandler()
    {
        int currentCharacter = 0;


        bool animateEntry = true;
        while (true)
        {
            yield return new WaitForSeconds(currentDelay);

            if (animateEntry)
            {
                animateEntry = false;
                float timer1 = 0;
                while (true)
                {
                    var timez = Time.deltaTime;
                    timer1 += timez;
                    Container.sizeDelta = new Vector2(startSize * (timer1 / 0.5f), Container.sizeDelta.y);
                    if (timer1 > 0.5f)
                    {
                        Container.sizeDelta = new Vector2(startSize, Container.sizeDelta.y);
                        break;
                    }
                    yield return new WaitForSeconds(timez);

                }
            }

            SoundManager.Instance.Play("chat");
            while (true)
            {

               
                if (currentCharacter < sequence.Segments[currentSegment].Text.Length)
                {
                    if (sequence.Segments[currentSegment].Text[currentCharacter] == '<')
                    {

                        while (sequence.Segments[currentSegment].Text[currentCharacter] != '>')
                        {
                            currentCharacter++;
                        }
                        currentCharacter++;
                    }
                }
                currentText = sequence.Segments[currentSegment].Text.Substring(0, currentCharacter);
                Text.text = currentText;
                yield return new WaitForSeconds(currentSpeed);
                currentCharacter++;
                if (currentCharacter >= sequence.Segments[currentSegment].Text.Length+1){
                    break;
                }
                
            }

            float time = 0;

            while (time < currentDuration)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }


           
            currentSegment++;
            currentCharacter = 0;
            if (currentSegment >= sequence.Segments.Count)
            {

                Text.text = string.Empty;
                float timer2 = 0;
                while (true)
                {
                    var time2 = Time.deltaTime;
                    timer2 += time2;
                    Container.sizeDelta = new Vector2(startSize * ( 1 - (timer2 / 0.3f)), Container.sizeDelta.y);
                    if (timer2 > 0.3f)
                    {
                        Container.sizeDelta = new Vector2(0, Container.sizeDelta.y);
                        break;
                    }
                    yield return new WaitForSeconds(time2);

                }


                Action.Invoke(target);
                invoked = true;
                Destroy(gameObject);
                break;
            }
            PopuplateValues();
            Text.text = string.Empty;
        }


    }

    private void OnDestroy()
    {
        if (!invoked)
        {
            Action.Invoke(target);
        }
    }


}
