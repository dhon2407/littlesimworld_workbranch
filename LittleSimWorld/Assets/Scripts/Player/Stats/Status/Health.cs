namespace PlayerStats
{
    public class Health : Status
    {
        protected override void InitializeData()
        {
            type = Type.Health;
            data.drainPerHour = -0.4166667f;
        }
    }
}
