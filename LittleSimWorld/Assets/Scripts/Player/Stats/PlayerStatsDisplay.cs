using System.Collections;
using TMPro;
using UnityEngine;

namespace PlayerStats
{
    using Type = Skill.Type;

    public class PlayerStatsDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI PhysicsLVLText = null;
        [SerializeField]
        private TextMeshProUGUI PhysicsXPText = null;
        [SerializeField]
        private TextMeshProUGUI StrengthLVLText = null;
        [SerializeField]
        private TextMeshProUGUI StrengthXPText = null;
        [SerializeField]
        private TextMeshProUGUI CharismaLVLText = null;
        [SerializeField]
        private TextMeshProUGUI CharismaXPText = null;
        [SerializeField]
        private TextMeshProUGUI FitnessLVLText = null;
        [SerializeField]
        private TextMeshProUGUI FitnessXPText = null;
        [SerializeField]
        private TextMeshProUGUI CookingLVLText = null;
        [SerializeField]
        private TextMeshProUGUI CookingXPText = null;
        [SerializeField]
        private TextMeshProUGUI RepairLVLText = null;
        [SerializeField]
        private TextMeshProUGUI RepairXPText = null;

        [Space]
        [SerializeField]
        private TextMeshProUGUI TotalLevelText = null;

        private void Start()
        {
            StartCoroutine(RegisterSkillChanges());
        }

        private IEnumerator RegisterSkillChanges()
        {
            while (!Stats.Ready || !Stats.Initialized)
                yield return null;

            Stats.OnSkillUpdate.AddListener(UpdateSkillDisplay);

            Stats.UpdateSkillsData();
        }

        private void UpdateSkillDisplay(Type type)
        {
            TextMeshProUGUI lvlText = null;
            TextMeshProUGUI xpText = null;

            switch (type)
            {
                case Type.Strength:
                    lvlText = StrengthLVLText;
                    xpText = StrengthXPText;
                    break;
                case Type.Fitness:
                    lvlText = FitnessLVLText;
                    xpText = FitnessXPText;
                    break;
                case Type.Intelligence:
                    lvlText = PhysicsLVLText;
                    xpText = PhysicsXPText;
                    break;
                case Type.Cooking:
                    lvlText = CookingLVLText;
                    xpText = CookingXPText;
                    break;
                case Type.Charisma:
                    lvlText = CharismaLVLText;
                    xpText = CharismaXPText;
                    break;
                case Type.Repair:
                    lvlText = RepairLVLText;
                    xpText = RepairXPText;
                    break;

                case Type.Writing:
                default:
                    Debug.LogWarning($"No dedicated text display for skill {type}");
                    break;
            }

            if (lvlText != null & xpText != null)
            {
                lvlText.text = (Stats.SkillLevel(type) == 0) ? "-" : Stats.SkillLevel(type).ToString("0");
                xpText.text = $" XP: {Stats.Skill(type).GetData().xp}/{Stats.Skill(type).GetData().requiredXP}";
            }

            int totalLevel = 0;
            foreach (var skill in Stats.PlayerSkills.Values)
                totalLevel += skill.CurrentLevel;

            TotalLevelText.text = (totalLevel == 0) ? "-" : totalLevel.ToString("0");
        }
    }
}
