using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MGUIUnitTracker : MonoBehaviour
{
    //[SerializeField] private TMPro.TextMeshProUGUI text;
    //[SerializeField] private int count;
    [SerializeField] private Sprite[] jobIcons;
    [SerializeField] private Color allyColor;
    [SerializeField] private Color enemyColor;

    private MGUnit _unit;
    private MGUI _ui;
    private Image _image;

    private void Awake()
    {
        _ui = GetComponentInParent<MGUI>();
        _image = GetComponent<Image>();
    }

    public void SetSquare(MGUISquare square)
    {
        transform.SetParent(square.transform);
        transform.localPosition = Vector3.zero;
    }

    public void SetUnit(MGUnit unit)
    {
        _unit = unit;

        MGUISquare trackerSquare = _ui.GetSquare(unit.CurrSpace);
        SetSquare(trackerSquare);

        _image.sprite = jobIcons[(int) unit.Data.job];
        //TODO: Change image based on the unit type (RPS)
        switch (unit.Data.side)
        {

            case MGUnitData.Allegiance.Ally:
                _image.color = allyColor;
                break;
            default:
                _image.color = enemyColor;
                break;
        }

        //Event Handlers
        unit.OnUnitMove += OnUnitMove;
        unit.OnUnitDestroy += OnUnitDestroy;
    }

    public void OnUnitMove(MGSpace oldSpace, MGSpace newSpace)
    {
        SetSquare(_ui.GetSquare(newSpace));
    }

    public void OnUnitDestroy()
    {
        _unit.OnUnitMove -= OnUnitMove;
        _unit.OnUnitDestroy -= OnUnitDestroy;
        Destroy(this.gameObject);
    }
}
