namespace PlayerStats
{
    public class Energy : Status
    {
        protected override void InitializeData()
        {
            type = Type.Energy;
            data.drainPerHour = -8.333333f;
        }
    }
}
