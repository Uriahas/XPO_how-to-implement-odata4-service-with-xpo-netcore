using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataService.Helpers;
using ODataService.Models;

namespace ODataService.Controllers {
    public class OrderDetailController : ODataController {

        private UnitOfWork Session;
        public OrderDetailController(UnitOfWork uow) {
            this.Session = uow;
        }

        [EnableQuery]
        public IQueryable<OrderDetail> Get() {
            return Session.Query<OrderDetail>();
        }

        [EnableQuery]
        public SingleResult<OrderDetail> Get([FromODataUri] int key) {
            var result = Session.Query<OrderDetail>().Where(t => t.OrderDetailID == key);
            return SingleResult.Create(result);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] ChangesSet<OrderDetail> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            OrderDetail orderDetail = Session.GetObjectByKey<OrderDetail>(key);
            if(orderDetail == null) {
                orderDetail = new OrderDetail(Session);
                changes.Put(orderDetail);
                Session.CommitChanges();
                return Created(orderDetail);
            } else {
                changes.Put(orderDetail);
                Session.CommitChanges();
                return Updated(orderDetail);
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] int key, Delta<OrderDetail> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var orderDetail = Session.GetObjectByKey<OrderDetail>(key);
            if(orderDetail == null) {
                return NotFound();
            }
            changes.Patch(orderDetail);
            Session.CommitChanges();
            return Updated(orderDetail);
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key) {
            return StatusCode(ApiHelper.Delete<OrderDetail, int>(key, Session));
        }
    }
}