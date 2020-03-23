using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influence : MonoBehaviour {

    [SerializeField] Agent agent;
    [SerializeField] HexCell currentCell;
    [SerializeField] int range = -1;
    [SerializeField] GameEffect friendlyInfluenceEffects;
    [SerializeField] GameEffect enemyInfluenceEffects;

    private void Start()
    {
        agent.GetComponent<EffectsController>().onInfoChange += UpdateEffects;
    }
    public void addEffect(GameEffect effect, bool friendly)
    {
        if(friendly)
        {
            friendlyInfluenceEffects.AddEffect(effect);
        }
        else
        {
            enemyInfluenceEffects.AddEffect(effect);
        }

        UpdateEffects();
    }

    public void RemoveEffect(GameEffect effect, bool friendly)
    {
        if (friendly)
        {
            friendlyInfluenceEffects.RemoveEffect(effect);
        }
        else
        {
            enemyInfluenceEffects.RemoveEffect(effect);
        }
        UpdateEffects();
    }

    public void UpdateEffects()
    {
        if(currentCell && currentCell != agent.HexUnit.Location)
        {
            RemoveEffects();
        }
        if(range != -1 && range != agent.InfluenceRange)
        {
            RemoveEffects();
        }

        ApplyEffects();

    }

    private void RemoveEffects()
    {
        foreach (HexCell cell in PathFindingUtilities.GetCellsInRange(currentCell, range))
        {
            if(cell.City)
            {
                if(cell.City.GetCityState().Player && cell.City.GetCityState().Player == agent.GetPlayer())
                {
                    cell.City.CityResouceController.EffectsController.RemoveEffect(agent.gameObject, friendlyInfluenceEffects.EffectName);
                }
                else
                {
                    cell.City.CityResouceController.EffectsController.RemoveEffect(agent.gameObject, enemyInfluenceEffects.EffectName);
                }
                
            }
            if (cell.agent)
            {
                if (cell.agent.unit.GetPlayer() == agent.GetPlayer())
                {
                    cell.agent.unit.GetComponent<EffectsController>().RemoveEffect(agent.gameObject, friendlyInfluenceEffects.EffectName);
                }
                else
                {
                    cell.agent.unit.GetComponent<EffectsController>().RemoveEffect(agent.gameObject, enemyInfluenceEffects.EffectName);
                }
            }
            if (cell.combatUnit)
            {
                if (cell.combatUnit.unit.GetPlayer() == agent.GetPlayer())
                {
                    cell.combatUnit.unit.GetComponent<EffectsController>().RemoveEffect(agent.gameObject, friendlyInfluenceEffects.EffectName);
                }
                else
                {
                    cell.combatUnit.unit.GetComponent<EffectsController>().RemoveEffect(agent.gameObject, enemyInfluenceEffects.EffectName);
                }
            }
        }
    }

    private void ApplyEffects()
    {
        foreach (HexCell cell in PathFindingUtilities.GetCellsInRange(agent.HexUnit.Location, agent.GetComponent<EffectsController>().TotalEffects.InfluenceRange))
        {
            if (cell.City)
            {
                if (cell.City.GetCityState().Player && cell.City.GetCityState().Player == agent.GetPlayer())
                {
                    cell.City.CityResouceController.EffectsController.AddEffect(agent.gameObject, friendlyInfluenceEffects);
                }
                else
                {
                    cell.City.CityResouceController.EffectsController.AddEffect(agent.gameObject, enemyInfluenceEffects);
                }
            }
            if(cell.agent)
            {
                if(cell.agent.unit.GetPlayer() == agent.GetPlayer())
                {
                    cell.agent.unit.GetComponent<EffectsController>().AddEffect(agent.gameObject, friendlyInfluenceEffects);
                }
                else
                {
                    cell.agent.unit.GetComponent<EffectsController>().AddEffect(agent.gameObject, enemyInfluenceEffects);
                }
            }
            if (cell.combatUnit)
            {
                if (cell.combatUnit.unit.GetPlayer() == agent.GetPlayer())
                {
                    cell.combatUnit.unit.GetComponent<EffectsController>().AddEffect(agent.gameObject, friendlyInfluenceEffects);
                }
                else
                {
                    cell.combatUnit.unit.GetComponent<EffectsController>().AddEffect(agent.gameObject, enemyInfluenceEffects);
                }
            }

        }
        currentCell = agent.HexUnit.Location;
        range = agent.GetComponent<EffectsController>().TotalEffects.InfluenceRange;
    }
}
