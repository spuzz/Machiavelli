using Assets.Scripts.AI.General;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BuildingManager{


    LinkedList<BuildInProgress> buildQueue = new LinkedList<BuildInProgress>();
    LinkedList<BuildInProgress> buildsReady = new LinkedList<BuildInProgress>();

    int lastID;
    public void AddBuild(BuildConfig buildConfig, int id = -1)
    {
        buildQueue.AddLast(new BuildInProgress(buildConfig, id));
    }

    public void AddBuildPriority(BuildConfig buildConfig, int id = -1)
    {
        buildQueue.AddFirst(new BuildInProgress(buildConfig, id));
    }

    public BuildConfig RemoveFromQueue(int queueNumber)
    {
        if (BuildsInQueue() > queueNumber)
        {
            return ExtensionMethods.RemoveAt(buildQueue, queueNumber).Value.BuildConfig;
        }
        return null;
            
    }

    public int BuildsInQueue()
    {
        return buildQueue.Count;
    }

    public int TimeLeftOnBuild(int production)
    {
        if(buildQueue.Count > 0)
        {
            int days = 9999;
            if (production != 0)
            {
                days = (buildQueue.First.Value.TimeLeft() + production - 1) / production;
            }
            return  days;
        }
        return -1;
    }
    public void DayPassed(int production)
    {
        if(buildQueue.Count > 0)
        {
            buildQueue.First.Value.DecreaseProductionLeft(production);
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
            lastID = buildsReady.First.Value.BuildID;
            buildsReady.RemoveFirst();
        }
        return buildConfig;
    }

    public int GetIDofLastCompletedBuild()
    {
        return lastID;
    }

    public BuildConfig currentBuilding()
    {
        if (buildQueue.Count > 0)
        {
            return buildQueue.First.Value.BuildConfig;
        }
        return null;
    }

    public BuildConfig GetConfigInQueue(int numberInQueue)
    {
        if(numberInQueue < BuildsInQueue())
        {
            return buildQueue.ElementAt(numberInQueue).BuildConfig;
        }
        return null;
    }

    public BuildConfig GetConfigInQueueByID(int id)
    {

        foreach (BuildInProgress build in buildQueue)
        {
            if (build.BuildID == id)
            {
                return build.BuildConfig;
            }
        }
        return null;
    }


    public int IDInConstruction()
    {
        if (buildQueue.Count > 0)
        {
            return buildQueue.First.Value.BuildID;
        }
        return -1;
    }
    public bool IsIDInQueue(int id)
    {
        foreach(BuildInProgress build in buildQueue)
        {
            if(build.BuildID == id)
            {
                return true;
            }
        }
        return false;
    }
    public void ClearQueue()
    {
        buildQueue.Clear();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(buildQueue.Count);
        foreach(BuildInProgress buildInProgress in buildQueue)
        {
            buildInProgress.Save(writer);
        }
        writer.Write(buildsReady.Count);
        foreach (BuildInProgress buildInProgress in buildsReady)
        {
            buildInProgress.Save(writer);
        }
    }
    public void Load(BinaryReader reader, GameController gameController, int header)
    {
        if(header >= 5)
        {
            int count = reader.ReadInt32();
            for(int a=0; a<count; a++)
            {
                buildQueue.AddLast(BuildInProgress.Load(reader,gameController));
            }
            count = reader.ReadInt32();
            for (int a = 0; a < count; a++)
            {
                buildsReady.AddLast(BuildInProgress.Load(reader, gameController));
            }
        }
    }
}
