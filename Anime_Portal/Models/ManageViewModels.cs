using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Anime_Portal.Validator;
using System.Web;
using System;

namespace Anime_Portal.Models
{

    
    public class DetailsModel
    {
        [Display(Name ="Jelszó")]
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }



    }

    public class IndexViewModel
    {
        [Display(Name="Felhasználónév")]
        public string UserName { get; set; }
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Display(Name ="Teljes név")]
        public string DisplayName { get; set; }
        public string Id { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }


    public class EditProfileViewModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }

        [Display(Name ="Profilkép")]
        [ValidateFile(ErrorMessage = "Nem megfelelő formátum")]
        public HttpPostedFileBase Picture { get; set; }

        [Display(Name = "Profilkép")]
        public string PictureUrl { get; set; }
    }
    public class DetailsProfileModel
    {
        [Display(Name = "Jelszó")]
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }

        [Display(Name ="Felhasználónév")]
        public string UserName { get; set; }
        [Display(Name ="Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name ="Profilkép")]
        public string PictureUrl { get; set; }
        [Display(Name ="Teljes név")]
        public string DisplayName { get; set; }

        public DateTime RegisterDate { get; set; }
        public string Status { get; set; }
    }
}