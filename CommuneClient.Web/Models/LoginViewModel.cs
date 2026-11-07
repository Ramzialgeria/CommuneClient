using System.ComponentModel.DataAnnotations;

namespace CommuneClient.Web.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "يرجى إدخال اسم المستخدم.")]
    [Display(Name = "اسم المستخدم")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "يرجى إدخال كلمة المرور.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة المرور")]
    public string Password { get; set; } = "";
}
