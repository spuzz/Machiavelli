using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatUnit : Unit
{
    [SerializeField] Texture mercBackground;

    public enum Stance
    {
        UNASSIGNED,
        OFFENCE,
        DEFENCE,
        EXPLORE
    }

    Stance currentStance = Stance.UNASSIGNED;
    CombatUnitConfig combatUnitConfig;
    bool mercenary;
    City cityOwner;
    Player player;

    public Stance CurrentStance
    {
        get { return currentStance; }
        set { currentStance = value; }
    }

    public bool Mercenary
    {
        get
        {
            return mercenary;
        }

        set
        {
            mercenary = value;
            if(mercenary == true)
            {
                BackGround = mercBackground;
            }
        }
    }

    public City CityOwner
    {
        get
        {
            return cityOwner;
        }

        set
        {
            if (cityOwner)
            {
                UpdateOwnerVisiblity(HexUnit.Location, false);
            }

            cityOwner = value;
            UpdateOwnerVisiblity(HexUnit.Location, true);
            if (unitUI)
            {
                if (GetCityState())
                {
                    unitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(GetCityState().SymbolID));

                }
                else
                {
                    unitUI.SetCityStateSymbolToDefault();
                }
            }
            UpdateColours();
        }
    }

    public void SetPlayer(Player player)
    {
        if (player)
        {
            UpdateOwnerVisiblity(HexUnit.Location, false);
        }
        this.player = player;

        UpdateOwnerVisiblity(HexUnit.Location, true);
        UpdateColours();

    }
    public override Player GetPlayer()
    {
        return player;
    }

    public override City GetCityOwner()
    {
        return cityOwner;
    }

    public void SetCombatUnitConfig(CombatUnitConfig config)
    {
        combatUnitConfig = config;
        HexUnit.SetMeshChild(Instantiate(config.MeshChild, gameObject.transform).transform);
        HexVision.AddVisibleObject(HexUnit.GetMesh());
        BaseMovement = config.BaseMovement;
        BaseStrength = config.BaseStrength;
        BaseRangeStrength = config.BaseRangeStrength;
        Range = config.Range;
        Symbol = config.Symbol;
        foreach (AbilityConfig abilityConfig in config.GetAbilityConfigs())
        {
            abilities.AbilitiesList.Add(abilityConfig);
        }

    }

    public override void Setup()
    {
        if (CityOwner)
        {
            
            unitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(GetCityState().SymbolID));
            UpdateColours();
        }

    }
    public CombatUnitConfig GetCombatUnitConfig()
    {
        return combatUnitConfig;
    }

    public override bool CanAttack(Unit unit)
    {
        if(unit.GetCityState() && unit.GetCityState() != GetCityState())
        {
            return true;
        }
        return false;
    }

    public void Save(BinaryWriter writer)
    {
        GetComponent<HexUnit>().Save(writer);
        writer.Write(HitPoints);
        writer.Write(GetMovementLeft());
        writer.Write(combatUnitConfig.Name);
    }

    public static CombatUnit Load(BinaryReader reader,GameController gameController, HexGrid grid, int header, City city)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        int hitPoints = 100;
        int movementLeft = 2;
        string unitName = reader.ReadString();
        if (header >= 3)
        {
            hitPoints = reader.ReadInt32();
            movementLeft = reader.ReadInt32();
        }

        string combatUnitConfig = "Swordsman";
        if (header >= 4)
        {
            combatUnitConfig = reader.ReadString();
        }


        HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, grid.GetCell(coordinates), city);
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        if (header >= 3)
        {
            combatUnit.HitPoints = hitPoints;
            combatUnit.SetMovementLeft(movementLeft);
        }
        return combatUnit;
    }
}