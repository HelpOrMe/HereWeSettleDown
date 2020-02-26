using UnityEngine;
using XNodeEditor;

namespace Nodes.HeightMapGeneration.Other
{
    [CustomNodeEditor(typeof(MapVisualizer))]
    public class MapVisualizerEditor : NodeEditor
    {
        HeightMap heightMap;

        private void SetTexture(HeightMap heightMap)
        {
            var node = (MapVisualizer)target;

            node.texture = new Texture2D(heightMap.width, heightMap.height);
            for (int x = 0; x < heightMap.width; x++)
            {
                for (int y = 0; y < heightMap.height; y++)
                {
                    node.texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, heightMap[x, y]));
                }
            }
            node.texture.Apply();
        }

        public void SetHeightMap()
        {
            var node = (MapVisualizer)target;
            heightMap = node.GetInputValue<HeightMap>("heightMap");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            var node = (MapVisualizer)target;
            
            if (GUILayout.Button("Update preview"))
            {
                heightMap = node.GetInputValue<HeightMap>("heightMap");

                if (heightMap != null)
                {
                    SetTexture(heightMap);
                }
            }

            // Draw texture
            GUIStyle style = new GUIStyle();
            style.fixedHeight = 175;
            try { GUILayout.Label(new GUIContent(node.texture), style); }
            catch { }

        }
    }
}
