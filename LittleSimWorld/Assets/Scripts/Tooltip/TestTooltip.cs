using TMPro;
using UnityEngine;

namespace LSW.Tooltip
{
    
    public class TestTooltip : Tooltip<string>
    {
        [SerializeField] private TextMeshProUGUI textmesh = null;
        protected override Vector2 MousePosition => new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        public override void SetData(string data)
        {
            textmesh.text = data;
        }
    }
}
