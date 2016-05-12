using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockScream.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> RolesList { get; set; }

        public bool ResetPassword { get; set; }
    }
}