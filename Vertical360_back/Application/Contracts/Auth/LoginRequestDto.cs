using System.ComponentModel.DataAnnotations;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Application.Contracts.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string Document { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        public bool RememberMe { get; set; }
    }
}
