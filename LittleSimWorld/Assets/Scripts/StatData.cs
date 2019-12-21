using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PlayerStats;

namespace CharacterStats
{
    public class StatData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string description;
        public bool showXp;
        public Skill.Type XpText;
      
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (showXp)
                StatsTooltip.Instance.ShowDescription(description + Mathf.Abs( Stats.Skill(XpText).GetData().xp));
            else
                StatsTooltip.Instance.ShowDescription(description);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StatsTooltip.Instance.HideDescription();
        }
        private void OnDestroy()
        {
            StatsTooltip.Instance.HideDescription();
        }
    }
}