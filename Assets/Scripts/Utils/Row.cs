using UnityEngine;
using TMPro;

public class Row
{
    public string attributeName;

    public float average;

    public float min;
    public float max;

    public GameObject setRow (GameObject parent, GameObject rowPrefab)
    {
        rowPrefab.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = attributeName; // Primer hijo
        rowPrefab.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = average.ToString(); // Segundo hijo
        rowPrefab.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = min.ToString();
        rowPrefab.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text = max.ToString();
        return rowPrefab;
    }

    public Row(string attributeName, float average, float min, float max, GameObject rowPrefab, GameObject parent)
    {
        this.attributeName = attributeName;
        this.average = average;
        this.min = min;
        this.max = max;
        setRow(parent, rowPrefab);
    }
}

    