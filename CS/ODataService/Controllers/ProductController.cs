using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataService.Helpers;
using ODataService.Models;

namespace ODataService.Controllers {
    public class ProductController : ODataController {

        private UnitOfWork Session;
        public ProductController(UnitOfWork uow) {
            this.Session = uow;
        }

        [EnableQuery]
        public IQueryable<Product> Get() {
            return Session.Query<Product>();
        }

        [EnableQuery]
        public SingleResult<Product> Get([FromODataUri] int key) {
            var result = Session.Query<Product>().Where(t => t.ProductID == key);
            return SingleResult.Create(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ChangesSet<Product> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var product = new Product(Session);
            changes.Put(product);
            Session.CommitChanges();
            return Created(product);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] ChangesSet<Product> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Product product = Session.GetObjectByKey<Product>(key);
            if(product == null) {
                product = new Product(Session);
                changes.Put(product);
                Session.CommitChanges();
                return Created(product);
            } else {
                changes.Put(product);
                Session.CommitChanges();
                return Updated(product);
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] int key, ChangesSet<Product> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var product = Session.GetObjectByKey<Product>(key);
            if(product == null) {
                return NotFound();
            }
            changes.Patch(product);
            Session.CommitChanges();
            return Updated(product);
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key) {
            return StatusCode(ApiHelper.Delete<Product, int>(key, Session));
        }

        [HttpPost, HttpPut]
        public IActionResult CreateRef([FromODataUri]int key, string navigationProperty, [FromBody] Uri link) {
            return StatusCode(ApiHelper.CreateRef<Product, int>(Request, key, navigationProperty, link, Session));
        }

        [HttpDelete]
        public IActionResult DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            return StatusCode(ApiHelper.DeleteRef<Product, int, int>(key, relatedKey, navigationProperty, Session));
        }
    }
}