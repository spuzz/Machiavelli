﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class HexCell : MonoBehaviour {

    [SerializeField] HexCell[] neighbors;

    [SerializeField] bool[] roads;

    [SerializeField] int production = 2;
    [SerializeField] int food = 2;
    [SerializeField] int income = 2;

    public HexCoordinates coordinates;

	public RectTransform uiRect;

	public HexGridChunk chunk;

	public int Index { get; set; }

	public int ColumnIndex { get; set; }
    int terrainTypeIndex;

    int elevation = int.MinValue;
    int waterLevel;

    int urbanLevel, farmLevel, plantLevel;

    int specialIndex;

    int distance;

    int visibility;

    bool explored;

    bool walled;

    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

    Color cellColor = Color.black;
    Color cellSecondColor = Color.black;

    public List<HexUnit> hexUnits = new List<HexUnit>();
    public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			elevation = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			RefreshPosition();
			ValidateRivers();

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
					SetRoad(i, false);
				}
			}

			Refresh();
		}
	}

	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			waterLevel = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			ValidateRivers();
			Refresh();
		}
	}

    
    public Color CellColor
    {
        get
        {
            return cellColor;
        }

        set
        {
            cellColor = value;
            Refresh();
        }
    }

    
    public Color CellSecondColor
    {
        get
        {
            return cellSecondColor;
        }

        set
        {
            cellSecondColor = value;
            Refresh();
        }
    }


    public int ViewElevation {
		get {
			return elevation >= waterLevel ? elevation : waterLevel;
		}
	}

	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}

	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}

	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}

	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}

	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}

	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}

	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}


	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}

	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				//RemoveRoads();
				RefreshSelfOnly();
			}
		}
	}

	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}


    public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh();
			}
		}
	}

	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
				ShaderData.RefreshTerrain(this);
			}
		}
	}

	public bool IsVisible {
		get {
			return visibility > 0 && Explorable;
		}
	}

	public bool IsExplored {
		get {
			return explored && Explorable;
		}
		private set {
			explored = value;
		}
	}

	public bool Explorable { get; set; }

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
		}
	}

    

	public HexCell PathFrom { get; set; }

	public int SearchHeuristic { get; set; }

	public int SearchPriority {
		get {
			return distance + SearchHeuristic;
		}
	}

	public int SearchPhase { get; set; }

	public HexCell NextWithSamePriority { get; set; }

	public HexCellShaderData ShaderData { get; set; }


    City city;
    public City City
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
            if(IsVisible)
            {
                city.HexVision.Visible = true;
            }
        }
    }

    OperationCentre opCentre;
    public OperationCentre OpCentre
    {
        get
        {
            return opCentre;
        }

        set
        {
            opCentre = value;
            if (IsVisible)
            {
                opCentre.HexVision.Visible = true;
            }
        }
    }

    public int Production
    {
        get
        {
            return production;
        }

        set
        {
            production = value;
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

    public int Income
    {
        get
        {
            return income;
        }

        set
        {
            income = value;
        }
    }


    public void UpdateVision()
    {
        if (visibility > 0)
        {
            if (City)
            {
                City.HexVision.Visible = true;
            }
            bool lastVisibleUnit = false;
            for (int a = hexUnits.Count - 1; a >= 0; a--)
            {

                if (lastVisibleUnit == false)
                {
                    if (hexUnits[a].transform.localPosition == this.Position)
                    {
                        lastVisibleUnit = true;
                        hexUnits[a].HexVision.Visible = true;
                           

                    }
                }
                else
                {
                    if(hexUnits[a].HexVision.HasVision == false)
                    {
                        hexUnits[a].HexVision.Visible = false;
                    }
                    
                }
            }
        }
        else
        {
            if (City)
            {
                City.HexVision.Visible = false;
            }
            foreach(HexUnit unit in hexUnits)
            {
                if (unit.HexVision.HasVision == false)
                {
                    unit.HexVision.Visible = false;

                }
            }
        }
    }

    public void IncreaseVisibility (bool editMode) {
		visibility += 1;
		if (visibility == 1)
        {
            if(editMode == false)
            {
                IsExplored = true;
            }
            
            ShaderData.RefreshVisibility(this);
            UpdateVision();
        }
    }

    public void DecreaseVisibility () {
		visibility -= 1;
		if (visibility == 0) {
			ShaderData.RefreshVisibility(this);
            UpdateVision();
        }
	}

	public void ResetVisibility () {
		if (visibility > 0) {
			visibility = 0;
			ShaderData.RefreshVisibility(this);
            UpdateVision();
        }
	}

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

    public HexDirection GetNeighborDirection(HexCell neighbour)
    {
        int direction = 0;
        foreach( HexCell cell in neighbors)
        {
            if (cell == neighbour)
                return  (HexDirection)direction;
            direction++;
        }
        throw new ArgumentException();
    }


    public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}

	public bool HasRiverThroughEdge (HexDirection direction) {
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveRiver () {
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}

	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!IsValidRiverDestination(neighbor)) {
			return;
		}

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.specialIndex = 0;

		SetRoad((int)direction, false);
	}

	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int)direction];
	}

	public void AddRoad (HexDirection direction) {
		if (
			!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
			GetElevationDifference(direction) <= 1
		) {
			SetRoad((int)direction, true);
		}
	}

	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				SetRoad(i, false);
			}
		}
	}

	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}

	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}

	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination(GetNeighbor(outgoingRiver))
		) {
			RemoveOutgoingRiver();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
		) {
			RemoveIncomingRiver();
		}
	}

	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
		neighbors[index].RefreshSelfOnly();
		RefreshSelfOnly();
	}

    public bool CanUnitMoveToCell(HexUnit.UnitType unitType)
    {
        HexUnit hexUnit = hexUnits.Find(c => c.HexUnitType == unitType);
        if (hexUnit)
        {
            return false;
        }
        return true;
    }


    public bool CanUnitMoveToCell(HexUnit unit)
    {
        HexUnit hexUnit = hexUnits.Find(c => c.HexUnitType == unit.HexUnitType);
        if (hexUnit)
        {
            return false;
        }
        if(City && unit.HexUnitType == HexUnit.UnitType.COMBAT && unit.GetComponent<Unit>().CityState != City.GetCityState())
        {
            return false;
        }
        return true;
    }


    public HexUnit GetFightableUnit(HexUnit unit)
    {
        foreach(HexUnit hexUnit in hexUnits)
        {
            if(hexUnit.GetComponent<Unit>().CanAttack(unit.GetComponent<Unit>()))
            {
                return hexUnit;
            }
        }
        return null;
    }
    


    public void AddUnit(HexUnit hexUnit)
    {
        hexUnits.Add(hexUnit);
        hexUnits = hexUnits.OrderBy(u => u.HexUnitType).ToList();
        UpdateVision();
    }

    public void RemoveUnit(HexUnit hexUnit)
    {
        hexUnits.Remove(hexUnit);
        hexUnits = hexUnits.OrderBy(u => u.HexUnitType).ToList();
        UpdateVision();
    }

    public HexUnit GetTopUnit()
    {
        if(hexUnits.Count == 0)
        {
            return null;
            
        }
        return hexUnits[hexUnits.Count - 1];

    }

    void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}

	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
			if (hexUnits.Count > 0) {
                foreach(HexUnit unit in hexUnits)
                {
                    unit.ValidateLocation();
                }
				
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
        if (hexUnits.Count > 0)
        {
            foreach (HexUnit unit in hexUnits)
            {
                unit.ValidateLocation();
            }

        }
    }

	public void Save (BinaryWriter writer) {
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)(elevation + 127));
		writer.Write((byte)waterLevel);
		writer.Write((byte)urbanLevel);
		writer.Write((byte)farmLevel);
		writer.Write((byte)plantLevel);
		writer.Write((byte)specialIndex);
		writer.Write(walled);

		if (hasIncomingRiver) {
			writer.Write((byte)(incomingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		if (hasOutgoingRiver) {
			writer.Write((byte)(outgoingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

		int roadFlags = 0;
		for (int i = 0; i < roads.Length; i++) {
			if (roads[i]) {
				roadFlags |= 1 << i;
			}
		}
		writer.Write((byte)roadFlags);
		writer.Write(IsExplored);

	}

	public void Load (BinaryReader reader, int header) {
		terrainTypeIndex = reader.ReadByte();
		ShaderData.RefreshTerrain(this);
		elevation = reader.ReadByte();
		elevation -= 127;
		RefreshPosition();
		waterLevel = reader.ReadByte();
		urbanLevel = reader.ReadByte();
		farmLevel = reader.ReadByte();
		plantLevel = reader.ReadByte();
		specialIndex = reader.ReadByte();
		walled = reader.ReadBoolean();

		byte riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasIncomingRiver = false;
		}

		riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasOutgoingRiver = false;
		}

		int roadFlags = reader.ReadByte();
		for (int i = 0; i < roads.Length; i++) {
			roads[i] = (roadFlags & (1 << i)) != 0;
		}

        IsExplored = reader.ReadBoolean();
		ShaderData.RefreshVisibility(this);
	}

	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}

	public void DisableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}

	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.color = color;
		highlight.enabled = true;
	}

	public void SetMapData (float data) {
		ShaderData.SetMapData(this, data);
	}
}