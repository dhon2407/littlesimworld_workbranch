namespace PlayerStats
{
    public class Bladder : Status
    {
        protected override void InitializeData()
        {
            type = Type.Bladder;
            data.drainPerHour = -2.083333f;
        }
    }
}
