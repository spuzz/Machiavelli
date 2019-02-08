using System.Collections;
using System.Collections.Generic;


public class BuildInProgress {

    int productionLeft;
    BuildConfig buildConfig;

    public BuildInProgress(BuildConfig buildConfig)
    {
        BuildConfig = buildConfig;
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

    public void DecreaseProductionLeft(int days)
    {
        productionLeft -= days;
    }

    public bool IsComplete()
    {
        if(productionLeft <= 0)
        {
            return true;
        }
        return false;
    }
    
}
