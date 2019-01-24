using System.Collections;
using System.Collections.Generic;


public class BuildInProgress {

    int timeLeft;
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
            timeLeft = buildConfig.BaseBuildTime;
        }
    }

    public void DecreaseTimeLeft(int days)
    {
        timeLeft -= days;
    }

    public bool IsComplete()
    {
        if(timeLeft <= 0)
        {
            return true;
        }
        return false;
    }
    
}
