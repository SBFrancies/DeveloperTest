namespace ClearBank.DeveloperTest.Types
{
    public class MakePaymentResult
    {
        public bool Success { get; }

        private MakePaymentResult( bool success)
        {
            Success = success;
        }

        public static MakePaymentResult Failed()
        {
            return new MakePaymentResult(false);
        }

        public static MakePaymentResult Successful()
        {
            return new MakePaymentResult(true);
        }
    }
}
