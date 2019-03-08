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
    private string unitPrefabName = "";
    private int speed = 0;
    UnitType hexUnitType = UnitType.COMBAT;
    public List<HexCell> pathToTravel = null;
    public HexGrid Grid { get; set; }

    Unit unit;

    HexVision hexVision;

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
                if(HexVision)
                {
                    HexVision.ClearCells();

                }

                location.RemoveUnit(this);
                unit.UpdateOwnerVisiblity(location, false);
            }
            location = value;
            if(location)
            {
                if (HexVision)
                {
                    HexVision.AddCells(Grid.GetVisibleCells(location,VisionRange));
                }
                transform.localPosition = value.Position;
                Grid.MakeChildOfColumn(transform, value.ColumnIndex);
                value.AddUnit(this);
                unit.UpdateOwnerVisiblity(location, true);
                unit.NotifyInfoChange();
            }

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

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        unit = GetComponent<Unit>();
    }



    public GameObject GetMesh()
    {
        if(!meshChild)
        {
            return null;
        }
        return meshChild.gameObject;
    }

    public void SetMeshChild(Transform childTransform)
    {
        meshChild = childTransform;
        animator = GetComponentInChildren<Animator>();
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

    public void Travel (List<HexCell> path) {
        location.RemoveUnit(this);
        if(unit.IsSomethingToAttack())
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
        unit.UpdateOwnerVisiblity(currentTravelLocation, true);
        if (currentTravelLocation.IsVisible == true || Location.IsVisible == true || HexVision.HasVision)
        {

            Vector3 a, b, c = pathToTravel[0].Position;
            yield return LookAt(pathToTravel[1].Position);
            animator.SetBool("Walking", true);

            HexVision.ClearCells();
            

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
                HexVision.AddCells(Grid.GetVisibleCells(pathToTravel[i], VisionRange));
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
                HexVision.ClearCells();

            }
            a = c;
            b = Location.Position;
            c = b;

            HexVision.AddCells(Grid.GetVisibleCells(location, VisionRange));

            //if (location.IsVisible)
            //{
            //    EnableMesh(true);
            //}


            if (unit.IsSomethingToAttack())
            {
                
                if(unit.AttackCity)
                {
                    City city = unit.AttackCity;
                    lookTowards = city.GetHexCell().Position;
                    float attackTime = 0;
                    animator.SetBool("Attacking", true);
                    for (; attackTime < fightSpeed; attackTime += Time.deltaTime)
                    {
                        yield return null;
                    }
                    animator.SetBool("Attacking", false);

                    if (city.HitPoints <= 0)
                    {
                        if (hexUnitType == UnitType.COMBAT && GetComponent<CombatUnit>().Mercenary && !GetComponent<Unit>().GetCityState())
                        {
                            GetComponent<Unit>().GetPlayer().Gold += city.Plunder();
                            city.HitPoints = 1;
                        }
                        else
                        {
                            city.GetCityState().KillLocalUnits(city);
                            city.SetCityState(GetComponent<Unit>().CityState);
                            city.HitPoints = 50;
                        }

                    }
                    city.UpdateUI();
                }
                else if(unit.AttackUnit)
                {
                    HexUnit unitToFight = unit.AttackUnit;
                    lookTowards = unitToFight.Location.Position;
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
                        Unit unitToFightUnitComp = unitToFight.GetComponent<Unit>();
                        unitToFightUnitComp.ShowHealthChange(unitToFightUnitComp.GetLastHitPointChange());
                        unitToFightUnitComp.UpdateUI();
                        if (unitToFightUnitComp.HitPoints <= 0)
                        {
                            unitToFight.DieAnimationAndRemove();
                        }

                    }
                }
                Unit unitComp = GetComponent<Unit>();
                unitComp.ShowHealthChange(unitComp.GetLastHitPointChange());
                unitComp.UpdateUI();
                if (unitComp.HitPoints <= 0)
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
        unit.UpdateOwnerVisiblity(location, true);
        orientation = transform.localRotation.eulerAngles.y;
        if (unit.IsSomethingToAttack())
        {
            yield return LookAt(lookTowards);
        }
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        if(HexVision.Visible)
        {
            animator.SetBool("Walking", false);
        }
        
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


    public void KillUnit()
    {
        Grid.RemoveUnit(this);
        location.RemoveUnit(this);
    }

    public void DieAnimationAndRemove()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        animator.SetBool("Dying", true);
        yield return new WaitForSeconds(2);
        DestroyHexUnit();
    }

    public void DestroyHexUnit()
    {
        Location = null;
        Destroy(gameObject);
    }

    void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
                HexVision.ClearCells();
                HexVision.AddCells(Grid.GetVisibleCells(location, VisionRange));
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

    public static void Load(BinaryReader reader, GameController gameController, HexGrid grid, int header)
    {

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