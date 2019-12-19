using PlayerSkills = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill>;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;

namespace PlayerStats
{
    public class PlayerStatsUpdater
    {
        public void UpdateStatus(PlayerStatus playerStats, float timeScale)
        {
            //timeScale = (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            bool notPassOutAndActive = !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf;

            foreach (var item in playerStats)
            {
                var stat = item.Value;
                if (stat.CurrentAmount > 0)
                    stat.Drain(timeScale);

                if (stat.CurrentAmount <= 0 && notPassOutAndActive)
                    stat.ZeroPenalty(timeScale);
            }
        }
    }
}
