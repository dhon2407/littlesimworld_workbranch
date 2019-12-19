namespace PlayerStats
{
    public class Health : Status
    {
        public Health() { }

        public Health(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            //Move to proper place to indicate passing out.
            passingOut = true;
            GameLibOfMethods.PassOut();
        }

        protected override void InitializeData()
        {
            type = Type.Health;
            data.drainPerHour = -0.4166667f;
        }
    }
}
