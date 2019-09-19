using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI
public class UnitUI : MonoBehaviour {

    // Works around Unity 5.5's lack of nested prefabs
    [Tooltip("The UI canvas prefab")]
    [SerializeField] Canvas canvas;
    [SerializeField] RawImage unitBackground;
    [SerializeField] UnitHealthBar unitHealthBar;
    [SerializeField] RawImage unitSymbol;
    [SerializeField] Image cityStateSymbol;

    [SerializeField] Texture Symbol;
    [SerializeField] Texture BackGround;
    [SerializeField] HexCellTextEffect textEffect;

    [SerializeField] Unit unit;
    [SerializeField] List<Button> unitStackButtons;
    Camera cameraToLookAt;
    bool visible = true;

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
            unitHealthBar.Unit = unit;
        }
    }

    public bool Visible
    {
        get
        {
            return visible;
        }

        set
        {
            visible = value;
            //canvas.enabled = value;
        }
    }

    public void SetColour(Color color)
    {
        unitBackground.color = new Color(color.r, color.g, color.b, 0.6f);
    }


    public void SetCityStateSymbol(Sprite symbol)
    {
        if(cityStateSymbol)
        {
            cityStateSymbol.sprite = symbol;
        }
        
    }

    public void SetBackground(Texture background)
    {
        unitBackground.texture = background;
    }

    private void Awake()
    {
        cameraToLookAt = Camera.main;
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + 14, unit.transform.position.z);
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        transform.Translate(new Vector3(0, 0, 6));
        
    }

    public void SetUnitSymbol(Texture symbol)
    {
        unitSymbol.texture = symbol;
        Symbol = symbol;
    }
    public void UpdateUnit(Unit unit, int healthChange)
    {
        UpdateHealthBar(healthChange);
    }
    public void UpdateHealthBar(int healthChange)
    {
        if(unitHealthBar)
        {
            unitHealthBar.UpdateHealth(healthChange);
        }
        
    }

    public void UpdateStackButtons()
    {
        int unitCount = 0;
        foreach(HexUnit hexUnit in unit.HexUnit.Location.hexUnits)
        {
            if(hexUnit != unit.HexUnit)
            {
                unitStackButtons[unitCount].gameObject.SetActive(true);
                unitStackButtons[unitCount].GetComponentInChildren<RawImage>().texture = hexUnit.unit.UnitUI.Symbol;
                unitCount++;
            }
        }
        for(int a= unitCount; a < unitStackButtons.Count;a++)
        {
            unitStackButtons[a].gameObject.SetActive(false);
        }
    }

    public void SelectUnit(int buttonNumber)
    {
        int unitCount = 0;
        foreach (HexUnit hexUnit in unit.HexUnit.Location.hexUnits)
        {
            if (hexUnit != unit.HexUnit)
            {
                if(unitCount == buttonNumber)
                {
                    FindObjectOfType<HexGameUI>().SelectUnit(hexUnit);
                    return;
                }
                unitCount++;
            }
        }

    }
}