using System;
using UnityEngine;
using UnityEditor;

namespace Settings.Generator
{
    [CustomEditor(typeof(BiomesSettings))]
    public class BiomesSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("biomesTypes"), true);
            
            var settings = (BiomesSettings)target;
            if (settings.biomesTypes.Length > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moistureLevels"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heightLevels"));
                EditorGUILayout.Space();
                UpdateBiomesArraySize();
                DrawTable(settings.moistureLevels, settings.heightLevels, DrawCell);
            }
            else EditorGUILayout.HelpBox("Add biomes to Biomes Names array", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateBiomesArraySize()
        {
            var settings = (BiomesSettings)target;

            int width = settings.moistureLevels;
            int height = settings.heightLevels;

            if (settings.selectedBiomes.width != width ||
                settings.selectedBiomes.height != height)
            {
                int[,] oldSelectedBiomes = settings.selectedBiomes;
                settings.selectedBiomes = new BiomesSettings.SelectedBiomesArray(width, height);
                for (int x = 0; x < oldSelectedBiomes.GetLength(0); x++)
                {
                    for (int y = 0; y < oldSelectedBiomes.GetLength(1); y++)
                    {
                        if (x < width && y < height)
                            settings.selectedBiomes[x, y] = oldSelectedBiomes[x, y];
                    }
                }
            }
        }

        private void DrawTable(int width, int height, Action<int, int> drawCellAction)
        {
            for (int y = 0; y < height + 1; y++)
            {
                GUILayout.BeginHorizontal();
                if (y > 0)
                    GUILayout.Label(y.ToString() + (y >= 10 ? "" : "  "));
                
                for (int x = 0; x < width + 1; x++)
                {
                    if (y == 0)
                        GUILayout.Label(x.ToString());
                    else if (x > 0)
                        drawCellAction(x - 1, y - 1);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawCell(int x, int y)
        {
            var settings = (BiomesSettings)target;
            settings.selectedBiomes[x, y] = EditorGUILayout.Popup(settings.selectedBiomes[x, y], settings.biomesTypes, GUILayout.MinWidth(4));
        }
    }
}
