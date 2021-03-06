using GUI_Animations;
using LSW.Tooltip;
using UnityEngine;
using Zenject;

public class ToolTipInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ITooltip<string>>().To<TestTooltip>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ITooltip<TestParams>>().To<TestTooltip2Param>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IUiPopup>().To<TooltipPopup>().FromComponentSibling().AsTransient();
    }
}