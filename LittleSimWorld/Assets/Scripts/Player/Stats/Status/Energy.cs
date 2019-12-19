namespace PlayerStats
{
    public class Energy : Status
    {
        public Energy() { }

        public Energy(Data existingData)
        {
            data = existingData;
        }

        protected override void InitializeData()
        {
            type = Type.Energy;
            data.drainPerHour = -8.333333f;
        }
    }
}
