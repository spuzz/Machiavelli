using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class HexCell : MonoBehaviour {
    [SerializeField] HexCell[] neighbors;

    [SerializeField] bool[] roads;

    [SerializeField] PlayerColour playerColour;
    [SerializeField] PlayerColour defaultPlayerColour;
    [SerializeField] HexCellTextEffectHandler textEffectHandler;
    [SerializeField] HexCellGameData hexCellGameData;
    [SerializeField] HexCellUI hexCellUI;
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

    bool forest;

    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;


    public HexUnit combatUnit;
    public HexUnit agent;

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

    
    public PlayerColour PlayerColour
    {
        get
        {
            return playerColour;
        }

        set
        {
            playerColour = value;
            if(playerColour == null)
            {
                playerColour = defaultPlayerColour;
            }
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
		set {
			explored = value;
            ShaderData.RefreshVisibility(this);
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

    public HexCellTextEffectHandler TextEffectHandler
    {
        get
        {
            return textEffectHandler;
        }

        set
        {
            textEffectHandler = value;
        }
    }

    public HexCellGameData HexCellGameData
    {
        get
        {
            return hexCellGameData;
        }

        set
        {
            hexCellGameData = value;
        }
    }

    public HexCellUI HexCellUI
    {
        get
        {
            return hexCellUI;
        }

        set
        {
            hexCellUI = value;
        }
    }

    public bool Forest
    {
        get
        {
            return forest;
        }

        set
        {
            forest = value;
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

            SetUnitVisible(combatUnit);
            SetUnitVisible(agent);
        }
        else
        {
            if (City)
            {
                City.HexVision.Visible = false;
            }
            SetUnitInvisible(combatUnit);
            SetUnitInvisible(agent);
        }
    }

    private void SetUnitVisible( HexUnit unit)
    {
        if (unit && Vector3.Distance(unit.transform.localPosition, this.Position + unit.OffSet) < 1)
        {
            unit.HexVision.Visible = true;

        }

    }

    private void SetUnitInvisible(HexUnit unit)
    {
        if (unit && Vector3.Distance(unit.transform.localPosition, this.Position + unit.OffSet) < 1 && unit.HexVision.HasVision == false)
        {
            unit.HexVision.Visible = false;

        }
    }


    public void UpdateUnitPositions()
    {
        if (agent)
        {
            agent.unit.UpdatePositionInCell();
        }

        if (combatUnit)
        {
            combatUnit.unit.UpdatePositionInCell();
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

    public bool CanUnitMoveToCell(Unit.UnitType unitType)
    {
        if(unitType == Unit.UnitType.AGENT && !agent)
        {
            return true;
        }
        else if(unitType == Unit.UnitType.COMBAT && !combatUnit)
        {
            return true;
        }
        return false;
    }


    public bool CanUnitMoveToCell(HexUnit unit)
    {
        return CanUnitMoveToCell(unit.unit.HexUnitType);
    }


    public HexUnit GetFightableUnit(HexUnit unit)
    {
        if (unit.unit.HexUnitType == Unit.UnitType.COMBAT && combatUnit)
        {
            if (unit.GetComponent<Unit>().CanAttack(combatUnit.GetComponent<Unit>()))
            {
                return combatUnit;
            }
        }
        return null;
    }

    public HexUnit GetFightableUnit(List<HexUnit> units)
    {
        foreach (HexUnit hexUnit in units)
        {
            HexUnit targetUnit = GetFightableUnit(hexUnit);
            if(targetUnit)
            {
                return targetUnit;
            }
        }
        return null;
    }

    public void AddUnit(HexUnit hexUnit)
    {
        if(hexUnit.unit.HexUnitType == Unit.UnitType.COMBAT)
        {
            combatUnit = hexUnit;
            combatUnit.unit.UpdateUI(0);
        }
        else if(hexUnit.unit.HexUnitType == Unit.UnitType.AGENT)
        {
            agent = hexUnit;
            agent.unit.UpdateUI(0);
        }
        UpdateVision();
    }

    public void RemoveUnit(HexUnit hexUnit)
    {
        if(combatUnit == hexUnit)
        {
            combatUnit = null;
        }
        else if(agent == hexUnit)
        {
            agent = null;
        }
        UpdateVision();
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
            ValidateUnitLocations();
        }
	}

	void RefreshSelfOnly ()
    {
        chunk.Refresh();
        ValidateUnitLocations();
    }

    private void ValidateUnitLocations()
    {
        if (agent)
        {
            agent.ValidateLocation();
        }
        if (combatUnit)
        {
            combatUnit.ValidateLocation();
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
        writer.Write(forest);
        HexCellGameData.Save(writer);
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
        if(header >= 4)
        {
            forest = reader.ReadBoolean();
        }

        city = null;

        HexCellGameData.Load(reader, header);
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