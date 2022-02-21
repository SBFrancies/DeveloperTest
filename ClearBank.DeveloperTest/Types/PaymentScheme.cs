using System;

namespace ClearBank.DeveloperTest.Types
{
    [Flags]
    public enum PaymentScheme
    {
        FasterPayments,
        Bacs,
        Chaps
    }
}
