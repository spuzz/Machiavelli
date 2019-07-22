﻿using NPBehave;
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
                                            new HasAbility("ability", "BuildOutpost"),
                                            new FindCellToBuildOutpost(""),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("", "BuildOutpost")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
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
                                                                                            //new Sequence(
                                                                                            //    new FindNearbyEnemyAgent("FindNearbyEnemyAgent"),
                                                                                            //    new Selector(
                                                                                            //        new Sequence(
                                                                                            //            new ReachedTarget("ReachedTarget"),
                                                                                            //            new AttackAgent(""),
                                                                                            //        ),
                                                                                            //        new MoveToTarget("")
                                                                                            //    )
                                                                                            //),
                                        new Sequence(
                                            new FindNearbyEnemyPlayerCity("FindNearbyEnemyPlayerCity"),
                                            new PickAbility("PickEnemyCityAbility", new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.EnemyPlayerCity}),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
                                        new Sequence(
                                            new FindNearbyEnemyCity("FindNearbyEnemyCity"),
                                            new PickAbility("PickEnemyCityAbility",new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.EnemyCity, AbilityConfig.AbilityType.City }),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
            
                                        new Sequence(
                                            new FindNearbyFriendlyCityTarget("FindNearbyFriendlyCity"),
                                            //new PickAbility("PickEnemyCityAbility", new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.FriendlyCity, AbilityConfig.AbilityType.City }),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
                                        new Sequence(
                                            new FindNearbyEnemyUnit("FindNearbyEnemyUnit"),
                                            new PickAbility("PickEnemyCityAbility", new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.EnemyUnit, AbilityConfig.AbilityType.EnemyAndFriendlyUnit } ),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("")
                                                ),
                                                new MoveToTarget("")
                                            )
                                        ),
                                        new Sequence(
                                            new FindNearbyFriendlyUnit("FindNearbyFriendlyUnit"),
                                            new PickAbility("PickEnemyCityAbility", new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.FriendlyUnit, AbilityConfig.AbilityType.EnemyAndFriendlyUnit }),
                                            new Selector(
                                                new Sequence(
                                                    new ReachedTarget("ReachedTarget"),
                                                    new UseAbility("")
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
        behaviourTree.Blackboard["ignoreExtraMovement"] = false;

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
        //if (agent.HexUnit.pathToTravel == null || agent.HexUnit.pathToTravel.Count == 0)
        //{
        //    return true;
        //}
        return true;
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
            if (!agent.Alive || agent.GetMovementLeft() <= 0 || (bool)Blackboard["ignoreExtraMovement"] == true)
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
            int range = (int)(Blackboard["range"]);
            if (!target)
            {
                Stopped(false);
                return;
            }
            if (agent.HexUnit.Location == target || agent.HexUnit.Location.coordinates.DistanceTo(target.coordinates) <= range)
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
                if(!agent.SetPath(path[1]))
                {
                    Blackboard["ignoreExtraMovement"] = true;
                }
                else
                {
                    Blackboard["ignoreExtraMovement"] = false;
                }
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

    public class Explore : Task
    {
        public Explore(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            List<HexCell> cells = PathFindingUtilities.FindNearestUnexplored(agent.HexUnit.Location, agent.HexUnit.Location, agent.GetPlayer().exploredCells);
            foreach(HexCell cell in cells)
            {
                if(cell.CanUnitMoveToCell(agent.HexUnit))
                {
                    HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
                    grid.FindPath(agent.HexUnit.Location, cell, agent.HexUnit, true, false);
                    List<HexCell> path = grid.GetPath();
                    if (path != null && path.Count > 1)
                    {
                        if (agent.SetPath(path[1]))
                        {
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

        public UseAbility(string name) : base(name)
        {
            
        }
        protected override void DoStart()
        {
            string abilityToUse = abilityName;
            if(string.IsNullOrEmpty(abilityToUse))
            {
                abilityToUse = (Blackboard["ability"] as string);
            }
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell targetCell = (Blackboard["targetCell"] as HexCell);
            if(agent.RunAbility(abilityToUse, targetCell))
            {
                Stopped(true);
            }
            else
            {
                Stopped(false);
            }
        }
        protected override void DoStop()
        {
            Stopped(true);
        }
    }

    public class FindCellToBuildOutpost : Task
    {
        public FindCellToBuildOutpost(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            AbilityConfig config = agent.GetAbility("BuildOutpost");
            List<City> cities = PathFindingUtilities.FindAllSeenCities(agent.GetPlayer().exploredCells);
            cities = cities.FindAll(c => !c.PlayerBuildingControl.HasOutpost(agent.GetPlayer()) && agent.GetPlayer().IsCityStateFriendly(c.GetCityState()));
            if (cities.Count > 0)
            {
                cities = cities.OrderBy(c => c.GetHexCell().coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                foreach (City cityToBuildOutpost in cities)
                {

                    if (config.IsValidTarget(cityToBuildOutpost.GetHexCell()))
                    {
                        Blackboard["targetCell"] = cityToBuildOutpost.GetHexCell();
                        Blackboard["range"] = config.Range;
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
                        if (config.IsValidTarget(targetCell))
                        {
                            Blackboard["targetCell"] = targetCell;
                            Blackboard["range"] = config.Range;
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

    public class FindCellToAssassinate : Task
    {
        public FindCellToAssassinate(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            AbilityConfig config = agent.GetAbility("AssassinateAgent");
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, agent.HexUnit.VisionRange);
            List<Agent> targets = new List<Agent>();
            foreach (HexCell cell in cells)
            {
                Agent possibleTarget = cell.GetAgent(agent.HexUnit);
                if (possibleTarget && possibleTarget.GetPlayer() != agent.GetPlayer())
                {
                    targets.Add(possibleTarget);
                }

            }
            
            if(targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.HexUnit.Location.coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0].HexUnit.Location;
                Blackboard["range"] = config.Range;
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

    public class FindNearbyEnemyPlayerCity : Task
    {
        public FindNearbyEnemyPlayerCity(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, 5);
            List<City> targets = new List<City>();
            foreach (HexCell cell in cells)
            {
                if(cell.City && cell.City.Player && cell.City.Player != agent.GetPlayer() && agent.GetPlayer().exploredCells.Contains(cell))
                {
                    targets.Add(cell.City);
                }
            }

            if (targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.GetHexCell().coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0].GetHexCell();
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

    public class FindNearbyEnemyCity : Task
    {
        public FindNearbyEnemyCity(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, 5);
            List<City> targets = new List<City>();
            foreach (HexCell cell in cells)
            {
                if (cell.City && !agent.GetPlayer().IsCityStateFriendly(cell.City.GetCityState()) && agent.GetPlayer().exploredCells.Contains(cell))
                {
                    targets.Add(cell.City);
                }
            }

            if (targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.GetHexCell().coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0].GetHexCell();
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

    public class FindNearbyFriendlyCityTarget : Task
    {
        public FindNearbyFriendlyCityTarget(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, 5);
            List<City> targets = agent.GetPlayer().GetFriendlyCitiesOrderByDistance(agent.HexUnit.Location.coordinates);
            foreach(City city in targets)
            {
                AbilityConfig config = FindAbility(new List<AbilityConfig.AbilityType>() { AbilityConfig.AbilityType.FriendlyCity }, city.GetHexCell(), agent);
                if(config)
                {
                    Blackboard["ability"] = config.AbilityName;
                    Blackboard["range"] = config.Range;
                    Blackboard["targetCell"] = city.GetHexCell();
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
    public class FindNearbyEnemyUnit : Task
    {
        public FindNearbyEnemyUnit(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, agent.HexUnit.VisionRange);
            List<HexCell> targets = new List<HexCell>();
            foreach (HexCell cell in cells)
            {
                if (cell.hexUnits.Count > 0 && agent.GetPlayer().exploredCells.Contains(cell))
                {
                    foreach (HexUnit unit in cell.hexUnits)
                    {
                        if (unit.HexUnitType == HexUnit.UnitType.COMBAT && unit.unit.GetCityOwner() && !agent.GetPlayer().IsCityStateFriendly(unit.unit.GetCityOwner().GetCityState()))
                        {
                            targets.Add(cell);
                        }
                    }
                    ;
                }
            }

            if (targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0];
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

    public class FindNearbyEnemyAgent : Task
    {
        public FindNearbyEnemyAgent(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, agent.HexUnit.VisionRange);
            List<HexCell> targets = new List<HexCell>();
            foreach (HexCell cell in cells)
            {
                if (cell.hexUnits.Count > 0 && agent.GetPlayer().exploredCells.Contains(cell))
                {
                    foreach (HexUnit unit in cell.hexUnits)
                    {
                        if (unit.HexUnitType == HexUnit.UnitType.AGENT && unit.unit.GetPlayer() && unit.unit.GetPlayer() != agent.GetPlayer())
                        {
                            targets.Add(cell);
                        }
                    }
                    ;
                }
            }

            if (targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0];
                Blackboard["range"] = 1;
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


    public class FindNearbyFriendlyUnit : Task
    {
        public FindNearbyFriendlyUnit(string name) : base(name)
        {

        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexGrid grid = (Blackboard["hexgrid"] as HexGrid);
            List<HexCell> cells = grid.GetVisibleCells(agent.HexUnit.Location, agent.HexUnit.VisionRange);
            List<HexCell> targets = new List<HexCell>();
            foreach (HexCell cell in cells)
            {
                if (cell.hexUnits.Count > 0 && agent.GetPlayer().visibleCells.Keys.Contains(cell) && agent.GetPlayer().visibleCells[cell] > 0)
                {
                    foreach (HexUnit unit in cell.hexUnits)
                    {
                        if (unit.HexUnitType == HexUnit.UnitType.COMBAT && unit.unit.GetCityOwner() && agent.GetPlayer().IsCityStateFriendly(unit.unit.GetCityOwner().GetCityState()))
                        {
                            targets.Add(cell);
                        }
                    }
                    ;
                }
            }

            if (targets.Count > 0)
            {
                targets = targets.OrderBy(c => c.coordinates.DistanceTo(agent.HexUnit.Location.coordinates)).ToList();
                Blackboard["targetCell"] = targets[0];
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
    

    public class PickAbility : Task
    {
        List<AbilityConfig.AbilityType> abilityTypes;
        public PickAbility(string name, AbilityConfig.AbilityType ability) : base(name)
        {
            abilityTypes = new List<AbilityConfig.AbilityType>();
            abilityTypes.Add(ability);
        }
        public PickAbility(string name, List<AbilityConfig.AbilityType> abilities) : base(name)
        {
            abilityTypes = new List<AbilityConfig.AbilityType>();
            foreach(AbilityConfig.AbilityType ability in abilities)
            {
                abilityTypes.Add(ability);
            }
            
        }

        protected override void DoStart()
        {
            Agent agent = (Blackboard["agent"] as Agent);
            HexCell targetCell = (Blackboard["targetCell"] as HexCell);
            AbilityConfig config = FindAbility(abilityTypes, targetCell, agent);
            if(config)
            {
                Blackboard["ability"] = config.AbilityName;
                Blackboard["range"] = config.Range;
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

    public static AbilityConfig FindAbility(List<AbilityConfig.AbilityType> abilityTypes, HexCell targetCell, Agent agent)
    {
        List<AbilityConfig> configs = agent.GetAbilities(abilityTypes);
        IListExtensions.Shuffle(configs);
        foreach (AbilityConfig config in configs)
        {
            if (config.IsValidTarget(targetCell))
            {
                if (config.IsGoodTarget(targetCell))
                {
                    return config;
                }

            }
        }
        return null;
    }
}
