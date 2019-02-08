using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class HexUnit : MonoBehaviour {

    public enum UnitType
    {
        COMBAT,
        AGENT
    }


    const float rotationSpeed = 540f;
	const float travelSpeed = 2f;
    const float fightSpeed = 3f;

    [SerializeField] Transform meshChild;

    HexCell location, currentTravelLocation;
    Animator animator;
    bool controllable = false;
    float orientation;
    HexCell attackCell;
    private string unitPrefabName;
    bool visible = false;
    private int speed = 0;
    UnitType hexUnitType = UnitType.COMBAT;
    public List<HexCell> pathToTravel = null;
    public HexGrid Grid { get; set; }

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                ChangeVisibility(location, false);
                location.RemoveUnit(this);
            }
            location = value;
            value.AddUnit(this);
            ChangeVisibility(value, true);
            transform.localPosition = value.Position;
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }

    public string UnitPrefabName
    {
        get
        {
            return unitPrefabName;
        }
        set
        {
            unitPrefabName = value;
        }
    }

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public int Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public int VisionRange
    {
        get
        {
            return 2;
        }

    }

    public bool Visible
    {
        get { return visible; }
        set
        {
            if (visible != value)
            {
                if (Grid && visible == true)
                {
                    Grid.DecreaseVisibility(location, VisionRange);
                }
                else if (Grid && visible == false)
                {
                    Grid.IncreaseVisibility(location, VisionRange);
                }
                visible = value;
            }
        }
    }

    public bool Controllable
    {
        get
        {
            return controllable;
        }

        set
        {
            controllable = value;
        }
    }

    public UnitType HexUnitType
    {
        get
        {
            return hexUnitType;
        }

        set
        {
            hexUnitType = value;
        }
    }
    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        visible = false;
    }



    public void EnableMesh(bool enable)
    {
        if(enable == true)
        {
            meshChild.gameObject.SetActive(true);
        }
        else
        {
            meshChild.gameObject.SetActive(false);
        }
        
    }

	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	public bool IsValidDestination (HexCell cell, bool allowUnxplored = false) {
		return (cell.IsExplored || allowUnxplored) && !cell.IsUnderwater && cell.CanUnitMoveToCell(this);
	}


    public bool IsValidAttackDestination(HexCell cell)
    {
        if(cell.City && hexUnitType == UnitType.COMBAT && cell.City.GetCityState() != GetComponent<Unit>().CityState)
        {
            return true;
        }
        if(cell.GetFightableUnit(this))
        {
            return true;
        }
        return false;
    }

    public void Travel (List<HexCell> path, HexCell attackCell = null) {
        this.attackCell = attackCell;
        location.RemoveUnit(this);
        if(attackCell)
        {
            location = path[path.Count - 2];
        }
        else
        {
            location = path[path.Count - 1];
        }
		
        location.AddUnit(this);
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}

	IEnumerator TravelPath () {

        Vector3 lookTowards = new Vector3(0, 0, 0);
        if (!currentTravelLocation)
        {
            currentTravelLocation = pathToTravel[0];
        }
        int currentColumn = currentTravelLocation.ColumnIndex;
        int nextColumn;
        if (Location.IsVisible == true || Visible)
        {

            Vector3 a, b, c = pathToTravel[0].Position;
            yield return LookAt(pathToTravel[1].Position);
            animator.SetBool("Walking", true);


            ChangeVisibility(currentTravelLocation, false);
            if (currentTravelLocation.IsVisible == true || Visible)
            {
                EnableMesh(true);
            }
            

            float t = Time.deltaTime * travelSpeed;
            for (int i = 1; i < pathToTravel.Count; i++)
            {
                currentTravelLocation = pathToTravel[i];
                a = c;
                b = pathToTravel[i - 1].Position;

                nextColumn = currentTravelLocation.ColumnIndex;
                if (currentColumn != nextColumn)
                {
                    if (nextColumn < currentColumn - 1)
                    {
                        a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                        b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    else if (nextColumn > currentColumn + 1)
                    {
                        a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                        b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    Grid.MakeChildOfColumn(transform, nextColumn);
                    currentColumn = nextColumn;
                }

                c = (b + currentTravelLocation.Position) * 0.5f;

                //if(pathToTravel[i].IsVisible)
                //{
                //    EnableMesh(true);
                //}
                //else
                //{
                //    EnableMesh(false);
                //}
                ChangeVisibility(pathToTravel[i], true);
                if (currentTravelLocation.IsVisible == true)
                {

                    for (; t < 1f; t += Time.deltaTime * travelSpeed)
                    {
                        transform.localPosition = Bezier.GetPoint(a, b, c, t);
                        Vector3 d = Bezier.GetDerivative(a, b, c, t);
                        d.y = 0f;
                        transform.localRotation = Quaternion.LookRotation(d);
                        yield return null;
                    }

                    t -= 1f;
                }
                ChangeVisibility(pathToTravel[i], false);
                if (currentTravelLocation.IsVisible == true || Visible)
                {
                    EnableMesh(true);
                }

            }
            a = c;
            b = Location.Position;
            c = b;
            
            ChangeVisibility(location, true);

            //if (location.IsVisible)
            //{
            //    EnableMesh(true);
            //}


            if (attackCell)
            {
                City city = attackCell.City;
                if(city)
                {
                    lookTowards = attackCell.Position;
                    float attackTime = 0;
                    animator.SetBool("Attacking", true);
                    for (; attackTime < fightSpeed; attackTime += Time.deltaTime)
                    {
                        yield return null;
                    }
                    animator.SetBool("Attacking", false);

                    if (city.HitPoints <= 0)
                    {
                        city.GetCityState().KillLocalUnits(city);
                        city.SetCityState(GetComponent<Unit>().CityState);
                    }
                    city.UpdateUI();
                }
                else
                {
                    HexUnit unitToFight = attackCell.GetFightableUnit(this);
                    lookTowards = unitToFight.Location.Position;
                    //yield return LookAt(attackCell.Position);
                    yield return unitToFight.LookAt(location.Position);
                    if (unitToFight)
                    {
                        float attackTime = 0;
                        animator.SetBool("Attacking", true);
                        unitToFight.animator.SetBool("Attacking", true);
                        for (; attackTime < fightSpeed; attackTime += Time.deltaTime)
                        {
                            yield return null;
                        }
                        animator.SetBool("Attacking", false);
                        unitToFight.animator.SetBool("Attacking", false);
                        
                        unitToFight.GetComponent<Unit>().UpdateUI();
                        if (unitToFight.GetComponent<Unit>().HitPoints <= 0)
                        {
                            unitToFight.DieAnimationAndRemove();
                        }

                    }
                }
                GetComponent<Unit>().UpdateUI();
                if (GetComponent<Unit>().HitPoints <= 0)
                {
                    DieAnimationAndRemove();
                }


               
                nextColumn = Location.ColumnIndex;
                
                if (currentColumn != nextColumn)
                {
                    if (nextColumn < currentColumn - 1)
                    {
                        a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    else if (nextColumn > currentColumn + 1)
                    {
                        a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    Grid.MakeChildOfColumn(transform, nextColumn);
                    currentColumn = nextColumn;
                }

            }



            currentTravelLocation = null;

            for (; t < 1f; t += Time.deltaTime * (travelSpeed * 2))
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
        }
        else
        {
            currentTravelLocation = null;
            nextColumn = Location.ColumnIndex;
            if (currentColumn != nextColumn)
            {
                Grid.MakeChildOfColumn(transform, nextColumn);
                currentColumn = nextColumn;
            }
        }

        transform.localPosition = location.Position;
        location.UpdateVision();
        orientation = transform.localRotation.eulerAngles.y;
        if (attackCell)
        {
            yield return LookAt(lookTowards);
        }
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        animator.SetBool("Walking", false);
    }

    IEnumerator LookAt (Vector3 point) {
		if (HexMetrics.Wrapping) {
			float xDistance = point.x - transform.localPosition.x;
			if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
			else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
		}

		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		//transform.LookAt(point);
		Orientation = transform.localRotation.eulerAngles.y;
	}

	public int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction, bool allowUnexplored = false)
	{
		if (!IsValidDestination(toCell, allowUnexplored) && !IsValidAttackDestination(toCell)) {
			return -1;
		}
		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) {
			return -1;
		}
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 3;
		}
		else if (fromCell.Walled != toCell.Walled) {
			return 10;
		}
		else {
			moveCost = (edgeType == HexEdgeType.Flat || fromCell.Elevation > toCell.Elevation) ? 5 : 10;
			//moveCost +=
			//	toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
	}

    private void ChangeVisibility(HexCell cell, bool increase)
    {
        if(Visible)
        {
            if (increase)
            {
                Grid.IncreaseVisibility(cell, VisionRange);
            }
            else
            {
                Grid.DecreaseVisibility(cell, VisionRange);
            }
        }
        GetComponent<Unit>().UpdateOwnerVisiblity(cell, increase);
    }

    public void KillUnit()
    {
        Grid.RemoveUnit(this);
    }
	public void Die () {
        GetComponent<Unit>().KillUnit();
        if (location) {
            ChangeVisibility(location, false);
            location.RemoveUnit(this);
        }
       
    }

    public void DieAnimationAndRemove()
    {
        StartCoroutine(Death());
        
    }

    public void DieAndRemove()
    {
        Die();
        Destroy(gameObject);
    }

    IEnumerator Death()
    {
        Die();
        animator.SetBool("Dying", true);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
                ChangeVisibility(location, true);
                ChangeVisibility(currentTravelLocation, false);
				currentTravelLocation = null;
			}
		}
	}

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(orientation);
        writer.Write(UnitPrefabName);
    }

    public static HexUnit Load(BinaryReader reader, HexGrid grid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        string unitName = reader.ReadString();
        HexUnit unit;
        unit = Instantiate(Resources.Load(unitName) as GameObject).GetComponent<HexUnit>();
        unit.UnitPrefabName = unitName;
        grid.AddUnit(unit, grid.GetCell(coordinates), orientation);
        return unit;
    }

    //	void OnDrawGizmos () {
    //		if (pathToTravel == null || pathToTravel.Count == 0) {
    //			return;
    //		}
    //
    //		Vector3 a, b, c = pathToTravel[0].Position;
    //
    //		for (int i = 1; i < pathToTravel.Count; i++) {
    //			a = c;
    //			b = pathToTravel[i - 1].Position;
    //			c = (b + pathToTravel[i].Position) * 0.5f;
    //			for (float t = 0f; t < 1f; t += 0.1f) {
    //				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //			}
    //		}
    //
    //		a = c;
    //		b = pathToTravel[pathToTravel.Count - 1].Position;
    //		c = b;
    //		for (float t = 0f; t < 1f; t += 0.1f) {
    //			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //		}
    //	}
}