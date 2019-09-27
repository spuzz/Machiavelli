using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class HexUnit : MonoBehaviour {


    // External Components
    public HexGrid Grid { get; set; }
    HexUnitActionController hexUnitActionController;

    // Internal Components
    [SerializeField] Transform meshChild;
    public Unit unit;
    HexVision hexVision;
    MaterialColourChanger materialColourChanger;
    Animator animator;
    [SerializeField] AnimatorOverrideController animatorOverrideController;

    // Attributes
    [SerializeField] Projectile projectilePreFab;
    const float rotationSpeed = 540f;
	const float travelSpeed = 3f;


    int visionRange = 0;
    bool controllable = false;
    float orientation;
    private int speed = 0;

    HexCell location, currentTravelLocation;
    float t;
    Vector3 a, b, c = new Vector3();

    public List<HexCell> pathToTravel = null;
    List<HexAction> actions = new List<HexAction>();


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

    public Projectile ProjectilePreFab
    {
        get
        {
            return projectilePreFab;
        }

        set
        {
            projectilePreFab = value;
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
            return visionRange;
        }

        set
        {
            visionRange = value;
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
    public IEnumerator MoveUnit(HexUnit unitToMove, HexCell currentTravelLocation, HexCell newTravelLocation, float t, float percOfFullDistance, bool startMove)
    {
        this.t = t;
        if(startMove)
        {
            a = currentTravelLocation.Position;
            b = currentTravelLocation.Position;
            c = currentTravelLocation.Position;
        }


        a = c;
        b = currentTravelLocation.Position;
        int currentColumn = currentTravelLocation.ColumnIndex;
        int nextColumn;
        nextColumn = newTravelLocation.ColumnIndex;
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
            unitToMove.Grid.MakeChildOfColumn(unitToMove.transform, nextColumn);
            currentColumn = nextColumn;
        }

        c = b + (( newTravelLocation.Position - b) * (0.5f * percOfFullDistance));


        unitToMove.HexVision.AddCells(unitToMove.Grid.GetVisibleCells(newTravelLocation, unitToMove.VisionRange));

        if (currentTravelLocation.IsVisible == true && GameConsts.playAnimations)
        {
            yield return unitToMove.LookAt(newTravelLocation.Position);
            unitToMove.Animator.SetBool("Walking", true);
            unitToMove.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                unitToMove.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                unitToMove.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            unitToMove.Animator.SetBool("Walking", false);
            t -= 1f;
        }
        else
        {
            unitToMove.HexVision.Visible = false;
            unitToMove.transform.localPosition = c;
            t = Time.deltaTime * HexUnit.TravelSpeed;
        }
        unitToMove.HexVision.ClearCells();
        unitToMove.HexVision.AddCells(unitToMove.Grid.GetVisibleCells(newTravelLocation, unitToMove.VisionRange));
    }

    public IEnumerator MoveUnitEnd(HexUnit unitToMove, HexCell currentTravelLocation, HexCell newTravelLocation)
    {
        a = c;
        b = newTravelLocation.Position;
        c = b;
        int currentColumn = currentTravelLocation.ColumnIndex;
        int nextColumn;
        nextColumn = newTravelLocation.ColumnIndex;

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
            unitToMove.Grid.MakeChildOfColumn(unitToMove.transform, nextColumn);
            currentColumn = nextColumn;
        }

        unitToMove.HexVision.AddCells(unitToMove.Grid.GetVisibleCells(newTravelLocation, unitToMove.VisionRange));
        t = Time.deltaTime * HexUnit.TravelSpeed;
        if (newTravelLocation.IsVisible == true && GameConsts.playAnimations)
        {
            yield return unitToMove.LookAt(newTravelLocation.Position);
            unitToMove.Animator.SetBool("Walking", true);
            unitToMove.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                unitToMove.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                unitToMove.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;

            }
            unitToMove.Animator.SetBool("Walking", false);
            t -= 1f;
        }
        else
        {
            unitToMove.HexVision.Visible = false;
            unitToMove.transform.localPosition = c;
            t = Time.deltaTime * HexUnit.TravelSpeed;
        }


        unitToMove.HexVision.ClearCells();
        unitToMove.HexVision.AddCells(unitToMove.Grid.GetVisibleCells(newTravelLocation, unitToMove.VisionRange));
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

    public IEnumerator Fight(HexCell target)
    {

        float attackTime = 0;
        if ((Location.IsVisible || target.IsVisible) && GameConsts.playAnimations)
        {

            LookAt(target.Position);
            Location.IncreaseVisibility(false);
            Animator.SetBool("Attacking", true);
            for (; attackTime < GameConsts.fightSpeed; attackTime += Time.deltaTime)
            {
                yield return null;
            }
            Animator.SetBool("Attacking", false);
            Location.DecreaseVisibility();
        }
    }

    public void Move(List<HexCell> moves)
    {

        HexAction action = hexUnitActionController.CreateAction();
        action.ActionsUnit = this;
        action.AddAction(moves);

        Location.RemoveUnit(this);
        SetLocationOnly(moves[moves.Count - 1]);
        if (unit.Alive == true)
        {
            AddUnitToLocation(moves[moves.Count - 1]);
        }

        actions.Add(action);
    }

    public void Attack(HexCell target, List<FightResult> results)
    {

        HexAction action = hexUnitActionController.CreateAction();
        action.ActionsUnit = this;
        List<HexUnit> combatUnits = Location.hexUnits.FindAll(c => c != this && c.unit.HexUnitType != Unit.UnitType.AGENT);
        List<HexUnit> enemyCombatUnits = target.hexUnits.FindAll(c => c != this && c.unit.HexUnitType != Unit.UnitType.AGENT);
        foreach (HexUnit unit in combatUnits)
        {
            action.AddExtraUnit(unit);
        }
        action.AddAction(target,Location, enemyCombatUnits, results);
        
        actions.Add(action);
    }

    public void SetLocationOnly(HexCell cell)
    {
        location = cell;
    }

    public void AddUnitToLocation(HexCell cell)
    {
        location.AddUnit(this);
    }

    public int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction, bool allowUnexplored = false)
	{
		if (!IsValidDestination(toCell, allowUnexplored)) {
            if(IsValidAttackDestination(toCell))
            {
                return 5;
            }
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

    public void UpdateUnit(int healthChange, bool killUnit)
    {
        unit.ShowHealthChange(healthChange);
        unit.UpdateUI(-healthChange);
        if (unit.HitPoints <= 0 && killUnit)
        {
            DieAnimationAndRemove();
        }
    }

    public AnimatorOverrideController AnimatorOverrideController
    {
        get
        {
            return animatorOverrideController;
        }

        set
        {
            animatorOverrideController = value;
        }
    }

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public void DoActions()
    {

        hexUnitActionController.AddActions(actions, this);
        actions.Clear();
    }

    public void AddAction(HexAction action)
    {
        if (actions.Count > 0)
        {
            if (actions[actions.Count - 1].Compare(action))
            {
                if (actions[actions.Count - 1].AddAction(action))
                {
                    Destroy(action.gameObject);
                    return;
                }
            }
        }
        actions.Add(action);

    }
    public bool IsValidDestination(HexCell cell, bool allowUnxplored = false)
    {
        return (cell.IsExplored || allowUnxplored) && !cell.IsUnderwater && cell.CanUnitMoveToCell(this);
    }

    public bool IsValidAttackDestination(HexCell cell)
    {
        HexUnit enemyUnit = cell.hexUnits.Find(c => c.unit.HexUnitType == unit.HexUnitType);
        if(enemyUnit)
        {
            if ((unit.GetCityOwner() && enemyUnit.unit.GetCityOwner() == unit.GetCityOwner()) || (unit.GetPlayer() && enemyUnit.unit.GetPlayer() == unit.GetPlayer()))
            {
                return false;
            }
            return true;
        }
        return false;


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