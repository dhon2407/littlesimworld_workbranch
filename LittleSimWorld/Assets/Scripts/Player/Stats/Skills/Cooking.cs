using UnityEngine;

namespace PlayerStats
{
    public class Cooking : Skill
    {
        protected override void Initialize()
        {
            name = nameof(Cooking);
            type = Type.Cooking;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Cooking;
        }
    }
}
