﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Anime_Portal.Models;
using System.Web.Security;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.IO;
using Anime_Portal.Helplers;
using Anime_Portal.Helper;
using System.Drawing;

namespace Anime_Portal.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(string Name)
        {
            string Id = Name ?? User.Identity.GetUserName();

            var user = await UserManager.FindByNameAsync(Id);
            var model = new EditProfileViewModel()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                UserName = user.UserName,
                PictureUrl = user.PictureUrl

            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Edit(string Name , EditProfileViewModel model, HttpPostedFileBase picture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = null;
           if( UserManager.IsInRole(User.Identity.GetUserId(), "Administrators"))
            {
                user = await UserManager.FindByNameAsync(Name);
            }
            else
            {
                user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            }
     
            user.DisplayName = model.DisplayName;
            user.Email = model.Email;
            var pictureUrl = FileHelper.GetFileName(user.UserName, picture);
            var avatarUrl = FileHelper.GetFileName(user.UserName + "mini", picture);
            if (pictureUrl != null)
            {
                var path = Path.Combine(Server.MapPath("~/Upload/ProfilePictures"), pictureUrl);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        var profilePicture = Request.Files[0];
                        Image img = Image.FromStream(profilePicture.InputStream);
                        img =FileHelper.ResizeImage(img, 100, 80); 
                        // Fájl mentése a fájlrendszerve
                        img.Save(path);
                user.PictureUrl = pictureUrl;
            }
            if (avatarUrl != null)
            {
                var path = Path.Combine(Server.MapPath("~/Upload/ProfilePictures"), avatarUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                var profilePicture = Request.Files[0];
                Image img = Image.FromStream(profilePicture.InputStream);
                img = FileHelper.ResizeImage(img, 50, 50);
                // Fájl mentése a fájlrendszerve
                img.Save(path);
                user.Avatar = avatarUrl;
            }

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var userIn = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user.UserName.Equals(userIn.UserName))
                {
                    Session["DisplayName"] = user.DisplayName;
                    Session["AvatarUrl"] = user.Avatar;
                }
        
                if (UserManager.IsInRole(User.Identity.GetUserId(), "Administrators"))
                {
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditProfileSuccess });
                }
                else
                {
                    return RedirectToAction("Details", new { Message = ManageMessageId.EditProfileSuccess });
                }

            }
            AddErrors(result);
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Administrators")]
        public async Task<ActionResult> EditAdmin(string UserName)
        {

            var user = await UserManager.FindByNameAsync(UserName);

            var model = new EditProfileViewModel()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                UserName = user.UserName,
                PictureUrl = user.PictureUrl
            };
            return View("Edit", model);
        }

        private void status(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
             message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
             : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
             : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
             : message == ManageMessageId.Error ? "An error has occurred."
             : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
             : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
             : message == ManageMessageId.EditProfileSuccess ? "Your profile has been changed."
             : "";
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Details(ManageMessageId? message,string Name)
        {
            string Id = Name ?? User.Identity.GetUserName();
            var user = await UserManager.FindByNameAsync(Id);

            var model = new DetailsProfileModel
            {
                HasPassword = HasPassword(),            
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName,
                PictureUrl = user.PictureUrl,
                RegisterDate = user.RegisterDate
            };

            return View(model);
        }


        //
        // GET: /Manage/Index
        [Authorize(Roles = "Administrators")]
        public ActionResult Index()
        {

            var users = UserManager.Users.ToList();
            List<IndexViewModel> model = new List<IndexViewModel>();
            foreach (var user in users)
            {
                model.Add(new IndexViewModel()
                {
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Id = user.Id
                });
            }


            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Administrators")]
        public async Task<ActionResult> Delete(string UserName)
        {
            
            var user = await UserManager.FindByNameAsync(UserName);
            if (!User.Identity.Name.Equals(user.UserName))
            {
                await UserManager.DeleteAsync(user);
            }
            //return RedirectToAction("Index");
            return View();
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            EditProfileSuccess,
            Error
        }

        #endregion
    }
}