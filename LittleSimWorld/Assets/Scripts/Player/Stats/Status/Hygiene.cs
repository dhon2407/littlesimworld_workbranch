namespace PlayerStats
{
    public class Hygiene : Status
    {
        protected override void InitializeData()
        {
            type = Type.Hygiene;
            data.drainPerHour = -4.166667f;
        }
    }
}
