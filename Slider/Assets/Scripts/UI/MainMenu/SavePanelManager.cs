using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SavePanelManager : MonoBehaviour
{
    public enum SaveMode 
    {
        Normal,
        Delete,
        Backup,
    }

    public class SaveModeArgs : System.EventArgs
    {
        public SaveMode mode;
    }
    public static System.EventHandler<SaveModeArgs> OnSaveModeChanged;

    private const string NORMAL_MODE_TEXT = "";
    private const string DELETE_MODE_TEXT = "Delete file?";
    private const string BACKUP_MODE_TEXT = "Restore a backup?";

    [SerializeField] private NewSavePanelManager newSavePanelManager;
    [SerializeField] private UIMenu savePanel;
    [SerializeField] private UIMenu newSavePanel;
    [SerializeField] private TextMeshProUGUI titleText;

    // We skip save picking if there are no saves and go straight to the new save menu. When we hit escape, we want
    // to come back here and not immediately go *back* to the new save menu.
    private bool hasAlreadySkippedSavePicking = false;

    public SaveMode CurrentMode { get; private set; } = SaveMode.Normal;

    public void OpenSaves()
    {
        if (!AreAnyProfilesLoaded() && !hasAlreadySkippedSavePicking)
        {
            hasAlreadySkippedSavePicking = true;
            OpenNewSave(0);
            return;
        }

        SetMode(SaveMode.Normal);
    }

    private void OnDisable()
    {
        hasAlreadySkippedSavePicking = false;
    }

    public void ToggleDeleteMode()
    {
        SetMode(CurrentMode == SaveMode.Delete ? SaveMode.Normal : SaveMode.Delete);
    }

    public void ToggleBackupMode()
    {
        SetMode(CurrentMode == SaveMode.Backup ? SaveMode.Normal : SaveMode.Backup);
    }

    public void SetMode(SaveMode mode)
    {
        switch (mode)
        {
            case SaveMode.Normal:
                titleText.text = NORMAL_MODE_TEXT;
                break;
            case SaveMode.Delete:
                titleText.text = DELETE_MODE_TEXT;
                break;
            case SaveMode.Backup:
                titleText.text = BACKUP_MODE_TEXT;
                break;
        }

        CurrentMode = mode;
        OnSaveModeChanged?.Invoke(this, new SaveModeArgs { mode = CurrentMode });
    }

    public void OpenNewSave(int profileIndex)
    {
        savePanel.MoveToMenu(newSavePanel);
        newSavePanelManager.OpenNewSave(profileIndex);
    }

    private bool AreAnyProfilesLoaded()
    {
        return SaveSystem.GetProfile(0) != null || SaveSystem.GetProfile(1) != null || SaveSystem.GetProfile(2) != null;
    }
}
