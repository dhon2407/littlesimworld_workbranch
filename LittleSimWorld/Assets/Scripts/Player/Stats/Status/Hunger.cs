namespace PlayerStats
{
    public class Hunger : Status
    {
        public Hunger() { }

        public Hunger(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            Stats.Remove(Type.Health, data.drainPerHourPunished * timeScale);
        }

        protected override void InitializeData()
        {
            type = Type.Hunger;
            data.drainPerHour = -4.166667f;
        }
    }
}
