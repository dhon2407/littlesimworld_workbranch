namespace PlayerStats
{
    public class Hygiene : Status
    {
        public Hygiene() { }

        public Hygiene(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            Stats.Remove(Type.Health, data.drainPerHourPunished * timeScale);
        }

        protected override void InitializeData()
        {
            type = Type.Hygiene;
            data.drainPerHour = -4.166667f;
        }
    }
}
