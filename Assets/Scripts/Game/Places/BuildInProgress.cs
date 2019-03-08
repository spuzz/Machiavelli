using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BuildInProgress {

    int productionLeft;
    BuildConfig buildConfig;
    int buildID;
    public BuildInProgress(BuildConfig buildConfig, int id)
    {
        BuildConfig = buildConfig;
        BuildID = id;
    }

    public BuildConfig BuildConfig
    {
        get
        {
            return buildConfig;
        }

        set
        {
            buildConfig = value;
            productionLeft = buildConfig.BaseBuildTime;
        }
    }

    public int BuildID
    {
        get
        {
            return buildID;
        }

        set
        {
            buildID = value;
        }
    }

    public void DecreaseProductionLeft(int days)
    {
        productionLeft -= days;
    }

    public int TimeLeft()
    {
        return productionLeft;
    }
    public bool IsComplete()
    {
        if(productionLeft <= 0)
        {
            return true;
        }
        return false;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(buildConfig.Name);
        writer.Write(productionLeft);
        writer.Write(buildID);
    }

    public static BuildInProgress Load(BinaryReader reader, GameController gameController)
    {
        string buildConfigName = reader.ReadString();
        int productLeft = reader.ReadInt32();
        int buildID = reader.ReadInt32();

        BuildConfig buildConfig = gameController.GetBuildConfig(buildConfigName);
        if(!buildConfig)
        {
            return null;
        }
        BuildInProgress buildInProgress = new BuildInProgress(buildConfig,buildID);
        buildInProgress.productionLeft = productLeft;
        return buildInProgress;

    }
    
}
