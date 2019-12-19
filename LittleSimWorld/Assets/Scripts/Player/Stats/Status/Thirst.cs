namespace PlayerStats
{
    public class Thirst : Status
    {
        public Thirst() { }

        public Thirst(Data existingData)
        {
            data = existingData;
        }

        protected override void InitializeData()
        {
            type = Type.Thirst;
            data.drainPerHour = -8.333333f;
        }
    }
}
