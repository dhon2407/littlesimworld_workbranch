namespace PlayerStats
{
    public class Bladder : Status
    {
        public Bladder() { }

        public Bladder(Data existingData)
        {
            data = existingData;
        }

        public override void ZeroPenalty(float timeScale)
        {
            GameLibOfMethods.animator.SetTrigger("PissingInPants");
        }

        protected override void InitializeData()
        {
            type = Type.Bladder;
            data.drainPerHour = -2.083333f;
        }
    }
}
