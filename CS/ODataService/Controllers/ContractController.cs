using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataService.Helpers;
using ODataService.Models;

namespace ODataService.Controllers {
    public class ContractController : ODataController {
        private UnitOfWork Session;
        public ContractController(UnitOfWork uow) {
            this.Session = uow;
        }

        [EnableQuery]
        public IQueryable<Contract> Get() {
            return Session.Query<Contract>();
        }

        [EnableQuery]
        public SingleResult<Contract> Get([FromODataUri] int key) {
            var result = Session.Query<Contract>().Where(t => t.ID == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key) {
            var result = Session.Query<Contract>().Where(m => m.ID == key).Select(m => m.Customer);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public SingleResult<BaseDocument> GetParentDocument([FromODataUri] int key) {
            var result = Session.Query<Contract>().Where(m => m.ID == key).Select(m => m.ParentDocument);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public IQueryable<BaseDocument> GetLinkedDocuments([FromODataUri] int key) {
            return Session.Query<Contract>().Where(m => m.ID == key).SelectMany(t => t.LinkedDocuments);
        }


        [HttpPost]
        public IActionResult Post([FromBody] ChangesSet<Contract> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Contract contract = new Contract(Session);
            changes.Put(contract);
            Session.CommitChanges();
            return Created(contract);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] ChangesSet<Contract> changes) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            Contract contract = Session.GetObjectByKey<Contract>(key);
            if(contract == null) {
                contract = new Contract(Session);
                changes.Put(contract);
                Session.CommitChanges();
                return Created(contract);
            } else {
                changes.Put(contract);
                Session.CommitChanges();
                return Updated(contract);
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] int key, ChangesSet<Contract> changesSet) {
            if(!ModelState.IsValid) {
                return BadRequest();
            }
            var contract = Session.GetObjectByKey<Contract>(key);
            if(contract == null) {
                return NotFound();
            }
            changesSet.Patch(contract);
            Session.CommitChanges();
            return Updated(contract);
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key) {
            return StatusCode(ApiHelper.Delete<Contract, int>(key, Session));
        }

        [HttpPost, HttpPut]
        public IActionResult CreateRef([FromODataUri]int key, string navigationProperty, [FromBody] Uri link) {
            return StatusCode(ApiHelper.CreateRef<Contract, int>(Request, key, navigationProperty, link, Session));
        }

        [HttpDelete]
        public IActionResult DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            return StatusCode(ApiHelper.DeleteRef<Contract, int, int>(key, relatedKey, navigationProperty, Session));
        }
    }
}