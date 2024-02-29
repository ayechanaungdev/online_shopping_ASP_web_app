using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using InfraStructure.Enums;
using InfraStructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Enums;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Presentation.Controllers
{
    /// <概要>
    /// カテゴリチェック
    /// </概要>
    [Authorize]
    public class CategoriesController : Controller
    {
        #region <<定義>>
        #region <<読み取り専用>>

        /// <summary>_コンテクスト</summary>
        private readonly ApplicationDbContext _context;
        /// <summary>_カテゴリリポジトリ</summary>
        private readonly ICategoryRepository _categoryRepository;
        /// <summary>_ユーザーマネージャー</summary>
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>_ページネーション</summary>
        private readonly Pagination _pagination;

        #endregion
        #endregion

        #region <<コンストラクタ>>

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="categoryRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="pagination"></param>
        public CategoriesController(ApplicationDbContext context, ICategoryRepository categoryRepository,UserManager<ApplicationUser> userManager, IOptions<Pagination> pagination)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _pagination = pagination.Value;
        }
        #endregion

        /// <summary>
        /// インデックスページ
        /// </summary>
        /// <param name="page"></param>
        /// <param name="CategoryName"></param>
        /// <returns>カテゴリリスト、ページ番号、ページサイズを返す</returns>
        public IActionResult Index(int? page = 1,string CategoryName = null)
        {
            orderCount();
            TopBarInfo();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            if (CategoryName==null)
            {
                return View(_categoryRepository.GetAll().ToPagedList((int)page, pageSize));
            }
            else
            {
                var categories=_context.Categories.Where(x => x.Name.Contains(CategoryName) && x.IsDelete == false).ToList();
                return View(categories.ToPagedList((int)page,pageSize));
            }       
        }
        /// <summary>
        /// 顧客注文数
        /// </summary>
        #region<注文数>
        public void orderCount()
        {
            int pendingCount = _context.Orders.Where(x => x.IsDeliver == null && x.IsDelete == false).Count();
            ViewBag.orderCount = pendingCount;
        }
        #endregion
        /// <summary>
        /// 注文を作成する
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            orderCount();
            return View();
        }
        /// <summary>
        /// カテゴリ作成ページにデータを送る
        /// </summary>
        /// <param name="category"></param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    category = await _userManager.AddUserAndTimestamp(category, User, DbEnum.DbActionEnum.Create);
                    var _category = await _categoryRepository.AddAsync(category);
                    if (_category != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }
        /// <summary>
        /// カテゴリを更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns>カテゴリを ID で更新</returns>
        public ActionResult Edit(int id)
        {
            orderCount();
            var _categories = _categoryRepository.Get(id);
            return View(_categories);
        }
        /// <summary>
        /// カテゴリ データを更新してカテゴリ インデックス ページに送信する
        /// </summary>
        /// <param name="category"></param>
        /// <returns>カテゴリを ID で更新</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    category = await _userManager.AddUserAndTimestamp(category, User, DbEnum.DbActionEnum.Update);
                    var _category = await _categoryRepository.UpdateAsync(category);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View(category);
        }
        /// <summary>
        /// カテゴリ ID で削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns>カテゴリを削除してインデックス ページに返す</returns>
        public async Task<ActionResult> Delete(int id)
        {
            orderCount();
            var _category = await _categoryRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }


        // for topbar image
        public void TopBarInfo()
        {
            var userList = _context.ApplicationUsers.Where(x => x.UserName == User.Identity.Name).ToList();
            ViewBag.userName = userList[0].Name;
            ViewBag.userImgPath = userList[0].ImgPath;
        }

    }
}
