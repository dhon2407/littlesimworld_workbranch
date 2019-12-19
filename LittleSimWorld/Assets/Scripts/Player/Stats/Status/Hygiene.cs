namespace PlayerStats
{
    public class Hygiene : Status
    {
        public Hygiene() { }

        public Hygiene(Data existingData)
        {
            data = existingData;
        }

        protected override void InitializeData()
        {
            type = Type.Hygiene;
            data.drainPerHour = -4.166667f;
        }
    }
}
