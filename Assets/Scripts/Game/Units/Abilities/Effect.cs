using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect  {

    Player player;
    protected string name;
    public string Name
    {
        get
        {
            return name;
        }

    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }


    public abstract void UseEffect(City city);

    public abstract bool Compare(Effect effect);
}
