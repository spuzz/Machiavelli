using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class City : MonoBehaviour {

    public static int cityIDCounter = 1;
    int cityID;
    [SerializeField] int baseHitPoints = 200;
    [SerializeField] int baseStrength = 25;
    [SerializeField] int baseProduction = 0;
    [SerializeField] int baseFood = 0;
    [SerializeField] int baseScience = 0;
    [SerializeField] int baseIncome = 0;
    [SerializeField] CityUI cityUI;
    
    [SerializeField] HexGrid hexGrid;
    [SerializeField] List<CityStateBuilding> buildings;
    [SerializeField] List<BuildConfig> availableBuildings;
    [SerializeField] List<BuildConfig> availableUnits;
    [SerializeField] int food = 0;
    [SerializeField] int population = 1;
    [SerializeField] int visionRange = 2;
    [SerializeField] int unitCap = 3;
    [SerializeField] int gold = 100;
    [SerializeField] PlayerBuildingControl playerBuildingControl;
    [SerializeField] CityBonus playerCityBonus;
    [SerializeField] int happiness = 100;
    [SerializeField] GameObject textEffect;
    Dictionary<Player, int> influenceDict = new Dictionary<Player, int>();
    Dictionary<Player, int> playerInfluencePerTurn = new Dictionary<Player, int>();
    Dictionary<Effect, int> currentEffects = new Dictionary<Effect, int>();
    BuildingManager buildingManager = new BuildingManager();
    private bool capital = false;
    public int currentStrength = 0;
    public int currentProduction = 0;
    public int currentFood = 0;
    public int currentScience = 0;
    public int currentIncome = 0;
    public int currentPlayerIncome = 0;
    public int foodConsumption = 0;
    public int foodForNextPop = 0;
    Player player;
    CityState cityStateOwner;
    GameController gameController;
    HexCell hexCell;
    List<HexCell> ownedCells = new List<HexCell>();
    HexVision hexVision;
    
    List<CombatUnit> cityUnits = new List<CombatUnit>();
    List<CombatUnit> unitsToDestroy = new List<CombatUnit>();

    public delegate void OnInfoChange(City city);
    public event OnInfoChange onInfoChange;

    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)BaseHitPoints; }
    }

    int hitPoints;
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
        }
    }
    public int Strength
    {
        get { return currentStrength; }
    }

    public BuildingManager BuildingManager
    {
        get
        {
            return buildingManager;
        }

        set
        {
            buildingManager = value;
        }
    }

    public int Population
    {
        get
        {
            return population;
        }

        set
        {
            population = value;
            //CityUI.SetPopulation(population.ToString());
            NotifyInfoChange();
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

    public int VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public CityUI CityUI
    {
        get
        {
            return cityUI;
        }

        set
        {
            cityUI = value;
        }
    }

    public int BaseHitPoints
    {
        get
        {
            return baseHitPoints;
        }

        set
        {
            baseHitPoints = value;
        }
    }

    public int Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    public PlayerBuildingControl PlayerBuildingControl
    {
        get
        {
            return playerBuildingControl;
        }

        set
        {
            playerBuildingControl = value;
        }
    }
    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            if(player)
            {
                player.RemoveCity(this);
            }
            player = value;
            influenceDict.Clear();
            if (player)
            {
                player.AddCity(this);
                influenceDict[Player] = 100;

            }
            gameController.CheckCityState(GetCityState());
            UpdateCityBar();
            foreach (CombatUnit unit in cityUnits)
            {
                unit.UpdateUI(0);
                unit.UpdateColours();
            }
            UpdateVision();
            NotifyInfoChange();

        }
    }
    public void UpdateVision()
    {
        if (!Player)
        {
            SetVision(false);
            return;
        }

        if (player.IsHuman)
        {
            SetVision(true);
        }
        else
        {
            SetVision(false);
        }
    }

    private void SetVision(bool vision)
    {

        HexVision.HasVision = vision;
        foreach (CombatUnit unit in cityUnits)
        {
            unit.HexVision.HasVision = vision;
        }

    }

    public void AdjustInfluence(Player adjustPlayer, int influence)
    {
        if (!Player)
        {
            if (!influenceDict.Keys.Contains(adjustPlayer))
            {
                influenceDict[adjustPlayer] = influence;
            }
            else
            {
                influenceDict[adjustPlayer] += influence;
            }
        }
        else if (Player && adjustPlayer == Player)
        {
            influenceDict[adjustPlayer] += influence;
        }
        NotifyInfoChange();
    }

    public void CheckInfluence()
    {
        if (!Player)
        {
            int maxValue = 0;
            if (influenceDict.Count > 0)
            {
                maxValue = influenceDict.Values.Max();
            }
            if (maxValue >= 100)
            {
                Player keyOfMaxValue = influenceDict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                Player = keyOfMaxValue;

            }
        }
        else
        {
            if (influenceDict[Player] <= 0)
            {
                Player = null;
            }
            else if (influenceDict[player] > 100)
            {
                influenceDict[player] = 100;
            }
        }
        TidyInfluenceDict();
    }

    private void TidyInfluenceDict()
    {
        List<Player> keys = new List<Player>(influenceDict.Keys);
        foreach (Player player in keys)
        {
            if(influenceDict[player] < 0)
            {
                influenceDict[player] = 0;
            }
            else if(influenceDict[player] > 100)
            {
                influenceDict[player] = 100;
            }
        }
    }

    public int GetInfluence(Player player)
    {
        if (!influenceDict.Keys.Contains(player))
        {
            return 0;
        }
        else
        {
            return influenceDict[player];
        }
    }

    public int GetInfluencePerTurn(Player player)
    {
        int infPerTurn = GetNegativeInfluence();
        if (playerInfluencePerTurn.Keys.Contains(player))
        {
            infPerTurn += playerInfluencePerTurn[player];
        }
        return infPerTurn;

    }

    public int GetNegativeInfluence()
    {
        return -((int)Math.Pow(2 * 1, cityStateOwner.GetCityCount() - 1));
    }

    public void AdjustInfluenceForAll(int influence)
    {
        List<Player> keys = influenceDict.Keys.ToList();
        foreach (Player player in keys)
        {
            influenceDict[player] += influence;
        }
        NotifyInfoChange();
    }

    public void AdjustInfluenceForAllExcluding(Player excludedPlayer, int influence)
    {
        List<Player> players = influenceDict.Keys.ToList();
        foreach (Player player in players)
        {
            if (player != excludedPlayer)
            {
                influenceDict[player] += influence;
            }
        }
        NotifyInfoChange();
    }

    private void UpdateInfluence()
    {
        int negativeInfluence = GetNegativeInfluence();
        List<Player> players;
        if(influenceDict.Keys.Count == 0)
        {
            players = gameController.GetPlayers();
        }
        else
        {
            players = influenceDict.Keys.ToList();
        }
        foreach(Player player in players)
        {
            int inf = negativeInfluence;
            if(playerInfluencePerTurn.ContainsKey(player))
            {
                inf += playerInfluencePerTurn[player];
            }
            AdjustInfluence(player, inf);
        }
        CheckInfluence();
        NotifyInfoChange();
    }

    public bool Capital
    {
        get
        {
            return capital;
        }

        set
        {
            capital = value;
            RefreshYields();
        }
    }

    public CityBonus PlayerCityBonus
    {
        get
        {
            return playerCityBonus;
        }

        set
        {
            playerCityBonus = value;
        }
    }

    public int Happiness
    {
        get
        {
            return happiness;
        }

        set
        {
            happiness = value;
            if(happiness <= 0)
            {
                Rebel();
            }
        }
    }

    public int CityID
    {
        get
        {
            return cityID;
        }

        set
        {
            cityID = value;
        }
    }

    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public bool HasUnitSpace()
    {
        return cityUnits.Count < unitCap;

    }

    public void RemoveUnit(CombatUnit unit)
    {
        cityUnits.Remove(unit);
        NotifyInfoChange();
    }

    public void AddUnit(CombatUnit unit)
    {
        if (player && player.IsHuman)
        {
            unit.HexVision.HasVision = true;
        }
        cityUnits.Add(unit);
        unit.CityOwner = this;
        NotifyInfoChange();
    }

    public IEnumerable<CombatUnit> GetUnits()
    {
        return cityUnits;
    }

    public void SetCityState(CityState cityState)
    {
        if (cityStateOwner)
        {
            cityStateOwner.RemoveCity(this);
            UpdateOwnerVisiblity(hexCell, false);
        }
        if(Player)
        {
            Player = null;
        }
        cityStateOwner = cityState;
        cityStateOwner.AddCity(this);
        if(cityStateOwner.Player)
        {
            Player = cityStateOwner.Player;
        }
        UpdateOwnerVisiblity(hexCell, true);
        foreach(CombatUnit unit in cityUnits)
        {
            unit.CityOwner = this;
        }
        UpdateCityBar();
        NotifyInfoChange();

    }
    public virtual void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (GetCityState())
        {
            List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, VisionRange);
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

    public CityState GetCityState()
    {
        return cityStateOwner;
    }

    public void SetHexCell(HexCell cell)
    {
        hexCell = cell;
        hexCell.Walled = false;
        hexCell.City = this;
        ownedCells.Clear();
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = hexCell.GetNeighbor(d);
            if (neighbour)
            {
                ownedCells.Add(neighbour);
            }

        }
        UpdateWalls();
        AddCellYield(hexCell);
        HexVision.SetCells(PathFindingUtilities.GetCellsInRange(hexCell, VisionRange));
        NotifyInfoChange();
    }


    public HexCell GetHexCell()
    {
        return hexCell;
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        hexGrid = FindObjectOfType<HexGrid>();
        hexVision = gameObject.AddComponent<HexVision>();
        hitPoints = BaseHitPoints;
        CalculateFoodForNextPop();
        gameController.VisionSystem.AddHexVision(hexVision);
        cityID = cityIDCounter;
        cityIDCounter++;
    }

    private void CalculateFoodForNextPop()
    {
        foodForNextPop = 10;
        for (int i = 1; i < population; i++)
        {
            foodForNextPop = (int)(foodForNextPop * 2f);
        }
    }

    public void StartTurn()
    {
        // cityStateOwner.Gold += currentIncome;
        //Food += currentFood;
        //if(Food >= foodForNextPop)
        //{
        //    Food -= foodForNextPop;
        //    Population += 1;
        //    foodConsumption += 1;
        //    AddCellYield(hexCell);
        //    CalculateFoodForNextPop();
        //}
        //else if(Food <= 0 && population != 1)
        //{
        //    Population -= 1;
        //    foodConsumption -= 1;
        //    RemoveCellYield(hexCell);
        //    CalculateFoodForNextPop();
        //}

        foreach (CombatUnit unit in cityUnits)
        {
            unit.StartTurn();
        }
        Gold += GetIncome();
        PlayerBuildingControl.StartTurn();
        foreach(Effect effect in currentEffects.Keys)
        {
            effect.UseEffect(this);
        }
        foreach (var i in currentEffects.Where(d => d.Value <= 0).ToList())
        {
            currentEffects.Remove(i.Key);
        }
        UpdateInfluence();
        RefreshYields();
    }


    public void TakeTurn()
    {
        BuildingManager.DayPassed(currentProduction);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.COMBAT_UNIT)
            {
                CreateUnit((buildConfig as CombatUnitBuildConfig).CombatUnitConfig);
            }
            else if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.CITY_STATE_BUILDING)
            {
                AddBuilding(buildConfig as CityStateBuildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
    }

    public void AddEffect(Effect effect, int duration)
    {
        Effect existingEffect = currentEffects.Keys.ToList().Find(c => c.Compare(effect));

        if (existingEffect != null)
        {
            currentEffects.Remove(existingEffect);
        }
        currentEffects[effect] = duration;
        
    }

    public bool HasEffect(Player player, string effectName)
    {
        if(currentEffects.Keys.ToList().Find(c => c.Name.CompareTo(effectName) == 0 && c.Player == player) != null)
        {
            return true;
        }
        return false;
    }
    public void UpdateHealthBar()
    {

        CityUI.UpdateHealthBar();
    }

    public void UpdateCityBar()
    {
        if (Player)
        {
            CityUI.SetPlayerColour(Player.GetColour().Colour);
            hexCell.PlayerColour = Player.GetColour();
        }
        else
        {
            CityUI.SetPlayerColour(Color.black);
            hexCell.PlayerColour = null;
        }

        cityUI.CityStateSymbol.sprite = gameController.GetCityStateSymbol(cityStateOwner.SymbolID);

    }

    public void AddInfluencePerTurn(Player player, int influence)
    {
        if(playerInfluencePerTurn.ContainsKey(player))
        {
            playerInfluencePerTurn[player] += influence;
        }
        else
        {
            playerInfluencePerTurn[player] = influence;
        }
    }

    public void RemoveInfluencePerTurn(Player player, int influence)
    {
        if (playerInfluencePerTurn.ContainsKey(player))
        {
            playerInfluencePerTurn[player] -= influence;
        }
    }

    private void AddCellYield(HexCell hexCell)
    {
        baseProduction += hexCell.Production;
        baseIncome += hexCell.Income;
        baseFood += hexCell.Food;
        RefreshYields();
    }

    private void RemoveCellYield(HexCell hexCell)
    {
        baseProduction -= hexCell.Production;
        baseIncome -= hexCell.Income;
        baseFood -= hexCell.Food;
        RefreshYields();
    }


    private void UpdateWalls()
    {
        foreach(HexCell cell in ownedCells)
        {
            cell.Walled = false;
        }
    }

    public void DestroyCity()
    {
        if (Player)
        {
            Player.RemoveCity(this);
        }
        DestroyCityUnits();
        foreach (HexCell cell in ownedCells)
        {
            cell.Walled = false;
        }
        hexCell.Walled = false;
        Destroy(gameObject);
        
    }

    public int TakeGold(float perc)
    {
        float goldToTake = ((float)gold / 100.0f) * perc;
        Gold -= (int)goldToTake;
        return (int)goldToTake;
    }
    public bool CreateUnit(CombatUnitConfig combatUnitConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, 2);
        foreach(HexCell cell in cells)
        {
            if(!cell.IsUnderwater && cell.CanUnitMoveToCell(HexUnit.UnitType.COMBAT))
            {
                HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, cell, this);
                hexUnit.Location.UpdateVision();
                UpdateVision();
                return true;
            }
        }
        return false;
    }

    public void KillCityUnits()
    {
        List<CombatUnit> unitsToKill = new List<CombatUnit>();
        foreach(CombatUnit unit in cityUnits)
        {
            unitsToKill.Add(unit);
        }
        foreach (CombatUnit unit in unitsToKill)
        {
            gameController.KillUnit(unit);
            unitsToDestroy.Add(unit);
        }
        cityUnits.Clear();
    }

    public void DestroyCityUnits()
    {

        foreach (CombatUnit unit in unitsToDestroy)
        {
            gameController.DestroyUnit(unit);
        }
        unitsToDestroy.Clear();
    }
    public void BuildBuilding()
    {

        BuildConfig config = availableBuildings[UnityEngine.Random.Range(0, availableBuildings.Count)];
        buildingManager.AddBuild(config);
    }

    public void BuildUnit()
    {

        BuildConfig config = availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
        buildingManager.AddBuild(config);
    }

    public void AddBuilding(CityStateBuildConfig buildConfig)
    {
        CityStateBuilding building = gameController.CreateCityStateBuilding(buildConfig);
        building.CityBuildIn = this;
        buildings.Add(building);
        RefreshYields();
    }

    public void RemoveBuilding(CityStateBuilding cityStateBuilding)
    {
        buildings.Remove(cityStateBuilding);
        RefreshYields();
    }

    public void RefreshYields()
    {
        currentStrength = baseStrength;
        currentProduction = baseProduction;
        currentFood = baseFood;
        currentFood -= foodConsumption;
        currentScience = baseScience;
        currentIncome = baseIncome;

        foreach(CityStateBuilding building in buildings)
        {
            currentStrength += (int)building.ResourceBenefit.Defence.y;
            currentStrength += (int)((float)baseStrength * (building.ResourceBenefit.Defence.x /100.0f));

            currentIncome += (int)building.ResourceBenefit.Gold.y;
            currentIncome += (int)((float)baseIncome * (building.ResourceBenefit.Gold.x / 100.0f));

            currentFood += (int)building.ResourceBenefit.Food.y;
            currentFood += (int)((float)baseFood * (building.ResourceBenefit.Food.x / 100.0f));

            currentScience += (int)building.ResourceBenefit.Science.y;
            currentScience += (int)((float)baseScience * (building.ResourceBenefit.Science.x / 100.0f));

            currentProduction += (int)building.ResourceBenefit.Production.y;
            currentProduction += (int)((float)baseProduction * (building.ResourceBenefit.Production.x / 100.0f));
        }

        ResourceBenefit playerBenefits = PlayerBuildingControl.GetTotalEffects();
        currentStrength += (int)playerBenefits.Defence.y;
        currentStrength += (int)((float)baseStrength * (playerBenefits.Defence.x / 100.0f));

        currentIncome += (int)playerBenefits.Gold.y;
        currentIncome += (int)((float)baseIncome * (playerBenefits.Gold.x / 100.0f));

        currentFood += (int)playerBenefits.Food.y;
        currentFood += (int)((float)baseFood * (playerBenefits.Food.x / 100.0f));

        currentScience += (int)playerBenefits.Science.y;
        currentScience += (int)((float)baseScience * (playerBenefits.Science.x / 100.0f));

        currentProduction += (int)playerBenefits.Production.y;
        currentProduction += (int)((float)baseProduction * (playerBenefits.Production.x / 100.0f));

        currentPlayerIncome = (int)(playerBenefits.PlayerGold.y);

        unitCap = GameConsts.baseUnitCap + playerBenefits.UnitCap;
        if(capital)
        {
            unitCap += 2;
        }
        NotifyInfoChange();
    }

    public int GetIncome()
    {
        return currentIncome;
    }

    public int GetPlayerIncome()
    {
        return currentPlayerIncome;
    }

    public void Rebel()
    {
        CityState cityState = gameController.CreateCityState();
        SetCityState(cityState);
        hexCell.TextEffectHandler.AddTextEffect("REBELLION", GetHexCell().transform, Color.red);
    }

    public void Save(BinaryWriter writer)
    {
        GetHexCell().coordinates.Save(writer);
        writer.Write(cityStateOwner.CityStateID);
        if(player)
        {
            writer.Write(player.PlayerNumber);
        }
        else
        {
            writer.Write(-1);
        }
        buildingManager.Save(writer);
        playerBuildingControl.Save(writer);
        writer.Write(food);
        writer.Write(population);
        writer.Write(cityUnits.Count);
        foreach(CombatUnit unit in cityUnits)
        {
            unit.Save(writer);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        int cityStateID = reader.ReadInt32();
        CityState cityState = gameController.GetCityState(cityStateID);
        int playerNumber = reader.ReadInt32();
        HexCell cell = hexGrid.GetCell(coordinates);
        City city = gameController.CreateCity(cell, cityState);
        if (playerNumber != -1)
        {
            city.Player = gameController.GetPlayer(playerNumber);
        }
        city.buildingManager.Load(reader,gameController,header);
        city.PlayerBuildingControl.Load(reader,gameController,header);
        city.food = reader.ReadInt32();
        city.Population = reader.ReadInt32();
        city.foodConsumption = city.population;
        city.CalculateFoodForNextPop();
        for (int a = 1; a < city.population; a++)
        {
            city.AddCellYield(city.GetHexCell());
        }
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, gameController, hexGrid, header, city);
        }

    }
    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }


}
