using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeAttributes : MonoBehaviour

{
    public List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();


    public GameObject prefabInstance;
    public string treeName;
    public float height;
    public float width;
    public int age;

    Color[] originalColors;
    
    public void ApplyColor(string atributo)
{
    (float min, float max, float average) Stats = MapLoader.Instance.CalculateStats(atributo);
    float valor = GetValue(atributo);
    Renderer renderer = prefabInstance.GetComponent<Renderer>();
    Material[] mats = renderer.materials;

    if (renderer == null)
    {
        Debug.LogWarning("El objeto no tiene un Renderer asignado.");
        return;
    }

    // Clone materials to prevent shared modifications
    for (int i = 0; i < mats.Length; i++)
    {
        mats[i] = new Material(mats[i]); 
    }

    // Define color range (Soft Red to Bright Red)
    Color softRed = new Color(1f, 0.8f, 0.8f); // Light pink
    Color brightRed = new Color(1f, 0f, 0f);   // Bright red

    // Out-of-range values → White
    if (valor == 0 || valor < Stats.min || valor > Stats.max)
    {
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].color = Color.white;
        }
        renderer.materials = mats;
        return;
    }

    // 🔥 **Improved Contrast Mapping**
    float t = (valor - Stats.min) / (Stats.max - Stats.min); // Normalize between 0-1

    // Adjust contrast with a **smoother exponential scale**
    float contrastFactor = 1.5f; // Adjust between 1.2 - 2.0 for different contrasts
    t = Mathf.Pow(t, contrastFactor) / Mathf.Pow(0.5f, contrastFactor - 1f); // Scaled mapping

    // Apply the new color gradient
    for (int i = 0; i < mats.Length; i++)
    {
        mats[i].color = Color.Lerp(softRed, brightRed, t);
    }

    renderer.materials = mats;
}





    public void EraseColor()
    {
        Renderer renderer = prefabInstance.GetComponent<Renderer>();
        Material[] mats = renderer.materials;
        for (int i = 0; i < mats.Length; i++){
                    mats[i].color = Color.white;
                }
    }



    
    // Método para asignar atributos dinámicamente

    public void SetPrefab(GameObject prefab){
        this.prefabInstance = prefab;
    }
    public void SetAttributes(Dictionary<string, string> attributeData)
    {
        attributes.Clear();
        foreach (var entry in attributeData)
        {
            attributes.Add(new KeyValuePair<string, string>(entry.Key, entry.Value));
        }
    }

    public float GetValue(string attributeName)
{
    // Busca en la lista de KeyValuePair
    foreach (var kv in attributes)
    {
        if (kv.Key == attributeName)
        {
            float val;
            if (float.TryParse(kv.Value, out val))
            {
                return val;
            }
        }
    }
    // Si no lo encuentra o no se puede parsear, regresa 0 u otro valor por defecto
    return 0f;
    }
}

