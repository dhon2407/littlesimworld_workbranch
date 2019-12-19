namespace PlayerStats
{
    public class Thirst : Status
    {
        protected override void InitializeData()
        {
            type = Type.Thirst;
            data.drainPerHour = -8.333333f;
        }
    }
}
