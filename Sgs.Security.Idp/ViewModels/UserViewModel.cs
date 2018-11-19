using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Security.Idp.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Employee id is required! - !الرقم الوظيفي مطلوب")]
        [Range(1, 10000)]
        [Remote(action: "VerifyEmployeeId", controller: "Users", AdditionalFields = nameof(Id))]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required! - !الاسم مطلوب")]
        [StringLength(100, MinimumLength = 20)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "User name is required! - !اسم المستخدم مطلوب")]
        [StringLength(10, MinimumLength = 2)]
        [Remote(action: "VerifyUsername", controller: "Users",AdditionalFields =nameof(Id))]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required! - !البريد الالكتروني مطلوب")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
