namespace PlayerStats
{
    public class Hunger : Status
    {
        protected override void InitializeData()
        {
            type = Type.Hunger;
            data.drainPerHour = -4.166667f;
        }
    }
}
