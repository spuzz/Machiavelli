using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AscDecButton : MonoBehaviour {

    [SerializeField] SearchableTable table;
    [SerializeField] int column;
    [SerializeField] GameObject selectedPanel;

    [SerializeField] bool defaultSetting = false;

    bool ascending;
    public bool selected = false;

    private void Awake()
    {
        ascending = defaultSetting;
    }

    public void Select()
    {
        if(selected == false)
        {
            selected = true;
            ascending = defaultSetting;
            selectedPanel.SetActive(true);
        }
        else
        {
            ascending = !ascending;
        }
        table.SelectColumn(column);
        table.SortBy(column, ascending);
    }

    public void Deselect()
    {
        selected = false;
        ascending = defaultSetting;
        selectedPanel.SetActive(false);
    }
}
