using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {


    [SerializeField] int baseMovement = 2;
    [SerializeField] int movementLeft = 0;
    [SerializeField] int baseStrength = 20;
    [SerializeField] int baseRangeStrength = 0;
    [SerializeField] int range = 0;
    [SerializeField] HexUnit hexUnit;
    [SerializeField] int baseMovementFactor = 5;
    [SerializeField] int baseHitPoints = 100;
    [SerializeField] GameObject unitUiPrefab;
    [SerializeField] Texture backGround;
    [SerializeField] Texture symbol;
    [SerializeField] HexCellTextEffect textEffect;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField] Projectile projectilePreFab;
    [SerializeField] Material defaultUnitMaterial;
    List<HexAction> actions = new List<HexAction>();
    HexUnitActionController hexUnitActionController;
    protected Abilities abilities;
    bool alive = true;
    List<HexCell> path = new List<HexCell>();

    protected HexGrid hexGrid;
    HexCell fightInCell;
    protected UnitUI unitUI;
    HUD hudUI;
    UnitBehaviour behaviour;
    protected GameController gameController;

    int hitPoints = 100;
    int lastHitPointChange = 0;

    public delegate void OnInfoChange(Unit unit);
    public event OnInfoChange onInfoChange;

    HexVision hexVision;

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

    public virtual City GetCityOwner()
    {
        return null;
    }

    public CityState GetCityState()
    {
        City city = GetCityOwner();
        if (city)
        {
            return city.GetCityState();
        }
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
            lastHitPointChange = value - hitPoints;
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

    public UnitBehaviour Behaviour
    {
        get { return behaviour; }
        set { behaviour = value; }
    }

    public int Strength
    {
        get { return BaseStrength; }
    }

    public int RangeStrength
    {
        get { return BaseRangeStrength; }
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


    public int Range
    {
        get
        {
            return range;
        }

        set
        {
            range = value;
        }
    }

    public int BaseRangeStrength
    {
        get
        {
            return baseRangeStrength;
        }

        set
        {
            baseRangeStrength = value;
        }
    }

    public Projectile ProjectilePreFab
    {
        get
        {
            return projectilePreFab;
        }

        set
        {
            projectilePreFab = value;
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
        hexUnitActionController = FindObjectOfType<HexUnitActionController>();
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

    public bool AttackCell(HexCell cell)
    {
        bool melee = true;
        if (Range == 0)
        {
            if(cell.coordinates.DistanceTo(HexUnit.Location.coordinates) > 1)
            {
                return false;
            }
        }
        else
        {
            if (cell.coordinates.DistanceTo(HexUnit.Location.coordinates) > Range)
            {
                return false;
            }
        }
        if(Range > 0)
        {
            melee = false;
        }
        if (cell.City)
        {
            CityState cityState = cell.City.GetCityState();
            KeyValuePair<int,int> result = FightCity(cell.City);
            HexAction action = hexUnitActionController.CreateAction();
            action.ActionsUnit = this.HexUnit;
            action.MeleeAction = melee;
            action.AddAction(cell, hexUnit.Location, cell.City, result.Value, result.Key, cityState);
            if (cell.City.GetCityState() == GetCityOwner())
            {
                action.SetKillTarget();
            }
            if (Alive == false)
            {
                action.SetKillSelf();
            }
            actions.Add(action);
            return true;
        }
        HexUnit unit = cell.GetFightableUnit(this.HexUnit);
        if(unit)
        {
            KeyValuePair<int, int> result =  FightUnit(unit);
            HexAction action = hexUnitActionController.CreateAction();
            action.ActionsUnit = this.HexUnit;
            action.MeleeAction = melee;
            action.AddAction(cell, hexUnit.Location, unit, result.Value, result.Key);
            if (unit.unit.HitPoints <= 0)
            {
                action.SetKillTarget();
            }
            if (Alive == false)
            {
                action.SetKillSelf();
            }
            actions.Add(action);
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
        HexAction action = hexUnitActionController.CreateAction();
        action.ActionsUnit = this.HexUnit;
        action.AddAction(move);

        hexUnit.Location.RemoveUnit(hexUnit);
        hexUnit.SetLocationOnly(move[move.Count - 1]);
        if (Alive == true)
        {
            hexUnit.AddUnitToLocation(move[move.Count - 1]);
        }

        actions.Add(action);
        for (int a = 1; a < move.Count; a++)
        {
            UpdateOwnerVisiblity(move[a - 1], false);
            UpdateOwnerVisiblity(move[a], true);
        }
        return true;
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
        DoActions();
        movementLeft = 0;
    }
    public void DoActions()
    {

        hexUnitActionController.AddActions(actions, hexUnit);
        actions.Clear();
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

    public KeyValuePair<int,int> FightCity(City city)
    {
        KeyValuePair<int, int> result = new KeyValuePair<int, int>();
        if (hexUnit.HexUnitType == HexUnit.UnitType.COMBAT && GetCityOwner() != city.GetCityState())
        {
            CityState currentCityState = city.GetCityState();
            result = CombatSystem.CityFight(this, city);
            if (city.HitPoints <= 0)
            {
                if (hexUnit.HexUnitType == HexUnit.UnitType.COMBAT && GetComponent<CombatUnit>().Mercenary && !GetComponent<Unit>().GetCityOwner())
                {
                    GetComponent<Unit>().GetPlayer().Gold += city.Plunder();
                    city.HitPoints = 1;
                }
                else
                {
                    HexUnit unitInCity = city.GetHexCell().hexUnits.Find(c => c.HexUnitType == HexUnit.UnitType.COMBAT);
                    if(unitInCity)
                    {
                        gameController.KillUnit(unitInCity.unit);
                    }
                    city.KillCityUnits();
                    city.SetCityState(GetCityState());
                    city.HitPoints = 50;
                }
            }

            SetMovementLeft(0);
        }
        return result;
    }

    public KeyValuePair<int, int> FightUnit(HexUnit unit)
    {
        KeyValuePair<int, int> result = new KeyValuePair<int, int>();
        if (unit)
        {
            result = CombatSystem.UnitFight(this, unit.GetComponent<Unit>());
            SetMovementLeft(0);
        }
        return result;
    }
    public void UpdateUI(int healthChange)
    {
        if(healthChange != 0)
        {
            unitUI.UpdateHealthBar(healthChange);
        }

    }

    public void UpdateColours()
    {
        if (GetPlayer())
        {
            unitUI.SetColour(GetPlayer().GetColour().Colour);
            HexUnit.MaterialColourChanger.ChangeMaterial(GetPlayer().GetColour());
        }
        else if(GetCityOwner() && GetCityOwner().Player)
        {
            unitUI.SetColour(GetCityOwner().Player.GetColour().Colour);
            HexUnit.MaterialColourChanger.ChangeMaterial(GetCityOwner().Player.GetColour());
        }
        else
        {
            unitUI.SetColour(Color.black);
            HexUnit.MaterialColourChanger.ChangeMaterial(defaultUnitMaterial);
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

    public void RunAbility(int abilityNumber, HexCell hexCell)
    {
        abilities.RunAbility(abilityNumber, hexCell);
        NotifyInfoChange();

    }
    public void RunAbility(string abilityName, HexCell hexCell)
    {
        abilities.RunAbility(abilities.AbilitiesList.IndexOf(abilities.AbilitiesList.Find(c => c.AbilityName == abilityName)), hexCell);
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
