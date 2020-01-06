using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    public class TTAreaParams1 : TooltipArea<string>
    {
        [SerializeField] private string dataToShow = "Test Data";

        protected override string TooltipData => dataToShow;
    }
}