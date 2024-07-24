using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryGrid : SGrid
{
    public static System.EventHandler<System.EventArgs> OnRestartMilitary;

    private bool isRestarting;

    public MilitarySpriteTable militarySpriteTable; // global reference
    
    [SerializeField] private Transform playerRestartSpawnPosition;
    [SerializeField] private List<MilitaryUnspawnedAlly> unspawnedAllies; // dont reset one on #16

    [SerializeField] private MilitaryResetChecker militaryResetChecker; // init order makes me cry
    
    [SerializeField] private GameObject allSlidersCollectible;

    public override void Init()
    {
        militaryResetChecker.Init(); // set up singleton
        InitArea(Area.Military);
        base.Init();
    }

    protected override void Start()
    {
        base.Start();

        AudioManager.PlayMusic("Military");

        if (unspawnedAllies.Count != 15)
        {
            Debug.LogWarning("Unspawned allies list should be 15 long.");
        }
        
        // if (
        //     !SaveSystem.Current.GetBool(MilitaryWaveManager.BEAT_ALL_ALIENS_STRING) &&
        //     PlayerInventory.Contains("Slider 1", Area.Military)
        // )
        if (!SaveSystem.Current.GetBool(MilitaryWaveManager.BEAT_ALL_ALIENS_STRING))
        {
            // Debug.LogWarning($"[Military] Joined area without finishing! Resetting Military...");
            DoRestartSimulation();
        }
        
        if (
            SaveSystem.Current.GetBool(MilitaryWaveManager.BEAT_ALL_ALIENS_STRING) &&
            !HasAllTiles()
        )
        {
            Debug.LogWarning($"[Military] Joined area without collecting all tiles.");
            allSlidersCollectible.SetActive(true);
        }
    }

    private void OnEnable()
    {
        SGridAnimator.OnSTileMoveEndLate += OnTileMove;
    }

    private void OnDisable()
    {
        SGridAnimator.OnSTileMoveEndLate -= OnTileMove;
        
        // Can cause some issues with the order things are disabled/destroyed
        // if (!SaveSystem.Current.GetBool(MilitaryWaveManager.BEAT_ALL_ALIENS_STRING))
        // {
        //     Debug.LogWarning($"[Military] Quit area without finishing! Resetting Military...");
        //     DoRestartSimulation(updatePlayer: false);
        // }

        UIEffects.DisablePixel();
    }

    public override void Save()
    {
        base.Save();
    }

    public override void Load(SaveProfile profile)
    {
        base.Load(profile);
    }


    // === Military puzzle specific ==

    public void RestartSimulation() => RestartSimulation(0.25f);

    public void RestartSimulation(float speed)
    {
        if (isRestarting)
            return;
        isRestarting = true;
        PauseManager.AddPauseRestriction(gameObject);
        AudioManager.Play("Slide Rumble"); 
        
        MilitaryMusicController.DoLoseTrigger();
        UIEffects.Pixelize(
            () => {

                SaveSystem.Current.SetBool("militaryFailedOnce", true);
                SaveSystem.Current.SetInt("militaryAttempts", SaveSystem.Current.GetInt("militaryAttempts", 0) + 1);

                DoRestartSimulation();
                AudioManager.Play("TFT Bell");
                MilitaryMusicController.SetMilitaryLevel(0);
            },
            () => {
                isRestarting = false;
                PauseManager.RemovePauseRestriction(gameObject);
            }, 
            speed
        );
    }

    private void DoRestartSimulation()
    {
        Debug.Log("[Military] Restart sim!");
        OnRestartMilitary?.Invoke(this, new System.EventArgs());

        if (Player.GetInstance().GetSTileUnderneath() != null)
        {
            Player.SetPosition(playerRestartSpawnPosition.position);
            Player.SetParent(null);
        }
        DisableSliders();

        RestartTroops();

        MilitaryCollectibleController.Reset();
        MilitaryWaveManager.Reset();
        MilitaryResetChecker.ResetCounters();
        MilitaryTurnAnimator.Reset();

        SaveSystem.SaveGame("Finished Restarting Military Sim");
    }

    private void DisableSliders()
    {
        foreach (STile s in grid)
        {
            UIArtifact.SetButtonComplete(s.islandId, false);
            gridTilesExplored.SetTileExplored(s.islandId, false);
            if (s.isTileActive)
            {
                s.SetTileActive(false);
                UIArtifact.GetInstance().RemoveButton(s);
            }
        }

        if (GetStileAt(0, 3).islandId != 1)
        {
            SwapTiles(GetStileAt(0, 3), GetStile(1));
        }
        
        PlayerInventory.RemoveCollectible(new Collectible.CollectibleData("Slider 1", myArea));
        PlayerInventory.RemoveCollectible(new Collectible.CollectibleData("New Slider", myArea));
    }

    private void RestartTroops()
    {
        foreach (MilitaryUnit unit in MilitaryUnit.ActiveUnits)
        {
            unit.KillImmediate();
        }

        foreach (MilitaryUnspawnedAlly m in unspawnedAllies)
        {
            m.Reset();
        }
    }

    // In case you quit after you win
    public void EnableAllSliders()
    {
        for (int i = 1; i <= 16; i++)
        {
            CollectSTile(i);
        }
    }

    // We want to end player turn after the units are moved
    public void OnTileMove(object sender, SGridAnimator.OnTileMoveArgs e)
    {
        // CoroutineUtils.ExecuteAfterEndOfFrame(() => MilitaryTurnManager.EndPlayerTurn(), this);
        MilitaryTurnManager.EndPlayerTurn();
    }

    public void FailedMilitary4Times(Condition c) => c.SetSpec(SaveSystem.Current.GetInt("militaryAttempts") >= 4);
}
