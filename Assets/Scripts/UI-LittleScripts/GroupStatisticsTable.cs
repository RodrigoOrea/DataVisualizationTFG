using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GroupStatisticsTable : SingletonMonoBehavior<GroupStatisticsTable>
{
    public GameObject rowParent;
    public GameObject rowPrefab;

    public void Start()
    {
        PopulateTable();
    }

    private void PopulateTable()
    {
        throw new NotImplementedException();
    }
}
