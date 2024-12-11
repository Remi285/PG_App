using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Generator generator = (Generator)target;
        if(GUILayout.Button("Generate perlin noise"))
        {
            generator.GeneratePerlin();
        }
        if(GUILayout.Button("Generate diamond square"))
        {
            generator.GenerateDiamonSquare();
        }
        if(GUILayout.Button("Generate 1D"))
        {
            generator.Generate1D();
        }
        if(GUILayout.Button("Generate Voronoi"))
        {
            generator.GenerateVoronoi();
        }
        if(GUILayout.Button("Clear visualization"))
        {
            generator.ClearVisualization();
        }
    }
}
