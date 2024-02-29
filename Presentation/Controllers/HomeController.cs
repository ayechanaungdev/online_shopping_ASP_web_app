using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Core.Repositories;
using InfraStructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Entities;
using InfraStructure.Enums;
using InfraStructure.Repositories;
using Microsoft.AspNetCore.Identity;
using InfraStructure.Helpers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Presentation.Enums;

using Microsoft.Extensions.Options;

namespace Presentation.Controllers
{
    /// <概要>
    /// ユーザホームページ
    /// </概要>
    public class HomeController : Controller
    {
        #region <<定義>>
        #region <<読み取り専用>>

        /// <summary>_コンテクスト</summary>
        private readonly ApplicationDbContext _context;
        /// <summary>_商品リポジトリ</summary>
        private readonly IProductRepository _productRepository;
        /// <summary>_種類リポジトリ</summary>
        private readonly ICategoryRepository _categoryRepository;
        /// <summary>_注文リポジトリ</summary>
        private readonly IOrderRepository _orderRepository;
        /// <summary>_注文詳細リポジトリ</summary>
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        /// <summary>ユーザーマネージャー</summary>
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>項目リスト </summary>
        public static List<AddToCartViewModel> _items = null;
        /// <summary> 種類リスト </summary>
        public static List<Category> categories = null;
        /// <summary>合計金額</summary>
        public static int _totalPrice = 0;
        /// <summary>合計数量</summary>
        public static int _totalQuantity = 0;
        #endregion
        #endregion

        #region <<コンストラクタ>>
        /// <summary>
        /// ホームページコンストラクタ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="productRepository"></param>
        /// <param name="categoryRepository"></param>
        /// <param name="orderRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="orderDetailsRepository"></param>
        /// <param name="pagination"></param>
        public HomeController(ApplicationDbContext context, IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, IOrderDetailsRepository orderDetailsRepository, IOptions<Pagination> pagination)
        {
            _context = context;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _orderDetailsRepository = orderDetailsRepository;

        }
        #endregion

        /// <summary>
        /// 注文リスト
        /// </summary>
        public void orderList()
        {
            ViewBag.data = _items;
            ViewBag.totalPrice = _totalPrice;
            ViewBag.totalQuantity = _totalQuantity;
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        #region<初期表示>
        public void Initialize()
        {
            var category = _context.Categories.Where(x => x.IsDelete == false).ToList();
            ViewData["Categories"] = new SelectList(category, "Id", "Name");
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="page"></param>
        /// <returns>View機能</returns>
        #endregion
        public IActionResult Index(int? page = 1)
        {
            Initialize();
            return View(_productRepository.GetAll());

        }
        /// <summary>
        /// 詳細
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>商品</returns>
        public IActionResult Detail(int productId)
        {
            var product = _productRepository.Get(productId);
            return View(product);
        }
        /// <summary>
        /// 種類
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>商品</returns>
        #region<カテゴリから商品を探す>
        public IActionResult Category(int? Id)
        {
            ViewBag.categories = _categoryRepository.GetAll();
            if (Id == null)
            {
                return View(_productRepository.GetAll());
            }
            var products = _categoryRepository.GetProductByCategoryID(Id);
            return View(products);
        }
        /// <summary>
        /// 接触
        /// </summary>
        /// <returns>View機能</returns>
        #endregion
        public IActionResult Contact()
        {
            return View();
        }
        /// <summary>
        /// エラー
        /// </summary>
        /// <returns>エラーメッセージ</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /// <summary>
        /// 注文した商品リスト
        /// </summary>
        /// <param name="items"></param>
        /// <returns>item count</returns>
        public IActionResult CheckOutProductList(List<AddToCartViewModel> items)
        {
            int totalPrice = 0;
            int totalQuantity = 0;
            if (items.Count != 0)
            {
                foreach (var item in items)
                {
                    var data = _productRepository.GetAllByProductId(item.id);
                    if (data != null)
                    {
                        item.ProductName = data.ProductName;
                        item.Price = data.Price;
                        item.UnitPrice = data.Price * item.quantity;
                        totalPrice += item.UnitPrice;
                        totalQuantity += item.quantity;
                    }
                }
                _items = items;
                _totalPrice = totalPrice;
                _totalQuantity = totalQuantity;
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        /// <summary>
        /// 注文リスト
        /// </summary>
        /// <returns>View機能</returns>
        public IActionResult CheckOut()
        {
            orderList();
            return View();
        }
        /// <summary>
        /// 注文書
        /// </summary>
        /// <param name="order"></param>
        /// <returns>注文リスト</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CheckOut(Order order)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        order.TotalPrice = _totalPrice;
                        order = await _userManager.AddUserAndTimestamp(order, User, DbEnum.DbActionEnum.Create);
                        var _order = await _orderRepository.AddAsync(order);

                        if (_items != null)
                        {
                            foreach (var item in _items)
                            {
                                OrderDetails details = new OrderDetails();
                                details = await _userManager.AddUserAndTimestamp(details, User, DbEnum.DbActionEnum.Create);
                                details.OrderId = _order.Id;
                                details.ProductId = item.id;
                                details.Qty = item.quantity;
                                details.QtyPrice = item.UnitPrice;
                                var _details = await _orderDetailsRepository.AddAsync(details);
                            }
                        }
                        await transaction.CommitAsync();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                    }
                    _items.Clear();
                    _totalPrice = 0;
                    _totalQuantity = 0;
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                    return RedirectToAction(nameof(Index));

                }
            }
            orderList();
            return View(order);
        }
    }
}
