using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatUnit : Unit
{
    [SerializeField] Texture mercBackground;
    [SerializeField] Material defaultUnitMaterial;
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
    CityState cityState;
    Player player;
    City cityOwner;

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
            cityOwner = value;
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
        if (unitUI)
        {
            unitUI.SetColour(player.GetColour().Colour);
        }
        HexUnit.MaterialColourChanger.ChangeMaterial(player.GetColour());

    }
    public override Player GetPlayer()
    {
        return player;
    }

    public override CityState GetCityState()
    {
        return cityState;
    }
    public override void SetCityState(CityState value)
    {
        if (cityState)
        {
            UpdateOwnerVisiblity(HexUnit.Location, false);
        }


        cityState = value;
        UpdateOwnerVisiblity(HexUnit.Location, true);
        if (unitUI)
        {
            if(cityState)
            {
                unitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(cityState.SymbolID));
                if (!player && cityState.Player)
                {
                    unitUI.SetColour(cityState.Player.GetColour().Colour);
                    HexUnit.MaterialColourChanger.ChangeMaterial(cityState.Player.GetColour());
                }
                else if(!player)
                {
                    HexUnit.MaterialColourChanger.ChangeMaterial(defaultUnitMaterial);
                }

            }
            else
            {
                unitUI.SetCityStateSymbolToDefault();
                HexUnit.MaterialColourChanger.ChangeMaterial(defaultUnitMaterial);
            }
        }
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
        if (cityState)
        {
            unitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(cityState.SymbolID));
            HexUnit.MaterialColourChanger.ChangeMaterial(defaultUnitMaterial);
        }

    }
    public CombatUnitConfig GetCombatUnitConfig()
    {
        return combatUnitConfig;
    }

    public override bool CanAttack(Unit unit)
    {
        if(unit.CityState && unit.CityState != CityState)
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

    public static CombatUnit Load(BinaryReader reader,GameController gameController, HexGrid grid, int header, int cityStateID)
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


        HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, grid.GetCell(coordinates), cityStateID);
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        if (header >= 3)
        {
            combatUnit.HitPoints = hitPoints;
            combatUnit.SetMovementLeft(movementLeft);
        }
        return combatUnit;
    }
}