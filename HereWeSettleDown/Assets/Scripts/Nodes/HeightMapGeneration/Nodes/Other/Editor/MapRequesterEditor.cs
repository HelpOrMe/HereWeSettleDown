using UnityEngine;
using XNodeEditor;

namespace Nodes.HeightMapGeneration.Other
{
    [CustomNodeEditor(typeof(MapRequester))]
    public class MapRequesterEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            GUI.color = Color.Lerp(Color.green, Color.white, 0.5f);
        }
    }
}
