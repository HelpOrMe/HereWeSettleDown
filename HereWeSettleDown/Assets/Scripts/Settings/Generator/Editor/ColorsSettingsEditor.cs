using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Settings.Generator
{
    [CustomEditor(typeof(ColorsSettings))]
    public class ColorsSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var settings = (ColorsSettings)target;

            if (settings.biomesSettings == null)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("biomesSettings"), true);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            UpdateBiomesList();
            DrawTableHeader();
            DrawTable();
            DrawButtons();
        }

        private void UpdateBiomesList()
        {
            var settings = (ColorsSettings)target;
            if (settings.biomesSettings != null)
            {
                foreach (string biomeType in settings.biomesSettings.biomesTypes)
                {
                    if (!settings.biomeColors.ContainsKey(biomeType))
                        settings.biomeColors.Add(biomeType, new BiomeColors());

                    Color[] oldBiomesColors = settings.biomeColors[biomeType].heightColors;
                    if (oldBiomesColors.Length != settings.heightLayers.Count)
                    {
                        Color[] newBiomeColors = new Color[settings.heightLayers.Count];
                        for (int i = 0; i < oldBiomesColors.Length; i++)
                            if (i < newBiomeColors.Length)
                                newBiomeColors[i] = oldBiomesColors[i];
                        settings.biomeColors[biomeType].heightColors = newBiomeColors;
                    }
                }
            }
        }

        private void DrawTableHeader()
        {
            var settings = (ColorsSettings)target;

            GUILayout.BeginHorizontal();
            GUILayout.Space(29);
            foreach (string biomeType in settings.biomeColors.Keys)
                EditorGUILayout.TextField(biomeType, EditorStyles.label);
            GUILayout.EndHorizontal();
        }

        private void DrawTable()
        {
            var settings = (ColorsSettings)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wtr", GUILayout.MaxWidth(25));
            foreach (string biomeType in settings.biomeColors.Keys)
                settings.biomeColors[biomeType].waterColor = EditorGUILayout.ColorField(settings.biomeColors[biomeType].waterColor);
            GUILayout.EndHorizontal();

            for (int i = 0; i < settings.heightLayers.Count; i++)
            {
                GUILayout.BeginHorizontal();
                settings.heightLayers[i] = EditorGUILayout.FloatField(settings.heightLayers[i], GUILayout.MaxWidth(25));
                foreach (string biomeType in settings.biomeColors.Keys)
                    settings.biomeColors[biomeType].heightColors[i] = EditorGUILayout.ColorField(settings.biomeColors[biomeType].heightColors[i]);
                GUILayout.EndHorizontal();
            }
        }

        private void DrawButtons()
        {
            var settings = (ColorsSettings)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add new height layer"))
                settings.heightLayers.Add(settings.heightLayers.Last() + 0.1f);
            if (settings.heightLayers.Count > 1 && GUILayout.Button("Remove last height layer"))
                settings.heightLayers.Remove(settings.heightLayers.Last());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill biomes colors"))
                FillBiomesColors();
            if (GUILayout.Button("Fill height colors"))
                FillHeightColors();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Lerp biome colors"))
                LerpBiomeColors();
            if (GUILayout.Button("Reset height"))
                ResetHeight();
            GUILayout.EndHorizontal();
        }

        private void FillBiomesColors()
        {
            var settings = (ColorsSettings)target;
            foreach (string biomeType in settings.biomeColors.Keys)
            {
                Color targetColor = Color.black;
                foreach (Color color in settings.biomeColors[biomeType].heightColors)
                {
                    if (color != Color.black)
                    {
                        targetColor = color;
                        break;
                    }
                }

                if (targetColor != Color.black)
                {
                    for (int i = 0; i < settings.biomeColors[biomeType].heightColors.Length; i++)
                    {
                        settings.biomeColors[biomeType].heightColors[i] = targetColor;
                    }
                }
            }
        }

        private void FillHeightColors()
        {
            var settings = (ColorsSettings)target;
            for (int i = 0; i < settings.heightLayers.Count; i++)
            {
                Color targetColor = Color.black;
                foreach (string biomeType in settings.biomeColors.Keys)
                {
                    if (settings.biomeColors[biomeType].heightColors[i] != Color.black)
                    {
                        targetColor = settings.biomeColors[biomeType].heightColors[i];
                        break;
                    }
                }
                if (targetColor != Color.black)
                {
                    foreach (string biomeType in settings.biomeColors.Keys)
                    {
                        settings.biomeColors[biomeType].heightColors[i] = targetColor;
                    }
                }
            }
            FillWaterColors();
        }

        private void FillWaterColors()
        {
            var settings = (ColorsSettings)target;

            Color targetColor = Color.black;
            foreach (string biomeType in settings.biomeColors.Keys)
            {
                if (settings.biomeColors[biomeType].waterColor != Color.black)
                {
                    targetColor = settings.biomeColors[biomeType].waterColor;
                    break;
                }
            }

            if (targetColor != Color.black)
            {
                foreach (string biomeType in settings.biomeColors.Keys)
                {
                    settings.biomeColors[biomeType].waterColor = targetColor;
                }
            }
        }

        private void LerpBiomeColors()
        {
            var settings = (ColorsSettings)target;
            foreach (string biomeType in settings.biomeColors.Keys)
            {
                Color startColor = settings.biomeColors[biomeType].heightColors[0];
                Color endColor = settings.biomeColors[biomeType].heightColors.Last();

                for (int i = 0; i < settings.heightLayers.Count; i++)
                {
                    settings.biomeColors[biomeType].heightColors[i] = Color.Lerp(startColor, endColor, settings.heightLayers[i]);
                }
            }
        }

        private void ResetHeight()
        {
            var settings = (ColorsSettings)target;
            for (int i = 0; i < settings.heightLayers.Count; i++)
            {
                settings.heightLayers[i] = i / ((float)settings.heightLayers.Count - 1);
            }
        }
    }
}
