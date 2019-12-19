namespace PlayerStats
{
    public class Mood : Status
    {
        public Mood() { }

        public Mood(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            Stats.Remove(Type.Health, data.drainPerHourPunished * timeScale);
        }

        protected override void InitializeData()
        {
            type = Type.Mood;
            data.drainPerHour = -2.083333f;
        }
    }
}
