namespace PlayerStats
{
    public class Thirst : Status
    {
        public Thirst() { }

        public Thirst(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            Stats.Remove(Type.Health, data.drainPerHourPunished * timeScale);
        }

        protected override void InitializeData()
        {
            type = Type.Thirst;
            data.drainPerHour = -8.333333f;
        }
    }
}
