using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PasswordChecker.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PasswordChecker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordHelper _passwordHelper;
        
        public PasswordController(IPasswordHelper passwordHelper)
        {
            _passwordHelper = passwordHelper;
        }
        
        [HttpPost]
        public async  Task<PasswordResponseDto> CheckPasswordStrength([FromBody] Password value)
        {
          var passStrength= _passwordHelper.GetPasswordScore(value.PasswordToCheck).ToString();
          var breachCount = await _passwordHelper.GetPasswordBreachCount(value.PasswordToCheck);
          return new PasswordResponseDto() {Strength = passStrength, BreachCount = breachCount};
        }
    }

    public class Password
    {
        public string PasswordToCheck;
    }
    public class PasswordResponseDto
    {
        public string Strength;
        public int BreachCount;
    }

}
