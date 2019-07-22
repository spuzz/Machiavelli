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
	const float travelSpeed = 3f;
    const float fightSpeed = 1f;

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

    public Unit unit;

    HexVision hexVision;

    HexUnitActionController hexUnitActionController;
    MaterialColourChanger materialColourChanger;
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

    public Animator Animator
    {
        get
        {
            return animator;
        }

        set
        {
            animator = value;
        }
    }

    public static float TravelSpeed
    {
        get
        {
            return travelSpeed;
        }
    }

    public MaterialColourChanger MaterialColourChanger
    {
        get
        {
            return materialColourChanger;
        }

        set
        {
            materialColourChanger = value;
        }
    }

    public void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        unit = GetComponent<Unit>();
        hexUnitActionController = FindObjectOfType<HexUnitActionController>();
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
        Animator = GetComponentInChildren<Animator>();
        MaterialColourChanger = GetComponentInChildren<MaterialColourChanger>();
    }
    public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	public bool IsValidDestination (HexCell cell, bool allowUnxplored = false) {
		return (cell.IsExplored || allowUnxplored) && !cell.IsUnderwater && cell.CanUnitMoveToCell(this);
	}


    public bool IsValidAttackDestination(HexCell cell)
    {
        if(cell.City && hexUnitType == UnitType.COMBAT && cell.City.GetCityState() != GetComponent<Unit>().GetCityState())
        {
            return true;
        }
        if(cell.GetFightableUnit(this))
        {
            return true;
        }
        return false;
    }

    public void SetLocationOnly(HexCell cell)
    {
        location = cell;
    }

    public void AddUnitToLocation(HexCell cell)
    {
        location.AddUnit(this);
    }

    public void UpdateUnit(int healthChange, bool killUnit)
    {
        Unit unitComp = GetComponent<Unit>();
        unitComp.ShowHealthChange(healthChange);
        unitComp.UpdateUI(-healthChange);
        if (unitComp.HitPoints <= 0 && killUnit)
        {
            DieAnimationAndRemove();
        }
    }

    public IEnumerator LookAt (Vector3 point) {
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

    public IEnumerator FightCity(City city, bool killTarget, CityState cityState, HexUnit targetUnit)
    {

        float attackTime = 0;
        if ((Location.IsVisible || city.GetHexCell().IsVisible) && GameConsts.playAnimations)
        {
            LookAt(city.GetHexCell().Position);
            Location.IncreaseVisibility(false);
            Animator.SetBool("Attacking", true);
            for (; attackTime < fightSpeed; attackTime += Time.deltaTime)
            {
                yield return null;
            }
            Animator.SetBool("Attacking", false);
            Location.DecreaseVisibility();
        }


        if (killTarget)
        {
            if(cityState.GetCityCount() == 0)
            {
                unit.GameController.DestroyCityState(cityState);
            }

            city.DestroyCityUnits();
            if(targetUnit)
            {
                targetUnit.DestroyHexUnit();
            }

            city.UpdateCityBar();
        }
        city.UpdateHealthBar();
    }

    public IEnumerator FightUnit(HexUnit targetUnit, int damageDone, bool killTarget)
    {

        if (targetUnit)
        {
            Unit unitToFightUnitComp = targetUnit.GetComponent<Unit>();
            if ((Location.IsVisible || targetUnit.Location.IsVisible) && GameConsts.playAnimations)
            {
                yield return targetUnit.LookAt(location.Position);
                Location.IncreaseVisibility(false);
                float attackTime = 0;
                Animator.SetBool("Attacking", true);
                if (unit.Range > 0)
                {
                    Projectile projectile = Instantiate(unit.ProjectilePreFab, gameObject.transform);
                    projectile.StartLocation = gameObject;
                    projectile.Target = targetUnit.gameObject;
                    yield return StartCoroutine(projectile.FlyProjectile());
                    targetUnit.Animator.SetTrigger("GetHit");
                }
                else
                {
                    targetUnit.Animator.SetBool("Attacking", true);
                    for (; attackTime < fightSpeed; attackTime += Time.deltaTime)
                    {
                        yield return null;
                    }
                    targetUnit.Animator.SetBool("Attacking", false);
                }

                Animator.SetBool("Attacking", false);
                unitToFightUnitComp.ShowHealthChange(damageDone);
                Location.DecreaseVisibility();
            }

            unitToFightUnitComp.UpdateUI(-damageDone);
            if (unitToFightUnitComp.HitPoints <= 0 && killTarget)
            {
                if(unitToFightUnitComp.HexUnit.Location.IsVisible)
                {
                    targetUnit.DieAnimationAndRemove();
                }
                else
                {
                    targetUnit.DestroyHexUnit();
                }

            }


        }
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
        Animator.SetTrigger("Dying");
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