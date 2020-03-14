using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    public enum UnitType
    {
        COMBAT,
        AGENT
    }

    // External Components
    protected GameController gameController;
    protected HexGrid hexGrid;

    // Internal Components
    [SerializeField] UnitUI unitUI;
    HexVision hexVision;

    // Unit attributes
    [SerializeField] int baseMovement = 2;
    [SerializeField] int movementLeft = 0;
    [SerializeField] int baseStrength = 20;
    [SerializeField] HexUnit hexUnit;
    [SerializeField] int baseMovementFactor = 5;
    [SerializeField] int baseHitPoints = 100;
    [SerializeField] protected Abilities abilities;

    UnitType hexUnitType;
    int hitPoints = 100;
    bool alive = true;
    CityState cityStateOwner;
    List<HexCell> path = new List<HexCell>();


    public delegate void OnInfoChange(Unit unit);
    public event OnInfoChange onInfoChange;



    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)baseHitPoints; }
    }


    public int GetBaseHitpoints()
    {
        return baseHitPoints;
    }

    public void DamageUnit(int defenceDamage)
    {
        int hitpointsLeft = HitPoints - defenceDamage;
        if(hitpointsLeft < 0)
        {
            hitpointsLeft = 0;
        }
        HitPoints = hitpointsLeft;
    }

    public virtual City GetCityOwner()
    {
        return null;
    }

    public int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            hitPoints = value;
            NotifyInfoChange();
            if(hitPoints <= 0)
            {
                GameController.KillUnit(this);
            }
        }
    }

    public HexUnit HexUnit
    {
        get
        {
            return hexUnit;
        }

        set
        {
            hexUnit = value;
        }
    }

    public GameController GameController
    {
        get
        {
            return gameController;
        }

        set
        {
            gameController = value;
        }
    }

    public int Strength
    {
        get { return BaseStrength; }
    }


    public bool Alive
    {
        get
        {
            return alive;
        }
    }
    public HexVision HexVision
    {
        get
        {
            return hexVision;
        }

        set
        {
            hexVision = value;
        }
    }

    public int BaseMovement
    {
        get
        {
            return baseMovement;
        }

        set
        {
            baseMovement = value;
            HexUnit.Speed = (BaseMovement * BaseMovementFactor);
        }
    }

    public int BaseStrength
    {
        get
        {
            return baseStrength;
        }

        set
        {
            baseStrength = value;
        }
    }

    public int BaseMovementFactor
    {
        get
        {
            return baseMovementFactor;
        }

        set
        {
            baseMovementFactor = value;
        }
    }

    public UnitType HexUnitType
    {
        get
        {
            return hexUnitType;
        }

        set
        {
            hexUnitType = value;

        }
    }

    public CityState CityStateOwner
    {
        get
        {
            return cityStateOwner;
        }

        set
        {
            cityStateOwner = value;
        }
    }

    public UnitUI UnitUI
    {
        get
        {
            return unitUI;
        }

        set
        {
            unitUI = value;
        }
    }

    public int GetMovementLeft()
    {
        return movementLeft;
    }

    public void SetMovementLeft(int movementPoints)
    {
        movementLeft = movementPoints;
        NotifyInfoChange();
    }

    public virtual Player GetPlayer()
    {
        return null;
    }


    public bool SetPath(List<HexCell> newPath)
    {
        path = newPath;
        MoveUnit();
        return true;
    }

    public bool SetPath(HexCell newPath)
    {
        path.Clear();
        path.Add(newPath);
        MoveUnit();
        return true;
    }
    private void Awake()
    {

        hexGrid = FindObjectOfType<HexGrid>();
        GameController = FindObjectOfType<GameController>();
        hexVision = gameObject.AddComponent<HexVision>();

        hexVision.AddVisibleObject(UnitUI.gameObject);
        if(hexUnit.GetMesh())
        {
            hexVision.AddVisibleObject(hexUnit.GetMesh());
        }
        hexUnit.HexVision = hexVision;
        gameController.VisionSystem.AddHexVision(hexVision);
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 10;
        HexUnit.Speed = (BaseMovement * BaseMovementFactor);
        hitPoints = baseHitPoints;


    }

    void Start()
    {
        UnitUI.Unit = this;
        StartTurn();
    }

    private void OnDestroy()
    {

    }


    public bool AttackCell(HexCell cell)
    {
        if(movementLeft < 1)
        {
            return false;
        }
        if (cell.coordinates.DistanceTo(HexUnit.Location.coordinates) > 1)
        {
            return false;
        }

        if (cell.GetFightableUnit(this.HexUnit) )
        {
            List<FightResult> results =  Fight(cell);
            hexUnit.Attack(cell, results);
            if(results.FindAll(C => C.cityTaken == true).Count != 0)
            {
                cell.City.GetCityState().SetPlayerOnly(this.GetPlayer());
                hexUnit.Capture(cell);
            }
            return true;
        }

        return false;
    }

    public bool CaptureCity(HexCell cell)
    {
        if (movementLeft < 1)
        {
            return false;
        }
        if (cell.coordinates.DistanceTo(HexUnit.Location.coordinates) > 1)
        {
            return false;
        }

        if (cell.City)
        {
            cell.City.GetCityState().SetPlayerOnly(this.GetPlayer());
            hexUnit.Capture(cell);
            return true;
        }

        return false;

    }

    public bool MoveUnit()
    {
        if (path.Count == 0)
        {
            return false;
        }
        List<HexCell> move = new List<HexCell>();
        move.Add(path[0]);
        int cellNumber = 1;
        while (movementLeft > 0 && path.Count > 1)
        {
            if (path.Count > cellNumber)
            {
                int movementCost = HexUnit.GetMoveCost(path[cellNumber - 1], path[cellNumber], path[cellNumber - 1].GetNeighborDirection(path[cellNumber]), true);
                if (movementCost == -1 || movementCost > movementLeft)
                {
                    break;
                }
                else
                {
                    move.Add(path[cellNumber]);
                    movementLeft -= movementCost;
                    cellNumber++;
                }
            }
            else
            {
                break;
            }
        }
        if (move.Count < 2)
        {
            return false;
        }

        hexUnit.Move(move);
        path.RemoveRange(0, move.Count - 1);
        for (int a = 1; a < move.Count; a++)
        {
            UpdateOwnerVisiblity(move[a - 1], false);
            UpdateOwnerVisiblity(move[a], true);
        }

        if (HexUnitType == Unit.UnitType.AGENT)
        {
            GetComponent<Influence>().UpdateEffects();
        }

        return true;
    }



    public virtual void StartTurn()
    {
        movementLeft = BaseMovement * BaseMovementFactor;
        NotifyInfoChange();
    }

    public void EndTurn()
    {
        movementLeft = 0;
    }

    public bool CheckPath()
    {
        if (path.Count == 0)
        {
            return false;
        }
        return true;
    }

    //public List<FightResult> FightCity(City city)
    //{
    //    List<FightResult> results = new List<FightResult>();
    //    if (HexUnitType == UnitType.COMBAT && GetCityOwner() != city.GetCityState())
    //    {
    //        CityState currentCityState = city.GetCityState();
    //        results = CombatSystem.Fight(this.HexUnit.Location, city.GetHexCell());

    //        SetMovementLeft(0);
    //    }
    //    return results;
    //}

    public List<FightResult> Fight(HexCell targetCell)
    {
        List<FightResult> results = new List<FightResult>();
        if (targetCell.GetFightableUnit(hexUnit))
        {
            Combat combat = CombatSystem.Fight(this.HexUnit.Location, targetCell);
            results = combat.Fight();
            hexUnit.unit.SetMovementLeft(0);
            
        }
        return results;
    }

    public virtual void UpdateUI(int healthChange)
    {
        if(healthChange != 0)
        {
            UnitUI.UpdateHealthBar(healthChange);
        }
    }


    public void ShowHealthChange(int change)
    {
        Color color;
        color = Color.red;
        if (hexUnit.Location.IsVisible)
        {
            hexUnit.Location.TextEffectHandler.AddTextEffect(change.ToString(), hexUnit.transform, color);
        }
    }

    public virtual void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (GetPlayer())
        {
            List<HexCell> cells = hexGrid.GetVisibleCells(hexCell, hexUnit.VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    GetPlayer().AddVisibleCell(cells[i]);
                }
                else
                {
                    GetPlayer().RemoveVisibleCell(cells[i]);
                }
            }
            ListPool<HexCell>.Add(cells);

        }
    }

    public void UpdatePositionInCell()
    {
        transform.localPosition = GetPositionInCell(hexUnit.Location);
    }

    public virtual Vector3 GetPositionInCell(HexCell cell)
    {
        return cell.Position;
    }

    public virtual Vector3 GetFightPosition(HexCell cell, HexCell targetCell)
    {
        return cell.Position;
    }

    public void KillUnit()
    {
        alive = false;
    }

    public abstract bool CanAttack(Unit unit);

    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }
}
