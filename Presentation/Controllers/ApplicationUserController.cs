using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.Enums;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        private IHostingEnvironment hostingEnv;
        private readonly IOrderRepository _orderRepository;

        public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOrderRepository orderRepository, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnv = env;
            _orderRepository = orderRepository;
            _signInManager = signInManager;
            
        }

        public void numberCount()
        {
            int pendingCount = _context.Orders.Where(x => x.IsDeliver == null && x.IsDelete == false).Count();
            ViewBag.orderCount = pendingCount;
            int productCount = _context.Products.Where(x => x.IsDelete == false).Count();
            ViewBag.productsCount = productCount;
            int categoryCount = _context.Categories.Where(x => x.IsDelete == false).Count();
            ViewBag.categoriesCount = categoryCount;
            var order = _orderRepository.GetAll();          
            var ordersAmount = _context.OrderDetails.GroupBy(x => x.CreatedAt.Value.Month).Select(g => new { Price = g.Sum(x => x.QtyPrice),Key=g.Key }).OrderBy(x=>x.Key).ToList();
            ViewBag.ordersAmount = JsonConvert.SerializeObject(ordersAmount);

            List<OrderDetailViewModel> bestSellerProducts = _context.OrderDetails
                             .Include(x => x.Product)
                             .GroupBy(x => new { x.ProductId, x.Product.ProductName, x.Product.ImgPath })
                             .Select(g => new OrderDetailViewModel
                             {
                                 Qty = g.Sum(x => x.Qty),
                                 Price = g.Sum(x => x.QtyPrice),
                                 ProductId = g.Key.ProductId,
                                 ProductName = g.Key.ProductName,
                                 ImgPath = g.Key.ImgPath
                             })
                             .OrderByDescending(x => x.Qty)
                             .Take(5)
                             .ToList();

            ViewBag.bestSellerProducts = bestSellerProducts;

            int totalAmount = 0;
            foreach (var item in order)
            {
                totalAmount += item.TotalPrice;
            }
            ViewBag.totalBalance = totalAmount;
        }
        public async Task<IActionResult> Dashboard()
        {
            numberCount();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewBag.userName = user.Name;
            ViewBag.userImgPath = user.ImgPath;
            return View(user);
        }
       

        public async Task<IActionResult> EditProfileAsync()
        {  
            numberCount();
            orderCount();
            ApplicationUser user =await _userManager.GetUserAsync(User);
            ViewBag.userName = user.Name;
            ViewBag.userImgPath = user.ImgPath;

            var loginUser = new RegisterViewModel
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImgPath=user.ImgPath
            };
            return View(loginUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(RegisterViewModel model, Models.FileUploadModel upload)
        {
            orderCount();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            user.Name = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.ImgPath = model.ImgPath;

            // file upload
            FileUpload(user, upload);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["notice"] = StatusEnum.NoticeStatus.EditProfile;
            }

            return Redirect("EditProfile");
        }
        #region<パスワード変更方法>
      
        public async Task<IActionResult> ChangePassword()
        {
            orderCount();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewBag.userName = user.Name;
            ViewBag.userImgPath = user.ImgPath;
            ViewBag.currentPasswordError = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(EditProfileModel editUser)

        {
            orderCount();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewBag.userName = user.Name;
            ViewBag.userImgPath = user.ImgPath;

            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await _userManager.ChangePasswordAsync(user, editUser.CurrentPassword, editUser.NewPassword);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                       if(error.Description.Equals("Incorrect password."))
                        {
                            ModelState.AddModelError(string.Empty, "Current Password is not match. Type Again!");
                        }
                       else
                        ModelState.AddModelError(string.Empty,error.Description);
                    }
                    return View();
                }
                
                  TempData["notice"] = StatusEnum.NoticeStatus.Change;
              
            }
            return View(editUser);
        }
        #endregion
        # region<ログイン方法>
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "ApplicationUser");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View(user);
        }
        #endregion
        #region<サインアップ>
        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterViewModel model, FileUploadModel uploadModel)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName=model.UserName,
                    Password=model.Password,
                    NRC=model.NRC,
                    Email=model.Email,
                    PhoneNumber=model.PhoneNumber
                };

                // file upload
                FileUpload(user, uploadModel);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                    return RedirectToAction("Login", "ApplicationUser");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        #endregion
        #region<ログアウト>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "ApplicationUser");
        }
        #endregion
        // for file upload
        public void FileUpload(ApplicationUser user, Models.FileUploadModel upload)
        {
            if (upload.File != null)
            {
                var FileDic = "Files";

                string FilePath = Path.Combine(hostingEnv.WebRootPath, FileDic);

                if (!Directory.Exists(FilePath))

                    Directory.CreateDirectory(FilePath);

                var fileName = upload.File.FileName;

                var filePath = Path.Combine(FilePath, fileName);

                using (FileStream fs = System.IO.File.Create(filePath))

                {
                    upload.File.CopyTo(fs);
                }

                user.ImgPath = fileName;
            }
        }

        // for view image
        public async Task TopBarInfo()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            ViewBag.userName = user.Name;
            ViewBag.userImgPath = user.ImgPath;
        }
        #region<注文数>
        public void orderCount()
        {
            int pendingCount = _context.Orders.Where(x => x.IsDeliver == null && x.IsDelete == false).Count();
            ViewBag.orderCount = pendingCount;

        }
        #endregion
    }
}