using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Security.Idp.Models
{
    public class SgsUser : IdentityUser
    {
        [Required(ErrorMessage ="Employee id is required !")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage ="Employee name is required !")]
        public string Name { get; set; }

    }
}
