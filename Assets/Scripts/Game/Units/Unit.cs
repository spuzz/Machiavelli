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
    [SerializeField] HexCellTextEffect textEffect;

    protected Abilities abilities;
    bool alive = true;
    List<HexCell> path = new List<HexCell>();
    Player player;
    HexGrid hexGrid;
    HexCell fightInCell;
    UnitUI unitUI;
    UnitBehaviour behaviour;
    HexUnit attackUnit;
    City attackCity;
    GameController gameController;
    CityState cityState;
    int hitPoints = 1;
    int lastHitPointChange = 0;
    HexVision hexVision;
    public HexUnit AttackUnit
    {
        get
        {
            return attackUnit;
        }

        set
        {
            attackUnit = value;
        }
    }

    public City AttackCity
    {
        get
        {
            return attackCity;
        }

        set
        {
            attackCity = value;
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

    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)baseHitPoints; }
    }


    public CityState CityState
    {
        get
        {
            return cityState;
        }

        set
        {
            if (cityState)
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


    public int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            lastHitPointChange = value - hitPoints;
            hitPoints = value;
            if(hitPoints <= 0)
            {
                KillUnit();
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

    private void Awake()
    {

        hexGrid = FindObjectOfType<HexGrid>();
        Behaviour = gameObject.AddComponent<UnitBehaviour>();
        GameController = FindObjectOfType<GameController>();
        unitUI = Instantiate(unitUiPrefab).GetComponent<UnitUI>();
        hexVision = gameObject.AddComponent<HexVision>();
        abilities = GetComponent<Abilities>();
        unitUI.Unit = this;
        if (player)
        {
            unitUI.SetColour(player.Color);
        }
        if (cityState)
        {
            unitUI.SetColour(cityState.Color);
        }

        
        hexVision.AddVisibleObject(unitUI.gameObject);
        hexVision.AddVisibleObject(hexUnit.GetMesh());
        hexUnit.HexVision = hexVision;
        gameController.VisionSystem.AddHexVision(hexVision);
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 10;

    }

    public int GetMovementLeft()
    {
        return movementLeft;
    }

    public void SetMovementLeft(int movementPoints)
    {
        movementLeft = movementPoints;
    }

    public void SetPlayer(Player player)
    {
        if (player)
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

    public bool IsSomethingToAttack()
    {
        if(AttackCity || AttackUnit)
        {
            return true;
        }
        return false;
    }
    void Start() {
        HexUnit.Speed = (baseMovement * baseMovementFactor);
        hitPoints = baseHitPoints;
        StartTurn();
    }

    private void OnDestroy()
    {
        if (unitUI)
        {
            Destroy(unitUI.gameObject);
        }

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

        hexGrid.FindPath(HexUnit.Location, path[path.Count - 1], HexUnit);
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
        AttackUnit = null;
        if (path.Count == 0)
        {
            return;
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

        if (move.Count > 1)
        {
            AttackUnit = null;
            AttackCity = null;
            City city = move[move.Count - 1].City;
            if (city && hexUnit.HexUnitType == HexUnit.UnitType.COMBAT && cityState != city.GetCityState())
            {
                AttackCity = city;
                CombatSystem.CityFight(this, city);
                SetMovementLeft(0);

            }
            else
            {
                HexUnit unitToFight = move[move.Count - 1].GetFightableUnit(HexUnit);
                if (unitToFight)
                {
                    AttackUnit = unitToFight;
                    CombatSystem.UnitFight(this, unitToFight.GetComponent<Unit>());
                    SetMovementLeft(0);
                }
            }

            path.RemoveRange(0, move.Count - 1);
            HexUnit.Travel(move);

        }

    }

    public void UpdateUI()
    {
        unitUI.UpdateHealthBar();
    }

    public void ShowHealthChange(int change)
    {
        Color color;
        color = Color.red;
        if (hexUnit.Location.IsVisible)
        {
            HexCellTextEffect effect = Instantiate(textEffect).GetComponent<HexCellTextEffect>();
            effect.Show(change.ToString(), hexUnit.transform, color);
        }
    }

    public int GetLastHitPointChange()
    {
        return lastHitPointChange;
    }

    public void Heal()
    {

    }

    public void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (player)
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
        hexUnit.KillUnit();
    }
    public abstract bool CanAttack(Unit unit);

    public void AttemptAbility(int abilityNumber, HexCell hexCell)
    {
        abilities.AttemptAbility(abilityNumber, hexCell);
    }

    public void UseAbility(int abilityNumber, HexCell hexCell)
    {
        abilities.UseAbility(abilityNumber, hexCell);

    }


    public List<AbilityConfig> GetAbilities()
    {
        return abilities.AbilitiesList;
    }

    public int GetNumberOfAbilities()
    {
        return abilities.GetNumberOfAbilities();
    }
    public bool IsAbilityUsable(int abilityNumber)
    {
        return abilities.IsAbilityValid(abilityNumber, hexUnit.Location);
    }
}
