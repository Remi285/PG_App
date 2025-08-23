using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Generator generator = (Generator)target;
        if (GUILayout.Button("Generate perlin noise"))
        {
            generator.GeneratePerlin();
        }
        if (GUILayout.Button("Generate diamond square"))
        {
            generator.GenerateDiamonSquare();
        }
        if (GUILayout.Button("Generate Worley"))
        {
            generator.GenerateWorley();
        }
        if (GUILayout.Button("Generate from image"))
        {
            generator.GenerateFromImage();
        }
        if (GUILayout.Button("Generate NN"))
        {
            generator.GenerateNN();
        }
    }
}
