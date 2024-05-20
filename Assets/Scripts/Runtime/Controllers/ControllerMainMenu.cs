using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMainMenu : ControllerLocal
{




    [SerializeField]
    GameObject pipLeft, pipRight;

    bool left;

    [SerializeField]
    GameObject Cover;

    [SerializeField, SceneDetails]
    SerializedScene Scene;


    public override void Init()
    {
        base.Init();
        Confirm(true);

        //Cover.gameObject.SetActive(!ControllerLoadingScene.Instance.HasSave);

        //if (ControllerLoadingScene.Instance.HasSave) {
        //    left = false;
        //    pipLeft.gameObject.SetActive(false);
        //    pipRight.gameObject.SetActive(true);
        //} else {
        //    left = true;
        //    pipLeft.gameObject.SetActive(true);
        //    pipRight.gameObject.SetActive(false);
        //}

        //ControllerInput.Instance.Horizontal.AddListener(Horizontal);

        //ControllerInput.Instance.Jump.AddListener(Confirm);
        //SoundManager.Instance.PlayLooped("menu");
      
    }



    void Horizontal(float amount)
    {
        if (!ControllerLoadingScene.Instance.HasSave)
        {
            return;
        }
        if (amount > 0)
        {
            if (left)
            {

                left = false;
                pipLeft.gameObject.SetActive(false);
                pipRight.gameObject.SetActive(true);
                SoundManager.Instance.Play("jump");
            }
           
        }
        else if (amount < 0)
        {
            if (!left)
            {

                left = true;
                pipLeft.gameObject.SetActive(true);
                pipRight.gameObject.SetActive(false);
                SoundManager.Instance.Play("jump");
            }
            
        }
    }

    private void Confirm(bool a)
    {

       
        if (a)
        {
            SoundManager.Instance.CancelAllLoops();
            if (left)
            {
                ControllerLoadingScene.Instance.SaveData = null;
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

            ControllerGameFlow.Instance.LoadNewScene(Scene.BuildIndex);
        }

    }
}
