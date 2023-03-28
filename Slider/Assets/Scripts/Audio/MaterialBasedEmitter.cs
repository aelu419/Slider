using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MaterialBasedEmitter : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour locatableRef;

    [SerializeField]
    private MaterialSoundMapping mapping;

    private ISTileLocatable locatable;

    // Start is called before the first frame update
    void Start()
    {
        if (locatableRef is ISTileLocatable)
        {
            locatable = locatableRef as ISTileLocatable;
        } else
        {
            Debug.LogError($"{locatableRef.name} does not actually implement ISTileLocatable ");
        }
    }

    public void Step()
    {
        Tilemap map = locatable.currentMaterialTilemap;
        TileBase tile = map == null ? null : map.GetTile(map.WorldToCell(locatableRef.transform.position));
        PlayStepType(mapping[tile]);
    }

    private void PlayStepType(FMODUnity.EventReference e)
    {
        AudioManager.PlayFmodWithPosition(e, transform.position);
    }
}
