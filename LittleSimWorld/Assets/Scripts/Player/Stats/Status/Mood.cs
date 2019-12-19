namespace PlayerStats
{
    public class Mood : Status
    {
        protected override void InitializeData()
        {
            type = Type.Mood;
            data.drainPerHour = -2.083333f;
        }
    }
}
