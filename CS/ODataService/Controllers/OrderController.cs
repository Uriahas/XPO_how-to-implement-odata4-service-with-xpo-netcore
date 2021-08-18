using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataService.Helpers;
using ODataService.Models;

namespace ODataService.Controllers {
    public class OrderController : ODataController {

        private UnitOfWork Session;
        public OrderController(UnitOfWork uow) {
            this.Session = uow;
        }

        [EnableQuery]
        public IQueryable<Order> Get() {
            return Session.Query<Order>();
        }

        [EnableQuery]
        public SingleResult<Order> Get([FromODataUri] int key) {
            var result = Session.Query<Order>().Where(t => t.ID == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key) {
            var result = Session.Query<Order>().Where(m => m.ID == key).Select(m => m.Customer);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public IQueryable<OrderDetail> GetOrderDetails([FromODataUri] int key) {
            return Session.Query<OrderDetail>().Where(t => t.Order.ID == key);
        }

        [EnableQuery]
        public SingleResult<BaseDocument> GetParentDocument([FromODataUri] int key) {
            var result = Session.Query<Order>().Where(m => m.ID == key).Select(m => m.ParentDocument);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public IQueryable<BaseDocument> GetLinkedDocuments([FromODataUri] int key) {
            return Session.Query<Order>().Where(m => m.ID == key).SelectMany(t => t.LinkedDocuments);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ChangesSet<Order> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var order = new Order(Session);
            changesSet.Put(order);
            Session.CommitChanges();
            return Created(order);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] ChangesSet<Order> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Order order = Session.GetObjectByKey<Order>(key);
            if(order == null) {
                order = new Order(Session);
                changesSet.Put(order);
                Session.CommitChanges();
                return Created(order);
            } else {
                changesSet.Put(order);
                Session.CommitChanges();
                return Updated(order);
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] int key, ChangesSet<Order> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Order order = Session.GetObjectByKey<Order>(key);
            if(order == null) {
                return NotFound();
            }
            changesSet.Patch(order);
            Session.CommitChanges();
            return Updated(order);
        }

        [HttpPost]
        [HttpPut]
        [ODataRoute("Order({key})/OrderDetails")]
        public IActionResult AddToOrderDetails([FromODataUri] int key, ChangesSet<OrderDetail> changes) {
            Order order = Session.GetObjectByKey<Order>(key);
            if(order == null) {
                return NotFound();
            }
            object orderDetailId;
            changes.TryGetPropertyValue(nameof(OrderDetail.OrderDetailID), out orderDetailId);
            OrderDetail orderDetail = order.OrderDetails.FirstOrDefault(d => d.OrderDetailID == (int)orderDetailId);
            if(orderDetail == null) {
                orderDetail = new OrderDetail(Session);
                changes.Put(orderDetail);
                order.OrderDetails.Add(orderDetail);
                Session.CommitChanges();
                return Created(orderDetail);
            } else {
                changes.Put(orderDetail);
                Session.CommitChanges();
                return Updated(orderDetail);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key) {
            return StatusCode(ApiHelper.Delete<Order, int>(key, Session));
        }

        [HttpPost, HttpPut]
        public IActionResult CreateRef([FromODataUri]int key, string navigationProperty, [FromBody] Uri link) {
            return StatusCode(ApiHelper.CreateRef<Order, int>(Request, key, navigationProperty, link, Session));
        }

        [HttpDelete]
        public IActionResult DeleteRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            return StatusCode(ApiHelper.DeleteRef<Order, int>(Request, key, navigationProperty, link, Session));
        }

        [HttpDelete]
        public IActionResult DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            return StatusCode(ApiHelper.DeleteRef<Order, int, int>(key, relatedKey, navigationProperty, Session));
        }
    }
}