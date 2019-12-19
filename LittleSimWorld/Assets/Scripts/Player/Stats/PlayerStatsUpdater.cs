using PlayerSkills = System.Collections.Generic.Dictionary<PlayerStats.Skill.Type, PlayerStats.Skill>;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;

namespace PlayerStats
{
    public class PlayerStatsUpdater
    {
        public void UpdateStatus(PlayerStatus playerStats, float timeScale)
        {
            //timeScale = (Time.deltaTime / GameClock.Speed)) * GameClock.TimeMultiplier);
            notPassOutAndActive = !GameLibOfMethods.passedOut && GameLibOfMethods.player.gameObject.activeSelf;

            foreach (var stat in playerStats)
            {
                if (stat.Value.CurrentAmount > 0)
                    stat.Value.Drain(timeScale);

                if (stat.Value.CurrentAmount <= 0 && notPassOutAndActive)
                    stat.Value.ZeroPenalty(timeScale);
            }


            if (Bladder.Instance.CurrentAmount > 0)
            {
                if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
                {
                    Bladder.Instance.Add((((Bladder.Instance.DrainSpeedPerHour) * timeScale / 2);
                }
                else
                {
                    Bladder.Instance.Add(((Bladder.Instance.DrainSpeedPerHour) * timeScale;
                }
            }
            if (Bladder.Instance.CurrentAmount <= 0 && passOutAndActive)
            {

                GameLibOfMethods.animator.SetTrigger("PissingInPants");
            }

            if (Hygiene.Instance.CurrentAmount > 0)
            {
                if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
                {
                    Hygiene.Instance.Add((((Hygiene.Instance.DrainSpeedPerHour) * timeScale / 2);
                }
                else
                {
                    Hygiene.Instance.Add(((Hygiene.Instance.DrainSpeedPerHour) * timeScale;
                }
            }
            if (Hygiene.Instance.CurrentAmount <= 0 && passOutAndActive)
            {
                Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * timeScale;
            }

            if (Thirst.Instance.CurrentAmount > 0)
            {
                if (GameLibOfMethods.isSleeping || JobManager.Instance.isWorking)
                {
                    Thirst.Instance.Add((((Thirst.Instance.DrainSpeedPerHour) * timeScale / 2);
                }
                else
                {
                    Thirst.Instance.Add(((Thirst.Instance.DrainSpeedPerHour) * timeScale;
                }
            }
            if (Thirst.Instance.CurrentAmount <= 0 && passOutAndActive)
            {
                Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * timeScale;
            }

            if (Energy.Instance.CurrentAmount > 0)
            {
                if (GameLibOfMethods.isSleeping)
                {
                    Energy.Instance.Add((((Energy.Instance.DrainSpeedPerHour) * timeScale / 2);
                }
                else
                {
                    Energy.Instance.Add(((Energy.Instance.DrainSpeedPerHour) * timeScale;
                }
            }
            if (Energy.Instance.CurrentAmount <= 0 && passOutAndActive)
            {
                GameLibOfMethods.animator.SetTrigger("PassOutToSleep");

            }
            if (Mood.Instance.CurrentAmount > 0)
            {
                if (GameLibOfMethods.isSleeping)
                {
                    Mood.Instance.Add((((Mood.Instance.DrainSpeedPerHour) * timeScale / 2);
                }
                else
                {
                    Mood.Instance.Add(((Mood.Instance.DrainSpeedPerHour) * timeScale;
                }
            }

            if (Mood.Instance.CurrentAmount <= 0 && passOutAndActive)
            {
                Health.Instance.Add(((Health.Instance.DrainSpeedPerHourIfPunished) * timeScale;
            }
        }
    }
}
