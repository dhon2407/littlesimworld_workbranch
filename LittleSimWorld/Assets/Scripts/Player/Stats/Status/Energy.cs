namespace PlayerStats
{
    public class Energy : Status
    {
        public Energy() { }

        public Energy(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            GameLibOfMethods.animator.SetTrigger("PassOutToSleep");
        }

        protected override void InitializeData()
        {
            type = Type.Energy;
            data.drainPerHour = -8.333333f;
        }
    }
}
