using System.Threading.Tasks;
using PasswordChecker.Services.Enums;

namespace PasswordChecker.Services
{
    public interface IPasswordHelper
    {
        PasswordScore GetPasswordScore(string password);
        Task<int> GetPasswordBreachCount(string passwordToCheck);
    }
}
