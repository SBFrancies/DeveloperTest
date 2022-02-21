using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Interface
{
    public interface IDataStore
    {
        Account GetAccount(string accountNumber);

        void UpdateAccount(Account account);
    }
}
