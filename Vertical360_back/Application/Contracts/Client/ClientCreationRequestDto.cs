using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Application.Contracts.Client
{
    public class ClientCreationRequestDto
    {
        //[Required]
        //public string BusinessName { get; set; }

        //[Required]
        //public string TipeDocument { get; set; }

        //[Required]
        //public string NumberDocument { get; set; }
        
        [Required]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; }

        [Required]
        public string AdminPassword { get; set; }

        //[Required]
        //public string neighborhood { get; set; }

        //[Required]
        //public string Address { get; set; }

        //[Required]
        //public int Departament { get; set; }

        //[Required]
        //public string PhoneNumber { get; set; }

        //[Required]
        //public string MovileNumber { get; set; }
    }
}
