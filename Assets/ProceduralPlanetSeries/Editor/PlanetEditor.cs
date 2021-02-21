using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);
    }

    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings == null) return;
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

        // Check if anything has changed
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (foldout)
            {
                // Create editor only if none exists
                CreateCachedEditor(settings, null, ref editor);
                // Draw editor
                editor.OnInspectorGUI();

                // Call update methods only if things have changed
                if (check.changed)
                {
                    onSettingsUpdated?.Invoke();
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
