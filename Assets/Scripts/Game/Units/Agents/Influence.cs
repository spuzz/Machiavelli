using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Units/Agents/Influence"))]
public class Influence : MonoBehaviour {

    [SerializeField] Agent agent;
    [SerializeField] HexCell currentCell;
    [SerializeField] int range = -1;
    [SerializeField] GameEffect gameEffects;

    private void Start()
    {
        agent.GetComponent<EffectsController>().onInfoChange += UpdateEffects;
    }
    public void addEffect(GameEffect effect)
    {
        gameEffects.AddEffect(effect);
        UpdateEffects();
    }

    public void RemoveEffects(GameEffect effect)
    {
        gameEffects.AddEffect(effect);
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
                cell.City.CityResouceController.EffectsController.RemoveEffect(agent.gameObject, gameEffects.EffectName);
            }
        }
    }

    private void ApplyEffects()
    {
        foreach (HexCell cell in PathFindingUtilities.GetCellsInRange(agent.HexUnit.Location, agent.GetComponent<EffectsController>().TotalEffects.InfluenceRange))
        {
            if (cell.City)
            {
                cell.City.CityResouceController.EffectsController.AddEffect(agent.gameObject, gameEffects);
            }
        }
        currentCell = agent.HexUnit.Location;
        range = agent.GetComponent<EffectsController>().TotalEffects.InfluenceRange;
    }
}
