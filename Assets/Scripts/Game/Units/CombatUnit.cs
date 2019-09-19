using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatUnit : Unit
{
    [SerializeField] Texture mercBackground;

    CombatUnitConfig combatUnitConfig;
    City cityOwner;
    Player player;

    public enum CombatUnitType
    {
        MELEE,
        SUPPORT,
        SIEGE
    }

    public enum CombatClassification
    {
        LIGHTCAVALRY,
        HEAVYCAVALRY,
        SPEARMEN,
        SWORDSMAN,
        AXEMEN,
        SUPPORT,
        SIEGE
        
    }

    CombatUnitType combatUnitType;

    public CombatUnitType CombatType
    {
        get
        {
            return combatUnitType;
        }

        set
        {
            combatUnitType = value;
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
        if (UnitUI)
        {
            UnitUI.SetColour(player.GetColour().Colour);
        }
        MaterialColourChanger changer = HexUnit.MaterialColourChanger;
        changer.ChangeMaterial(player.GetColour());

    }
    public override Player GetPlayer()
    {
        return player;
    }


    public void SetCombatUnitConfig(CombatUnitConfig config)
    {
        combatUnitConfig = config;
        HexUnit.SetMeshChild(Instantiate(config.MeshChild, gameObject.transform).transform);
        HexVision.AddVisibleObject(HexUnit.GetMesh());
        BaseMovement = config.BaseMovement;
        BaseStrength = config.BaseStrength;
        CombatType = config.CombatUnitType;
        UnitUI.SetUnitSymbol(config.Symbol);
        // TODO
        //Symbol = config.Symbol;


    }

    public override void UpdateUI(int healthChange)
    {
        base.UpdateUI(healthChange);
    }
    public CombatUnitConfig GetCombatUnitConfig()
    {
        return combatUnitConfig;
    }

    public override bool CanAttack(Unit unit)
    {
        if(unit.CityStateOwner && unit.CityStateOwner != CityStateOwner)
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