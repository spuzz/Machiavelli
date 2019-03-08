using NPBehave;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentBehaviourTree : MonoBehaviour {

    Root behaviourTree;

    [SerializeField] Agent agent;
    HexGrid hexGrid;
    public Root BehaviourTree
    {
        get
        {
            return behaviourTree;
        }

        set
        {
            behaviourTree = value;
        }
    }

    private void Awake()
    {
        behaviourTree = new Root(
            new WaitForCondition(CheckStatus,
                new Selector(
                    new Repeater(
                        new WaitForCondition(CheckBusy,
                            new Sequence(
                                new HasMovementLeft("Test"),
                                    new Selector(
                                        new Sequence(
                                            new HasAbility("ability", "BuildOperationCentre"),
                                            new FindCellToBuildOperationCentre(""),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("", "BuildOperationCentre")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
                                        new Explore("Explore")
                                    )
                            )
                        )
                    ),
                    new Action(() => FinishTurn())
                )
           )
        );
        behaviourTree.Blackboard["agent"] = agent;
        hexGrid = FindObjectOfType<HexGrid>();
        behaviourTree.Blackboard["hexgrid"] = hexGrid;
        behaviourTree.Blackboard["TakeTurn"] = false;

    }

    public void TakeTurn()
    {
        behaviourTree.Blackboard["TakeTurn"] = true;

    }
    public void FinishTurn()
    {
        behaviourTree.Blackboard["TakeTurn"] = false;

    }

    public bool IsFinished()
    {
        return !behaviourTree.Blackboard.Get<bool>("TakeTurn");
    }
    public bool CheckStatus()
    {

        if(behaviourTree.Blackboard.Get<bool>("TakeTurn"))
        {
            return true;
        }
        return false;
    }
    public bool CheckBusy()
    {
        if (agent.HexUnit.pathToTravel == null || agent.HexUnit.pathToTravel.Count == 0)
        {
            return true;
        }
        return false;
    }
    public void OnDestroy()
    {
        StopBehaviorTree();
    }

    public void StartTree()
    {
        if(!behaviourTree.IsActive)
        {
            behaviourTree.Start();
        }
        
    }
    public void StopBehaviorTree()
    {
        if (behaviourTree != null && behaviourTree.CurrentState == Node.State.ACTIVE)
        {
            behaviourTree.Stop();
        }
    }
    public class HasMovementLeft : Task
    {
        public HasMovementLeft(string name) : base(name)
        {

        }
        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            if (agent.GetMovementLeft() <= 0)
            {
                Stopped(false);
                return;
            }

            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class IsBusy : Task
    {
        public IsBusy(string name) : base(name)
        {
        }
        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            if(agent.HexUnit.pathToTravel == null || agent.HexUnit.pathToTravel.Count == 0)
            {
                Stopped(true);
            }
            Stopped(false);
        }
        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class ReachedTarget : Task
    {
        public ReachedTarget(string name) : base(name)
        {
        }
        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell target = (Blackboard["targetCell"] as HexCell);
            if(!target)
            {
                Stopped(false);
                return;
            }
            if (agent.HexUnit.Location == target)
            {
                Stopped(true);
                return;
            }
            Stopped(false);
        }
        protected override void DoStop()
        {
            Stopped(true);
        }
    }


    public class MoveToTarget : Task
    {
        public MoveToTarget(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell targetCell = (Blackboard["targetCell"] as HexCell);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            grid.FindPath(agent.HexUnit.Location, targetCell, agent.HexUnit, true, false);
            List<HexCell> path = grid.GetPath();
            if (path != null && path.Count > 1)
            {
                if (agent.SetPath(path))
                {
                    Stopped(true);
                    return;
                }
            }
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class Explore : Task
    {
        public Explore(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell cell = PathFindingUtilities.FindNearestUnexplored(agent);
            if(cell)
            {
                HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
                grid.FindPath(agent.HexUnit.Location,cell,agent.HexUnit,true,false);
                List<HexCell> path = grid.GetPath();
                if (path != null && path.Count > 1)
                {
                    if(agent.SetPath(path))
                    {
                        Stopped(true);
                        return;
                    }
                    
                }
              
            }
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class HasAbility : Task
    {
        string abilityName;
        public HasAbility(string name, string ability) : base(name)
        {
            abilityName = ability;
        }
        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            if (agent.HasAbility(abilityName))
            {
                Stopped(true);
                return;
            }
            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class UseAbility : Task
    {
        string abilityName;
        public UseAbility(string name, string ability) : base(name)
        {
            abilityName = ability;
        }
        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell targetCell = (Blackboard["targetCell"] as HexCell);
            agent.UseAbility(abilityName, targetCell);
            Stopped(true);
        }
        protected override void DoStop()
        {
            Stopped(true);
        }
    }


    public class FindCellToBuildOperationCentre : Task
    {
        public FindCellToBuildOperationCentre(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            AbilityConfig config = agent.GetAbility("BuildOperationCentre");
            List<City> cities = PathFindingUtilities.FindAllSeenCities(agent.GetPlayer().exploredCells);
            cities = cities.FindAll(c => !c.PlayerBuildingControl.HasOutpost(agent.GetPlayer()));
            if(cities.Count > 0)
            {
                cities = cities.OrderBy(c => c.GetHexCell().coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                foreach(City cityToBuildOpCentre in cities)
                {
                    List<HexCell> cells = PathFindingUtilities.GetCellsInRange(cityToBuildOpCentre.GetHexCell(), 2);
                    cells = cells.OrderBy(c => c.coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                    foreach(HexCell targetCell in cells)
                    {
                        if (config.IsValidTarget(targetCell).Count > 0)
                        {
                            Blackboard["targetCell"] = targetCell;
                            Stopped(true);
                            return;
                        }
                    }

                }
                
            }

            Stopped(false);
        }

        protected override void DoStop()
        {
            Stopped(true);
        }
    }
}
