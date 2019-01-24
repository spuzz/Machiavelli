using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HexCellNode
{
    HexCell hexCell;
    HexCellNode hexCellParent;

    int cost = 0;
    int hCost = 0;
    int fCost = 0;

    public HexCell HexCell
    {
        get
        {
            return hexCell;
        }

        set
        {
            hexCell = value;
        }
    }

    public HexCellNode HexCellNodeParent
    {
        get
        {
            return hexCellParent;
        }

        set
        {
            hexCellParent = value;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
            fCost = cost + hCost;
        }
    }

    public int HCost
    {
        get
        {
            return hCost;
        }

        set
        {
            hCost = value;
            fCost = cost + hCost;
        }
    }

    public int FCost
    {
        get
        {
            return fCost;
        }

    }
}
