using Core.Entities;
using Core.Repositories;
using InfraStructure.Data;
using InfraStructure.Enums;
using InfraStructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    /// 注文リストチェック
    /// </概要>
    [Authorize]
    public class OrderController : Controller
    {
        #region <<定義>>
        #region <<読み取り専用>>

        /// <summary>_コンテクスト</summary>
        private readonly ApplicationDbContext _context;
        /// <summary>_注文リポジトリ</summary>
        private readonly IOrderRepository _orderRepository;
        /// <summary>_注文詳細リポジトリ</summary>
        private readonly IOrderDetailsRepository _orderdetailRepository;
        /// <summary>_ページネーション</summary>
        private readonly Pagination _pagination;

        #endregion
        #endregion

        #region <<コンストラクタ>>

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="orderRepository"></param>
        /// <param name="orderdetailRepository"></param>
        /// <param name="pagination"></param>
        public OrderController(ApplicationDbContext context, IOrderRepository orderRepository,IOrderDetailsRepository orderdetailRepository, IOptions<Pagination> pagination)
        {
            _context = context;
            _orderRepository = orderRepository;
            _orderdetailRepository = orderdetailRepository;
            _pagination = pagination.Value;
        }
        #endregion

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
        /// インデックスページ
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Email"></param>
        /// <param name="IsDeliver"></param>
        /// <returns>注文リスト、ページ番号、ページサイズを返す</returns>
        public IActionResult Index(int? page = 1, string Email = null,int IsDeliver = 0)
        {
            orderCount();
            TopBarInfo();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize; 
            var IsDeliverstr = "";

            ViewBag.Deliver = new SelectList(GetStatus(),"Id","Name",IsDeliver);

            IsDeliverstr = IsDeliver.ToString();

            if (Email == null)
            {
                if (IsDeliverstr.Equals("0"))
                {
                    var EmailDelivers = _orderRepository.GetAll();
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
                else if (IsDeliverstr.Equals("1"))
                {
                    var EmailDelivers = _orderRepository.GerOrderByDeliver(IsDeliverstr);
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
                else
                {
                    var EmailDelivers = _orderRepository.GerOrderByDeliverPending(IsDeliverstr);
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
            }
            else
            {
                if (IsDeliverstr.Equals("0"))
                {
                    var EmailDelivers = _orderRepository.GetOrderByUserEmailandAllDeliverStatus(Email, IsDeliverstr);
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
                else if (IsDeliverstr.Equals("1"))
                {
                    var EmailDelivers = _orderRepository.GetOrderByUserEmailandDeliver(Email,IsDeliverstr);
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
                else
                {
                    var EmailDelivers = _orderRepository.GetOrderByUserEmailandDeliverPending(Email, IsDeliverstr);
                    return View(EmailDelivers.ToPagedList((int)page, pageSize));
                }
            }
        }

        /// <summary>
        /// 配送状況を返す
        /// </summary>
        /// <returns>配送ステータスオブジェクトリストを返す</returns>
        private List<DeliverStatus> GetStatus()
        {
            return new List<DeliverStatus>()
            {
                new DeliverStatus() { Id = 1, Name = "Delivered" },
                new DeliverStatus() { Id = 2, Name = "Pending" },
            };
        }
        /// <summary>
        /// 注文の詳細を返す
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns>注文IDで注文の詳細, ページ番号、ページサイズを返す</returns>
        public ActionResult OrderDetail(int id, int? page = 1)
        {
            orderCount();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            var orderDetails = _orderdetailRepository.GetOrderDetailByOrderID(id);
            return View(orderDetails.ToPagedList((int)page, pageSize));
        }
        /// <summary>
        /// 配達
        /// </summary>
        /// <param name="id"></param>
        /// <returns>配信をインデックスページに返す</returns>
        public async Task<IActionResult> Deliver(int id)
        {
            orderCount();
            try
            {
                var _deliver = await _orderRepository.DeliverAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Deliver;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// 注文IDによる注文詳細の削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns>注文詳細を削除後、注文詳細に返す</returns>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _prodcut = await _orderdetailRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return RedirectToAction(nameof(OrderDetail));
        }
        /// <summary>
        /// 注文を削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns>注文を削除した後、インデックス ページに戻る</returns>
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var _prodcut = await _orderRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }

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

