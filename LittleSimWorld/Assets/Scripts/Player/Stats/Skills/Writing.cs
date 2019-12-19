using UnityEngine;

namespace PlayerStats
{
    public class Writing : Skill
    {
        protected override void Initialize()
        {
            name = nameof(Writing);
            type = Type.Writing;
        }

        protected override Sprite GetSkillSprite()
        {
            return UIManager.Instance.Writing;
        }
    }
}
