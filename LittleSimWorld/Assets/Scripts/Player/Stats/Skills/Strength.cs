using UnityEngine;

namespace PlayerStats
{
    public class Strength : Skill
    {
        protected override void Initialize()
        {
            name = nameof(Strength);
            type = Type.Strength;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Strength;
        }
    }
}
