using TMPro;
using UnityEngine;

namespace PlayerStats
{
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
    }
}
