using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BancoEstadoBodega.Models;

namespace BancoEstadoBodega.Controllers
{
    public class PRODUCTOesController : Controller
    {
        private LosHeroesEntities db = new LosHeroesEntities();

        // GET: PRODUCTOes
        public ActionResult Index()
        {
            var pRODUCTO = db.PRODUCTO.Include(p => p.CATEGORIA).Include(p => p.CLIENTE);
            return View(pRODUCTO.ToList());
        }

        // GET: PRODUCTOes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCTO);
        }

        // GET: PRODUCTOes/Create
        public ActionResult Create()
        {
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre");
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias");
            return View();
        }

        // POST: PRODUCTOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,CostoUnid,IDCategoriaFK,TotalCajas,TotalSueltas,FechaVencimiento,IDClienteFK,UrlImagen")] PRODUCTO pRODUCTO)
        {
            if (ModelState.IsValid)
            {
                db.PRODUCTO.Add(pRODUCTO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        // GET: PRODUCTOes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,CostoUnid,IDCategoriaFK,TotalCajas,TotalSueltas,FechaVencimiento,IDClienteFK,UrlImagen")] PRODUCTO pRODUCTO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pRODUCTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Producto");
            }
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        // GET: PRODUCTOes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            db.PRODUCTO.Remove(pRODUCTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
