using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControllerRooms : MonoBehaviour
{

    //rooms

    [SerializeField]
    SpriteAnimator transitionIn;

    [SerializeField]
    SpriteAnimator transitionOut;

    public Room CurrentRoom => m_CurrentRoom;
    Room m_CurrentRoom;
    EntrancePosition m_CurrentEntrance;

    [SerializeField]
    List<Room> m_Rooms;

    int nextEntrance;

    int currentEntranceId;
    public int CurrentEntrance => currentEntranceId;
    Room NextRoom;

    [SerializeField]
    RectTransform GameOverText;

    Vector2 GameOverTextStartPosition;

    [SerializeField,SceneDetails]
    SerializedScene Scene;

    Vector2 WinTextStartPosition;
    [SerializeField]
    RectTransform WinText;

    private void Start()
    {
        GameOverTextStartPosition = GameOverText.anchoredPosition;
        WinTextStartPosition = WinText.anchoredPosition;
    }


    public IEnumerator LoadRoom(int room, int entrance)
    {

        NextRoom = m_Rooms.Find(x => x.Id == room);
        nextEntrance = entrance;
        if (NextRoom && NextRoom.HasEntrance(nextEntrance))
        {
           yield return StartCoroutine(LoadingCoroutine());
        }
    }

    public void InitialRoom(int room, int entrance)
    {
        NextRoom = m_Rooms.Find(x => x.Id == room);
        nextEntrance = entrance;
        StartCoroutine(FirstLoad());
    }



    public void OnGameOver()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    public void OnWin()
    {
        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2);
        transitionIn.gameObject.SetActive(true);
        transitionIn.Reset(false);
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        float duration = 3f;

        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            var pos   = Vector2.Lerp(WinTextStartPosition, default, time / duration);
            WinText.anchoredPosition = new Vector2(Mathf.Round(pos.x), pos.y);
            time += 0.05f;

            if (time > duration)
            {
                break;
            }

        }
        yield return new WaitForSeconds(30);
        WinText.anchoredPosition = WinTextStartPosition;
        ControllerLoadingScene.Instance.Load();

       
        ControllerGameFlow.Instance.LoadNewScene(Scene.BuildIndex);


    }


    IEnumerator RestartGameCoroutine()
    {
        yield return new WaitForSeconds(2);
        transitionIn.gameObject.SetActive(true);
        transitionIn.Reset(false);
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        float duration = 1f;
        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            var pos = Vector2.Lerp(GameOverTextStartPosition, default, time / duration);
            GameOverText.anchoredPosition = new Vector2(pos.x, Mathf.Round(pos.y));
            time += 0.05f;

            if (time > duration)
            {
                break;
            }
            
        }
        ControllerGame.Player.Revive();
        yield return new WaitForSeconds(1);
        GameOverText.anchoredPosition = GameOverTextStartPosition;
        yield return LoadRoom(m_CurrentRoom.Id, m_CurrentEntrance.Id);
        ControllerGame.Instance.Continue();


    }

    IEnumerator LoadingCoroutine()
    {


        ControllerGame.Instance.IsGamePlaying = false;
        SoundManager.Instance.Play("room_transition_in");
        if (!transitionIn.gameObject.activeSelf)
        {
            transitionIn.gameObject.SetActive(true);
            transitionIn.Reset(false);
       


        yield return new WaitUntil(() => !transitionIn.IsAnimating);
        }
        Destroy(m_CurrentRoom.gameObject);

        Load();
        
        yield return new WaitForSeconds(0.5f);

        
        transitionIn.gameObject.SetActive(false);
        yield return LoadNext();
    }

    void Load()
    {
        m_CurrentRoom = Instantiate(NextRoom);

       
        m_CurrentEntrance = m_CurrentRoom.GetEntrance(nextEntrance);
        currentEntranceId = nextEntrance;
        ControllerGame.Instance.Save();
        ControllerGame.Instance.AssignConfiner(m_CurrentRoom.GetComponentInChildren<PolygonCollider2D>());

      //  m_CurrentRoom.GetComponentInChildren<ParalaxBkg>().Init(ControllerGame.Instance.GetCCamera.transform);
        ControllerGame.Player.transform.position = m_CurrentEntrance.transform.position;
    }

    IEnumerator LoadNext()
    {


        transitionOut.gameObject.SetActive(true);
        transitionOut.Reset(false);
        SoundManager.Instance.Play("room_transition_out");
        yield return new WaitUntil(() => !transitionOut.IsAnimating);
        transitionOut.gameObject.SetActive(false);

        ControllerGame.Instance.IsGamePlaying = true;
        yield return false;
    }

    IEnumerator FirstLoad()
    {

        Load();
        yield return LoadNext();
    }
#if UNITY_EDITOR

    private void OnValidate()
    {
        List<Room> rooms = new();
        UnityEditor.AssetDatabase.FindAssets("t:Prefab room").ToList()
            .ForEach(x =>
            rooms.Add(
                UnityEditor.AssetDatabase.LoadAssetAtPath<Room>(
                    UnityEditor.AssetDatabase.GUIDToAssetPath(x)
                    )
                )
            );

        foreach (var r in rooms)
        {
            if (r == null)
            {
                continue;
            }
            if (r.Id != -1)
            {
                if (!m_Rooms.Contains(r))
                {
                    m_Rooms.Add(r);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }
        }


        foreach (var r in m_Rooms)
        {
            foreach (var exit in r.Exits)
            {
                var room = m_Rooms.Find(x => x.Id == exit.Room);
                if (room == null)
                {
                    Debug.LogError($"Room {r.name} has Exit to room {exit.Room} but that room doesn't exist", r);
                }
                if (!room.HasEntrance(exit.Entrance))
                {
                    Debug.LogError($"Room {r.name} has Exit to room {exit.Room} entrance {exit.Entrance} but that entrance doesnt exist", r);
                }
            }
        }
    }

#endif

    public void CheckRooms()
    {
        bool canBuild = true;
        foreach (var r in m_Rooms)
        {
            foreach (var exit in r.Exits)
            {
                var room = m_Rooms.Find(x => x.Id == exit.Room);
                if (room == null)
                {
                    canBuild = false;
                    Debug.LogError($"Room {r.name} has Exit to room {exit.Room} but that room doesn't exist", r);
                }
                if (!room.HasEntrance(exit.Entrance))
                {
                    canBuild = false;
                    Debug.LogError($"Room {r.name} has Exit to room {exit.Room} entrance {exit.Entrance} but that entrance doesnt exist", r);
                }
            }
        }
        if (!canBuild)
        {
            throw new System.Exception("Something wrong with entrance and exit");
        }
    }


}
