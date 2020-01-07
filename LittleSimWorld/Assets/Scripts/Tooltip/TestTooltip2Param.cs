using TMPro;
using UnityEngine;

namespace LSW.Tooltip
{
    public class TestTooltip2Param : Tooltip<TestParams>
    {
        [SerializeField] private TextMeshProUGUI textmesh1 = null;
        [SerializeField] private TextMeshProUGUI textmesh2 = null;

        protected override Vector2 MousePosition => new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        public override void SetData(TestParams data)
        {
            textmesh1.text = data.param1;
            textmesh2.text = data.param2;
        }
    }

    public struct TestParams
    {
        public string param1;
        public string param2;
    }
}