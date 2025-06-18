using System.Threading.Tasks;

public interface IAccountRepository
{
    void Save(AccountDTO accountDto);
    Task<AccountDTO> FindAsync(string email);
}