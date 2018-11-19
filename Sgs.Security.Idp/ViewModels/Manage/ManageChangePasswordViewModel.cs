using System.ComponentModel.DataAnnotations;

namespace Sgs.Security.Idp.ViewModels.Manage
{
    public class ManageChangePasswordViewModel : UserChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
    }
}
