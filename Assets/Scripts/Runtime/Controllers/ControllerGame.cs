using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using System.Linq;

public class ControllerGame : ControllerLocal
{

    public static ControllerGame Instance;


    [BeginGroup("Prefabs")]
    [EndGroup]
    [SerializeField]
    Player PlayerPrefab;


    [BeginGroup("References")]
    [SerializeField]
    [EndGroup]
    CinemachineVirtualCamera VCamera;

    [BeginGroup("Settings")]
    [EndGroup]
    [SerializeField]
    Vector3 StartPosition = new Vector3(-9.7f, -5.4f, 0);


    public CinemachineVirtualCamera GetCCamera => VCamera;
    private CinemachineConfiner2D Confiner;


    [HideInInspector]
    public static Player Player => Instance.player;

    Player player;

    [Hide]
    public bool IsGameOver;

    [Hide]
    public bool IsGamePlaying;


    public bool AcceptInput => !IsGameOver && IsGamePlaying;

    #region Controllers
    ControllerEntities m_ControllerEntities;
    public static ControllerEntities ControllerEntities => Instance.m_ControllerEntities;


    ControllerRespawn m_ControllerRespawn;
    public static ControllerRespawn ControllerRespawn => Instance.m_ControllerRespawn;


    ControllerAttack m_ControllerAttack;
    public static ControllerAttack ControllerAttack => Instance.m_ControllerAttack;

    ControllerRooms m_ControllerRooms;
    public static ControllerRooms Rooms => Instance.m_ControllerRooms;

    ControllerPickups m_ControllerPickups;
    public static ControllerPickups ControllerPickups => Instance.m_ControllerPickups;

    ControllerDialog m_ControllerDialog;
    public static ControllerDialog ControllerDialog => Instance.m_ControllerDialog;


    TimeManager m_TimeManager;
    public static TimeManager TimeManager => Instance.m_TimeManager;

    #endregion

    [SerializeField]
    TMP_Text CabbageLabel;
    
    [SerializeField]
    HpView HpView;

    [SerializeField]
    int PlayerMaxHealthStarting = 2;


    int currentMaxHealth;
    int currentMaxStamina;
    int currenthp;
    int currentStamina;

    public bool HasStick;
    public bool HasSpike;
    public bool HasIceMelee;

    [SerializeField]
    int initRoom, initEntrance;

    public static bool Initialized
    {
        get
        {
            if (!Instance)
            {
                return false;

            }
            else
            {
                return Instance.isInitialized;
            }
        }

    }

    public override void Init()
    {
       
     

        m_ControllerEntities = GatherComponent<ControllerEntities>();
        m_ControllerRespawn = GatherComponent<ControllerRespawn>();

        m_ControllerAttack = GetComponent<ControllerAttack>();
        m_ControllerRooms = GetComponent<ControllerRooms>();
        m_ControllerPickups = GatherComponent<ControllerPickups>();
        m_ControllerDialog = GetComponent<ControllerDialog>();
        m_TimeManager = GetComponent<TimeManager>();

        if (ControllerLoadingScene.Instance.HasSave)
        {
            var save = ControllerLoadingScene.Instance.SaveData;
            currentMaxHealth = save.MaxHp;
            currenthp = save.CurrentHP;
            currentStamina = save.CurrentStamina;
            currentMaxStamina = save.MaxStamina;
            HasSpike = save.HasSpike;
            HasStick = save.HasStick;
            HasIceMelee = save.HasIceMelee;
            initRoom = save.Room;
            initEntrance = save.Entrance;

            foreach (var t in save.Dialogues)
            {
                m_ControllerDialog.Triggered.Add(t);
            }

            foreach (var pickup in save.Pickups)
            {
                m_ControllerPickups.PickedUp.Add(pickup);
            }

        }
        else
        {
 #if !UNITY_EDITOR
            currentMaxHealth = 1;
            currenthp = 1;
            currentStamina = 0;
            currentMaxStamina = 0;
            HasSpike = false;
            HasStick = false;
            HasIceMelee = false;
            initRoom = 0;
            initEntrance = 0;
#endif
        }
        Instance = this;



        // MusicPlayer.Instance.PlayPlaylist("overworld");

        

        base.Init();
        CabbageLabel.SetText($"0x");
        StartCoroutine(WaitForSceneLoad());
    }

    public void AssignConfiner(PolygonCollider2D polygonCollider2D)
    {
        if (Confiner == null)
        {
            Confiner = VCamera.GetComponent<CinemachineConfiner2D>();
        }
        if (polygonCollider2D == null)
        {
            Confiner.enabled = false;
        }
        else
        {
            Confiner.enabled = true;
            Confiner.m_BoundingShape2D = polygonCollider2D;

        }
        Confiner.InvalidateCache();
    }
    public int cabbageCount;
    public void CollectCabbage()
    {
        cabbageCount++;
        CabbageLabel.SetText($"{cabbageCount}x");
        CheckWin();
    }
    public void ResetCabbage()
    {
        cabbageCount = 0;
        slugCabbage = 0;
        CabbageLabel.SetText($"{cabbageCount}x");
    }

    IEnumerator WaitForSceneLoad() {
        yield return null;
        player = Instantiate(PlayerPrefab);
        SetPlayerMaxHealth(PlayerMaxHealthStarting+currentMaxHealth);
        player.ChangeHealth(PlayerMaxHealthStarting+currentMaxHealth);
        Rooms.InitialRoom(initRoom,initEntrance);
        VCamera.Follow = player.transform;


       // MusicPlayer.Instance.PlayMusic("main_theme", loop: true);
       //MusicPlayer.Instance.PlayPlaylist("main");

    }

    public void SetPlayerMaxHealth(int amount)
    {
        player.SetInitialHealth(amount);
        HpView.SetMaxHealth(amount);
    }
    
    public void IncreaseMax(int amount)
    {
        currentMaxHealth += amount;
        player.SetInitialHealth(PlayerMaxHealthStarting+currentMaxHealth);
        HpView.SetMaxHealth(PlayerMaxHealthStarting+ currentMaxHealth);
    }

    public void UpdateHealthUI()
    {
        HpView.UpdateHealth(player.CurrentHealth);

    }

    public void GameOver()
    {
        ResetCabbage();
        SoundManager.Instance.Play("game_over");
        IsGameOver = true;
        IsGamePlaying = false;
        Rooms.OnGameOver();  
    }
    int slugCabbage = 0;

    public void SlugEatCabbage()
    {
        slugCabbage++;
        CheckWin();

    }
    List<CabbagePlot> cabbages;
    public void CheckWin()
    {
        if (m_ControllerRooms.CurrentRoom.Id == 1)
        {
            if (cabbages == null || cabbages.Count == 0)
            {
                cabbages = FindObjectsOfType<CabbagePlot>().ToList();
            }
        }
        if (cabbages.Count == slugCabbage + cabbageCount)
        {
            Win();
        }
    }

    public void Win()
    {
        IsGameOver = true;
        IsGamePlaying = false;

       
        SoundManager.Instance.PlayDelayed("victory", 1f);
       
        Invoke(nameof(WinDelayed), 1f);
        Invoke(nameof(MusicDelayed), 4f);

    }

    void WinDelayed()
    {
        TimeManager.PlayMusic(false);
        Rooms.OnWin();
    }

    void MusicDelayed()
    {
        TimeManager.PlayMusic(true);
    }

    public void Continue()
    {

       
        SetPlayerMaxHealth(PlayerMaxHealthStarting+ currentMaxHealth);
        player.ChangeHealth(PlayerMaxHealthStarting+ currentMaxHealth);
       
        IsGameOver = false;
        IsGamePlaying = true;
    }

    public void Save()
    {
        var savedata = new SaveData
        {
            HasIceMelee = HasIceMelee,
            HasSpike = HasSpike,
            HasStick = HasStick,
            Entrance = Rooms.CurrentEntrance,
            Room = Rooms.CurrentRoom.Id,
            Dialogues = m_ControllerDialog.Triggered,
            Pickups = m_ControllerPickups.PickedUp,
            CurrentHP = player.CurrentHealth,
            MaxHp = currentMaxHealth,
            CurrentStamina = 0,
            MaxStamina = 0
        };
        ControllerLoadingScene.Instance.Save(savedata);

    }

  

    void Reload()
    {
    }

    public void PlayerDie()
    {
        //reload scenes or entities
    }

    public T GatherComponent<T>() where T : MonoBehaviour {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
}
