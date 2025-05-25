using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeAttributes))]
public class TreeAttributesEditor : Editor
{
    // Esto se llama cuando se dibuja el Inspector de "TreeAttributes"
    public override void OnInspectorGUI()
    {
        // Obtiene la referencia al objeto que estamos editando
        TreeAttributes treeAttributes = (TreeAttributes)target;

        // Puedes dibujar el Inspector por defecto si quieres que aparezcan
        // los campos automáticos
        // base.OnInspectorGUI();

        // O dibujar manualmente tu tabla
        DrawTable(treeAttributes);
    }

    private void DrawTable(TreeAttributes tree)
    {
        // Inicio de sección en el Inspector
        EditorGUILayout.LabelField("Tabla de Atributos", EditorStyles.boldLabel);

        // Encabezados de la tabla
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Atributo", EditorStyles.boldLabel, GUILayout.Width(100));
        EditorGUILayout.LabelField("Valor", EditorStyles.boldLabel, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Fila 1 - Nombre
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Nombre", GUILayout.Width(100));
        EditorGUILayout.LabelField(tree.treeName, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Fila 2 - Altura
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Altura", GUILayout.Width(100));
        EditorGUILayout.LabelField(tree.height.ToString(), GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Fila 3 - Ancho
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ancho", GUILayout.Width(100));
        EditorGUILayout.LabelField(tree.width.ToString(), GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Fila 4 - Edad
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Edad", GUILayout.Width(100));
        EditorGUILayout.LabelField(tree.age.ToString(), GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Agrega más filas o columnas según lo necesites
    }
}
