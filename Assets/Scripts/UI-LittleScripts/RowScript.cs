using TMPro;
using UnityEngine;

public class RowScript : MonoBehaviour
{

    public TMP_Text attributeName;

    public TMP_Text average;

    public TMP_Text min;
    public TMP_Text max;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeRow(string attributeName, float average, float min, float max)
    {
        this.attributeName.text = attributeName;
        this.average.text = average.ToString();
        this.min.text = min.ToString();
        this.max.text = max.ToString();
    }
}
