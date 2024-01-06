using UnityEngine;
using UnityEngine.UI;

public class LaserUIOffMap : MonoBehaviour
{
    private bool laserOn;
    [SerializeField] private MagiTechArtifact magiTechArtifact;
    [SerializeField] private Image laserSprite;
    public GameObject desertPortalUI;

    void LateUpdate()
    {
        bool shouldShow = laserOn &&
        desertPortalUI.activeSelf &&
        (magiTechArtifact == null
        || magiTechArtifact.IsDisplayingPast());
        laserSprite.enabled = shouldShow;
    }

    public void ShowLaser()
    {
        laserOn = true;
    }

    public void HideLaser()
    {
        laserOn = false;
    }
}
