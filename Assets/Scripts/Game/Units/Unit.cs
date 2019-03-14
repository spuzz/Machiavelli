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
    [SerializeField] AnimatorOverrideController animatorOverrideController;

    protected Abilities abilities;
    bool alive = true;
    List<HexCell> path = new List<HexCell>();

    protected HexGrid hexGrid;
    HexCell fightInCell;
    protected UnitUI unitUI;
    HUD hudUI;
    UnitBehaviour behaviour;
    HexUnit attackUnit;
    City attackCity;
    protected GameController gameController;

    int hitPoints = 100;
    int lastHitPointChange = 0;

    public delegate void OnInfoChange(Unit unit);
    public event OnInfoChange onInfoChange;

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
        set { backGround = value;
            if (unitUI)
            {
                unitUI.SetBackground(backGround);
            }
        }
    }

    public Texture Symbol
    {
        get { return symbol; }
        set
        {
            symbol = value;
            if(unitUI)
            {
                unitUI.SetUnitSymbol(symbol);
            }
        }
    }

    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)baseHitPoints; }
    }


    public int GetBaseHitpoints()
    {
        return baseHitPoints;
    }
    public CityState CityState
    {
        get
        {
            return GetCityState();
        }

        set
        {
            SetCityState(value);
        }
    }

    public virtual CityState GetCityState()
    {
        return null;
    }

    public virtual void SetCityState(CityState cityState)
    {

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
            NotifyInfoChange();
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

    public HUD HUDUI
    {
        get
        {
            return hudUI;
        }

        set
        {
            hudUI = value;
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

    public AnimatorOverrideController AnimatorOverrideController
    {
        get
        {
            return animatorOverrideController;
        }

        set
        {
            animatorOverrideController = value;
        }
    }

    public virtual Player GetPlayer()
    {
        return null;
    }

    public virtual void Setup()
    {

    }
    private void Awake()
    {

        hexGrid = FindObjectOfType<HexGrid>();
        HUDUI = FindObjectOfType<HUD>();
        Behaviour = gameObject.AddComponent<UnitBehaviour>();
        GameController = FindObjectOfType<GameController>();
        unitUI = Instantiate(unitUiPrefab).GetComponent<UnitUI>();
        hexVision = gameObject.AddComponent<HexVision>();
        abilities = GetComponent<Abilities>();
        unitUI.Unit = this;
        hexVision.AddVisibleObject(unitUI.gameObject);
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
        Setup();

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

    public bool SetPath(List<HexCell> path)
    {
        this.path = path;
        return MoveUnit();

    }

    public bool SetPath(HexCell path)
    {
        this.path.Clear();
        this.path.Add(HexUnit.Location);
        this.path.Add(path);
        return MoveUnit();

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

    public bool MoveUnit()
    {
        AttackUnit = null;
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

        if (move.Count > 1)
        {
            AttackUnit = null;
            AttackCity = null;
            City city = move[move.Count - 1].City;
            if (city && hexUnit.HexUnitType == HexUnit.UnitType.COMBAT && GetCityState() != city.GetCityState())
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
            return true;

        }
        return false;
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

    public virtual void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (GetCityState())
        {
            List<HexCell> cells = hexGrid.GetVisibleCells(hexCell, hexUnit.VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    GetCityState().AddVisibleCell(cells[i]);
                }
                else
                {
                    GetCityState().RemoveVisibleCell(cells[i]);
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
        NotifyInfoChange();
    }

    public void UseAbility(int abilityNumber, HexCell hexCell)
    {
        abilities.UseAbility(abilityNumber, hexCell);
        NotifyInfoChange();

    }
    public void UseAbility(string abilityName, HexCell hexCell)
    {
        abilities.UseAbility(abilities.AbilitiesList.IndexOf(abilities.AbilitiesList.Find(c=> c.AbilityName == abilityName)), hexCell);
        NotifyInfoChange();

    }

    public IEnumerable<AbilityConfig> GetAbilities()
    {
        return abilities.AbilitiesList;
    }

    public bool HasAbility(string abilityName)
    {
        if (abilities.AbilitiesList.FindAll(c => c.AbilityName == abilityName).Count > 0)
        {
            return true;
        }
        return false;
    }
    public AbilityConfig GetAbility(int abilityNumber)
    {
        return abilities.AbilitiesList[abilityNumber];
    }

    public AbilityConfig GetAbility(string abilityName)
    {
        return abilities.AbilitiesList.Find(c => c.AbilityName == abilityName);
    }

    public List<AbilityConfig> GetAbilities(AbilityConfig.AbilityType abilityType)
    {
        return abilities.AbilitiesList.FindAll(c => c.Type == abilityType);
    }

    public int GetNumberOfAbilities()
    {
        return abilities.GetNumberOfAbilities();
    }
    public bool IsAbilityUsable(int abilityNumber)
    {
        return abilities.ValidTargets(abilityNumber, hexUnit.Location).Count > 0;
    }

    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }
}
