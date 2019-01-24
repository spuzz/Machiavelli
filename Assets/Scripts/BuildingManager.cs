using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour {


    LinkedList<BuildInProgress> buildQueue = new LinkedList<BuildInProgress>();
    LinkedList<BuildInProgress> buildsReady = new LinkedList<BuildInProgress>();

    public void AddBuild(BuildConfig buildConfig)
    {
        buildQueue.AddLast(new BuildInProgress(buildConfig));
    }

    public void AddBuildPriority(BuildConfig buildConfig)
    {
        buildQueue.AddFirst(new BuildInProgress(buildConfig));
    }

    public int buildsInQueue()
    {
        return buildQueue.Count;
    }

    public void DayPassed()
    {
        if(buildQueue.Count > 0)
        {
            buildQueue.First.Value.DecreaseTimeLeft(1);
            if (buildQueue.First.Value.IsComplete())
            {
                buildsReady.AddLast(buildQueue.First.Value);
                buildQueue.RemoveFirst();
            }
        }
    }

    public BuildConfig GetCompletedBuild()
    {
        BuildConfig buildConfig = null;
        if (buildsReady.Count > 0)
        {
            buildConfig = buildsReady.First.Value.BuildConfig;
            buildsReady.RemoveFirst();
        }
        return buildConfig;
    }

    public void ClearQueue()
    {
        buildQueue.Clear();
    }
}
