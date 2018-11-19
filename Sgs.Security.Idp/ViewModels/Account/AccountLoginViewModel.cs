using System.ComponentModel.DataAnnotations;

namespace Sgs.Security.Idp.ViewModels.Account
{
    public class AccountLoginViewModel
    {
        [Required(ErrorMessage = "Username is required! - !اسم المستخدم مطلوب")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required! - !كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me - تذكرني ")]
        public bool RememberMe { get; set; }
    }
}
