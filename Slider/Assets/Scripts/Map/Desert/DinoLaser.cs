using UnityEngine;

public class DinoLaser : MonoBehaviour
{
    [SerializeField] private MagiLaser laser;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public UILaserManager uILaserManager;

    public void SetSkullSpriteToBroken()
    {
        spriteRenderer.enabled = true;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void EnableLaser(bool on)
    {
        laser.SetEnabled(on);
        uILaserManager.UpdateSpritesFromSource();
    }
}
