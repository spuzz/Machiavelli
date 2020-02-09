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
            //if (CombatType == CombatUnit.CombatUnitType.SUPPORT)
            //{
            //    HexUnit.OffSet = new Vector3(4, 0, 0);
            //}
            //if (CombatType == CombatUnit.CombatUnitType.SIEGE)
            //{
            //    HexUnit.OffSet = new Vector3(-4, 0, 0);
            //}
        }
    }

    public void SetPlayer(Player ply)
    {
        if (player)
        {

            UpdateOwnerVisiblity(HexUnit.Location, false);
        }

        if (ply)
        {
            player = ply;
            if (ply.IsHuman)
            {
                HexUnit.Controllable = true;
            }
            else
            {
                HexUnit.Controllable = false;
            }

            UpdateOwnerVisiblity(HexUnit.Location, true);
            if (UnitUI)
            {
                UnitUI.SetColour(ply.GetColour().Colour);
            }
            MaterialColourChanger changer = HexUnit.MaterialColourChanger;
            changer.ChangeMaterial(ply.GetColour());
        }
        else
        {
            if (UnitUI)
            {
                UnitUI.SetColour(Color.white);
            }
            MaterialColourChanger changer = HexUnit.MaterialColourChanger;
            changer.ChangeMaterial(gameController.DefaultColour);
        }


    }
    public override Player GetPlayer()
    {
        return player;
    }

    public override City GetCityOwner()
    {
        return cityOwner;
    }

    public void SetCityOwner(City city)
    {
        cityOwner = city;
    }
    //public override Vector3 GetPositionInCell(HexCell cell)
    //{
    //    if (CombatType != CombatUnitType.MELEE)
    //    {
    //        HexUnit meleeUnit = cell.hexUnits.Find(c => c.unit.HexUnitType == UnitType.COMBAT && (c.unit as CombatUnit).CombatType == CombatUnitType.MELEE);
    //        if (meleeUnit)
    //        {
    //            transform.rotation = meleeUnit.transform.rotation;
    //            if (CombatType == CombatUnitType.SUPPORT)
    //            {
    //                HexUnit.OffSet = meleeUnit.transform.rotation * GameConsts.supportBaseOffset;
    //                return meleeUnit.transform.localPosition + HexUnit.OffSet;
    //            }
    //        }
    //        else
    //        {
    //            return cell.Position;
    //        }
    //    }
    //    return cell.Position;
    //}

    //public override Vector3 GetFightPosition(HexCell cell, HexCell targetCell)
    //{
    //    if (CombatType != CombatUnitType.MELEE)
    //    {
    //        int count = 1;
    //        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
    //        {
    //            if (cell.GetNeighbor(d) == targetCell)
    //            {
    //                HexUnit.OffSet = Quaternion.Euler(0,90,0) * GameConsts.supportBaseOffset;
    //                return cell.transform.localPosition + HexUnit.OffSet;
    //            }
    //        }
    //    }
    //    return cell.Position;
    //}

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
        //string unitName = reader.ReadString();

        hitPoints = reader.ReadInt32();
        movementLeft = reader.ReadInt32();

        string combatUnitConfig = "Swordsman";
        combatUnitConfig = reader.ReadString();

        HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, grid.GetCell(coordinates), city);
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();

        combatUnit.HitPoints = hitPoints;
        combatUnit.SetMovementLeft(movementLeft);
        return combatUnit;
    }
}