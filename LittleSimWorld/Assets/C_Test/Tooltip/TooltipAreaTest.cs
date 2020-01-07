using System;
using System.Collections;
using GUI_Animations;
using LSW.Tooltip;
using NSubstitute;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine.TestTools;
using UnityEngine;
using static UnityEngine.Object;

namespace TooltipTests
{
    public class TooltipAreaTest
    {
        private const int MagicNumber = 7;
        private GameObject _gameObj;
        private TestTooltipArea _tooltipArea;
        private ITooltip<int> _tooltipStub;

        [SetUp]
        public void Setup()
        {
            _gameObj = Instantiate(new GameObject());
            _tooltipStub = Substitute.For<ITooltip<int>>();

            _tooltipArea = _gameObj.AddComponent<TestTooltipArea>();
            _tooltipArea.tooltipData = MagicNumber;
            _tooltipArea.Init(_tooltipStub);
        }

        [UnityTest]
        public IEnumerator Tooltip_Shows_When_MouseEnter()
        {
            yield return null;
            
            _tooltipStub.Received().Show();
            _tooltipStub.Received().SetData(MagicNumber);
            _tooltipStub.DidNotReceive().Hide();
            
            //TODO: Simulate OnMouseEnter
            Assert.Fail("Simulate OnMouseEnter needed");
        }

        [UnityTest]
        public IEnumerator Tooltip_Hide_When_MouseExit()
        {
            yield return null;
            
            _tooltipStub.DidNotReceive().Show();
            _tooltipStub.DidNotReceive().SetData(Arg.Any<int>());
            _tooltipStub.Received().Hide();
            
            //TODO: Simulate OnMouseEnter
            Assert.Fail("Simulate OnMouseExit needed");
        }

        private class TestTooltipArea : TooltipArea<int>
        {
            public int tooltipData;
            protected override int TooltipData => tooltipData;
        }
    }
}