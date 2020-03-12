using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchableTable : MonoBehaviour {

    [SerializeField] TMP_InputField search;
    [SerializeField] List<SearchableTableObject> searchableTableObjects;
    [SerializeField] GameObject tableObjectPrefab;
    [SerializeField] List<AscDecButton> columnButtons;

    int selectedColumn = -1;

    public GameObject TableObjectPrefab
    {
        get
        {
            return tableObjectPrefab;
        }

        set
        {
            tableObjectPrefab = value;
        }
    }

    public List<SearchableTableObject> SearchableTableObjects
    {
        get
        {
            return searchableTableObjects;
        }

        set
        {
            searchableTableObjects = value;
        }
    }

    public void ShowDefault()
    {
        FillTable(0, true);
    }

    public virtual void SortBy(int column, bool ascending)
    {

    }

    public virtual void FillTable(int column = 0, bool ascending = true)
    {

    }

    public virtual void UpdateTableList()
    {

    }

    public void SelectColumn(int column)
    {
        if(selectedColumn != column )
        {
            if(selectedColumn != -1)
            {
                columnButtons[selectedColumn].Deselect();
            }
            selectedColumn = column;

        }
    }

    private void OnEnable()
    {
        UpdateTableList();
        SelectColumn(0);
        SortBy(0, true);
    }

    public void ClearObjects()
    {
        foreach (SearchableTableObject tableObject in searchableTableObjects)
        {
            Destroy(tableObject.gameObject);
        }
        searchableTableObjects.Clear();
    }
}
