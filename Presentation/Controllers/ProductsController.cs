using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using InfraStructure.Enums;
using InfraStructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Presentation.Models;
using Microsoft.Extensions.Options;
using X.PagedList;

namespace Presentation.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        #region <<変数>>
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment hostingEnv;                         //ファイルアップロード用
        private readonly ICategoryRepository _categoryRepository;       
        private readonly Pagination _pagination;                        //ページネーション用
        #endregion

        #region<<コンストラクタ>>
        public ProductsController(ApplicationDbContext context, IProductRepository productRepository, UserManager<ApplicationUser> userManager, IHostingEnvironment env, ICategoryRepository categoryRepository, IOptions<Pagination> pagination)
        {
            _context = context;
            _productRepository = productRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            this.hostingEnv = env;
            _pagination = pagination.Value;
        }
        
        #endregion

        #region<<一般的に使用されるメソッド>>

        //初期化機能
        public async void Initialize(int catId)
        {
            orderCount();
            TopBarInfo();

            //Category Name を取得する
            var category = _context.Categories.Where(x => x.IsDelete == false).ToList();
            ViewData["Categories"] = new SelectList(category, "Id", "Name", catId);
        }

        //注文カウント機能
        public void orderCount()
        {
            int pendingCount = _context.Orders.Where(x => x.IsDeliver == null && x.IsDelete == false).Count();
            ViewBag.orderCount = pendingCount;

        }

        //Top Bar 情報の機能
        public void TopBarInfo()
        {
            //ユーザー名とユーザーimage 情報を取得する
            var userList = _context.ApplicationUsers.Where(x => x.UserName == User.Identity.Name).ToList();
            ViewBag.userName = userList[0].Name;
            ViewBag.userImgPath = userList[0].ImgPath;
        }

        //ファイルアップロードの機能
        public void FileUpload(Product product, Models.FileUploadModel upload)
        {
            //ファイルが null でない場合
            if (upload.File != null)
            {
                var FileDic = "Files";
                string FilePath = Path.Combine(hostingEnv.WebRootPath, FileDic);

                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                //image ァイル名を設定する
                var fileName = upload.File.FileName;
                var filePath = Path.Combine(FilePath, fileName);

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    upload.File.CopyTo(fs);
                }

                product.ImgPath = fileName;
            }
        }

        #endregion

        #region<<アクション>>

        //Index アクション
        public IActionResult Index(int? page = 1,string ProductName = null,int CategoryId = 0)
        {
            orderCount();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
           Initialize(CategoryId);

            //Category から探す
            if (ProductName == null)
            {
                //get all products
                if (CategoryId == 0)
                {
                    return View(_productRepository.GetAll().ToPagedList((int)page, pageSize));
                }
                //Category だけから探す
                else
                {
                    var products = _context.Products.Where(x =>x.CategoryId==CategoryId && x.IsDelete == false).ToList();
                    foreach (var item in products)
                    {
                        item.Category = _context.Categories.Find(item.CategoryId);
                    }
                    return View(products.ToPagedList((int)page, pageSize));
                }
            }
            //Product Name から探す
            else
            {
                //Product Name だけから探す
                if (CategoryId == 0)
                {
                    var products = _context.Products.Where(x => x.ProductName.Contains(ProductName) && x.IsDelete == false).ToList();
                    foreach (var item in products)
                    {
                        item.Category = _context.Categories.Find(item.CategoryId);
                    }
                    return View(products.ToPagedList((int)page, pageSize));
                }
                //Category と Product Name の両方で検索
                else 
                {
                    var products = _context.Products.Where(x => x.ProductName.Contains(ProductName) && x.CategoryId == CategoryId && x.IsDelete == false).ToList();
                    foreach (var item in products)
                    {
                        item.Category = _context.Categories.Find(item.CategoryId);
                    }
                    return View(products.ToPagedList((int)page, pageSize));
                }
                
            }
        }

        //Create[GET] アクション
        public IActionResult Create()
        {

            Initialize(0);
            return View();
        }

        //Create[POST] アクション
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, Models.FileUploadModel upload)
        {
            if (ModelState.IsValid)
            {
                product = await _userManager.AddUserAndTimestamp(product, User, DbEnum.DbActionEnum.Create);

                //ファイルをアップロードする
                FileUpload(product, upload);

                //Product を保存する
                var _product = await _productRepository.AddAsync(product);
                if (_product != null)
                {
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                }

                return RedirectToAction(nameof(Index));
            }
            Initialize(0);
            return View(product);
        }

        //Edit[GET] アクション
        public IActionResult Edit(int id)
        {
            Initialize(0);
            var product = _productRepository.Get(id);
            return View(product);
        }

        //Edit[POST] アクション
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, Models.FileUploadModel upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product = await _userManager.AddUserAndTimestamp(product, User, DbEnum.DbActionEnum.Update);

                    //Product を更新する
                    FileUpload(product, upload);
                    var _product = await _productRepository.UpdateAsync(product);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
               Console.WriteLine(e.InnerException.Message);
            }
            Initialize(product.CategoryId);
            return View(product);
        }

        //Delete アクション
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Product を削除する
                var _prodcut = await _productRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

    }
}
