using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {


    [SerializeField] int baseMovement = 2;
    [SerializeField] int movementLeft = 0;
    [SerializeField] int baseStrength = 20;
    [SerializeField] HexUnit hexUnit;
    [SerializeField] int baseMovementFactor = 5;
    [SerializeField] int baseHitPoints = 100;
    [SerializeField] GameObject unitUiPrefab;
    [SerializeField] Texture backGround;
    [SerializeField] Texture symbol;

    bool alive = true;
    UnitBehaviour behaviour;

    HexCell attackCell;
    public HexCell AttackCell
    {
        get
        {
            return attackCell;
        }

        set
        {
            attackCell = value;
        }
    }

    public Texture BackGround
    {
        get { return backGround; }
        set { backGround = value; }
    }

    public Texture Symbol
    {
        get { return symbol; }
        set { symbol = value; }
    }

    

    List<HexCell> path = new List<HexCell>();
    Player player;
    HexGrid hexGrid;
    HexCell fightInCell;
    UnitUI unitUI;

    GameController gameController;
    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)baseHitPoints; }
    }


    public void EnableUI(bool vision)
    {

        unitUI.Visible = vision;

    }



    CityState cityState;
    public CityState CityState
    {
        get
        {
            return cityState;
        }

        set
        {
            if(cityState)
            {
                UpdateOwnerVisiblity(hexUnit.Location, false);
            }

            
            cityState = value;
            UpdateOwnerVisiblity(hexUnit.Location, true);
            if (unitUI)
            {
                unitUI.SetColour(cityState.Color);
            }
        }
    }

    int hitPoints = 1;
    public int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            hitPoints = value;
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

    public UnitBehaviour Behaviour
    {
        get { return behaviour; }
        set { behaviour = value; }
    }

    public int Strength
    {
        get { return baseStrength; }
    }

    public bool Alive
    {
        get
        {
            return alive;
        }
    }

    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        Behaviour = gameObject.AddComponent<UnitBehaviour>();
        GameController = FindObjectOfType<GameController>();
        unitUI = Instantiate(unitUiPrefab).GetComponent<UnitUI>();
        unitUI.Unit = this;
        if (player)
        {
            unitUI.SetColour(player.Color);
        }
        if (cityState)
        {
            unitUI.SetColour(cityState.Color);
        }
    }
    void Start () {
        HexUnit.Speed = (baseMovement * baseMovementFactor);
        hitPoints = baseHitPoints;
        

        
        StartTurn();
    }

    public int GetMovementLeft()
    {
        return movementLeft;
    }

    public void SetPlayer(Player player)
    {
        if(player)
        {
            UpdateOwnerVisiblity(hexUnit.Location, false);
        }
        this.player = player;

        UpdateOwnerVisiblity(hexUnit.Location, true);
        if (unitUI)
        {
            unitUI.SetColour(player.Color);
        }
        
    }

    private void OnDestroy()
    {
        if(unitUI)
        {
            Destroy(unitUI.gameObject);
        }
        
    }
    public Player GetPlayer()
    {
        return player;
    }

    public void SetPath(List<HexCell> path)
    {
        this.path = path;
        MoveUnit();

    }

    public void SetPath(HexCell path)
    {
        this.path.Clear();
        this.path.Add(HexUnit.Location);
        this.path.Add(path);
        MoveUnit();

    }


    public void StartTurn()
    {
        movementLeft = baseMovement * baseMovementFactor;
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

        hexGrid.FindPath(HexUnit.Location, path[path.Count - 1],HexUnit);
        path = hexGrid.GetPath();
        if (path.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void MoveUnit()
    {
        AttackCell = null;
        if (path.Count == 0)
        {
            return;
        }
        List<HexCell> move = new List<HexCell>();
        move.Add(path[0]);
        int cellNumber = 1;
        while (movementLeft > 0 && path.Count > 1)
        {
           if(path.Count > cellNumber)
            {
                int movementCost = HexUnit.GetMoveCost(path[cellNumber - 1], path[cellNumber], path[cellNumber - 1].GetNeighborDirection(path[cellNumber]),true);
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
        
        if(move.Count > 1)
        {
            AttackCell = null;
            City city = move[move.Count - 1].City;
            if (city && hexUnit.HexUnitType == HexUnit.UnitType.COMBAT && cityState != city.GetCityState())
            {
                AttackCell = move[move.Count - 1];
                CombatSystem.CityFight(this, city);

            }
            else
            {
                HexUnit unitToFight = move[move.Count - 1].GetFightableUnit(HexUnit);
                if (unitToFight)
                {
                    AttackCell = move[move.Count - 1];
                    CombatSystem.UnitFight(this, unitToFight.GetComponent<Unit>());
                }
            }

            path.RemoveRange(0, move.Count - 1);
            HexUnit.Travel(move, AttackCell);
            
        }

    }

    public void UpdateUI()
    {
        unitUI.UpdateHealthBar();
    }

    public void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if(player)
        {
            List<HexCell> cells = hexGrid.GetVisibleCells(hexCell, hexUnit.VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    player.AddVisibleCell(cells[i]);
                }
                else
                {
                    player.RemoveVisibleCell(cells[i]);
                }
            }
            ListPool<HexCell>.Add(cells);

        }
        if (cityState)
        {
            List<HexCell> cells = hexGrid.GetVisibleCells(hexCell, hexUnit.VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    cityState.AddVisibleCell(cells[i]);
                }
                else
                {
                    cityState.RemoveVisibleCell(cells[i]);
                }
            }
            ListPool<HexCell>.Add(cells);

        }

    }

    public void KillUnit()
    {
        alive = false;
        //if(player && GetComponent<Agent>())
        //{
        //    player.RemoveAgent(GetComponent<Agent>());
        //}

        //if (cityState)
        //{
        //    cityState.RemoveUnit(this);
        //}
        hexGrid.RemoveUnit(hexUnit);
    }
    public abstract bool CanAttack(Unit unit);

    public abstract void UseAbility(int abilityNumber, HexCell hexCell);

}
