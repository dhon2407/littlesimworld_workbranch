namespace PlayerStats
{
    public class Mood : Status
    {
        public Mood() { }

        public Mood(Data existingData)
        {
            data = existingData;
        }

        protected override void InitializeData()
        {
            type = Type.Mood;
            data.drainPerHour = -2.083333f;
        }
    }
}
