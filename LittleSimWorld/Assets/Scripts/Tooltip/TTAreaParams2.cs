using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    public class TTAreaParams2 : TooltipArea<TestParams>
    {
        [SerializeField] private string dataToShow = "Test Data1";
        [SerializeField] private string dataToShow2 = "Test Data2";
        
        protected override TestParams TooltipData => new TestParams {param1 = dataToShow, param2 = dataToShow2};
    }
}