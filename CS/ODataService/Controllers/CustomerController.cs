using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataService.Helpers;
using ODataService.Models;

namespace ODataService.Controllers {
    public class CustomerController : ODataController {

        private UnitOfWork Session;
        public CustomerController(UnitOfWork uow) {
            this.Session = uow;
        }

        [EnableQuery]
        public IQueryable<Customer> Get() {
            return Session.Query<Customer>();
        }

        [EnableQuery]
        public SingleResult<Customer> Get([FromODataUri] string key) {
            var result = Session.Query<Customer>().Where(t => t.CustomerID == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public IQueryable<Order> GetOrders([FromODataUri] string key) {
            return Session.Query<Customer>().Where(m => m.CustomerID == key).SelectMany(m => m.Orders);
        }


        [HttpPost]
        public IActionResult Post([FromBody] ChangesSet<Customer> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var customer = new Customer(Session);
            changesSet.Put(customer);
            Session.CommitChanges();
            return Created(customer);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] string key, [FromBody] ChangesSet<Customer> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Customer customer = Session.GetObjectByKey<Customer>(key);
            if(customer == null) {
                customer = new Customer(Session);
                changesSet.Put(customer);
                Session.CommitChanges();
                return Created(customer);
            } else {
                changesSet.Put(customer);
                Session.CommitChanges();
                return Updated(changesSet);
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] string key, [FromBody] ChangesSet<Customer> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Customer customer = Session.GetObjectByKey<Customer>(key);
            if(customer == null) {
                return NotFound();
            }
            changesSet.Patch(customer);
            Session.CommitChanges();
            return Updated(customer);
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] string key) {
            return StatusCode(ApiHelper.Delete<Customer, string>(key, Session));
        }

        [HttpPost, HttpPut]
        public IActionResult CreateRef([FromODataUri]string key, string navigationProperty, [FromBody] Uri link) {
            return StatusCode(ApiHelper.CreateRef<Customer, string>(Request, key, navigationProperty, link, Session));
        }

        [HttpDelete]
        public IActionResult DeleteRef([FromODataUri] string key, [FromODataUri] int relatedKey, string navigationProperty) {
            return StatusCode(ApiHelper.DeleteRef<Customer, string, int>(key, relatedKey, navigationProperty, Session));
        }
    }
}