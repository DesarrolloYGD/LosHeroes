using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BancoEstadoBodega.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Entity;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage type
using System.Net;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace BancoEstadoBodega.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private LosHeroesEntities db = new LosHeroesEntities();
        // GET: Reporte
        public ActionResult Index()
        {
            var productos = db.PRODUCTO.Include(m => m.IDClienteFK);
            return View();
        }

        public ActionResult Critico(int? IDEmpresa, int? IDCategoria, int?  Stock,int? StockCritico, PRODUCTO model)
        {
            int cod = EnRol();
            ViewBag.IDEmpresa = new SelectList(db.CLIENTE.ToList(), "IDCliente", "Alias", IDEmpresa);
            ViewBag.IDCategoria = new SelectList(db.CATEGORIA.ToList(), "IDCategoria", "Nombre", IDCategoria);
            ViewBag.IDCategoriaFK = ViewBag.IDCategoria;
            ViewBag.IDClienteFK = ViewBag.IDEmpresa;
            // var productos = db.PRODUCTO.Include(m => m.stock_ideal);
            //var stock2 = db.PRODUCTO.Include(m => m.CantidadTotal);
            //int porcentje = (stock2 * 100) / productos;
            //var porcentaje = (model.CantidadTotal.Value * 100) / model.stock_ideal;
            List<PRODUCTO> lista = db.PRODUCTO.ToList();
            lista = lista.Where(r => r.CantidadTotal.Value * 100 / r.stock_ideal.Value <= 20).ToList();
            if (!User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.IDClienteFK == cod).ToList();
            }
            //lista = lista.Where(r => r.CantidadTotal.Value *100 / r.stock_ideal.Value<=20).ToList();
            return View(lista);
        }
        // GET: Reporte/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reporte/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reporte/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reporte/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reporte/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reporte/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reporte/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #region Roles
        public int EnRol()
        {
            int codigo = 0;

            if (User.IsInRole("Banca Mayorista"))
            {
                codigo = 1;
                return codigo;
            }
            else if (User.IsInRole("Banca Personas"))
            {
                codigo = 2;
                return codigo;
            }
            else if (User.IsInRole("Caja Vecina"))
            {
                codigo = 3;
                return codigo;
            }
            else if (User.IsInRole("Capital de Riesgo"))
            {
                codigo = 4;
                return codigo;
            }
            else if (User.IsInRole("Cliente Calidad"))
            {
                codigo = 5;
                return codigo;
            }
            else if (User.IsInRole("Comunicaciones"))
            {
                codigo = 6;
                return codigo;
            }
            else if (User.IsInRole("Convenio"))
            {
                codigo = 7;
                return codigo;
            }
            else if (User.IsInRole("Desayuno Cliente"))
            {
                codigo = 8;
                return codigo;
            }
            else if (User.IsInRole("Educacion Financiera"))
            {
                codigo = 9;
                return codigo;
            }
            else if (User.IsInRole("Fondos Mutuos"))
            {
                codigo = 10;
                return codigo;
            }
            else if (User.IsInRole("Gerencia de Ventas"))
            {
                codigo = 11;
                return codigo;
            }
            else if (User.IsInRole("Hipotecario"))
            {
                codigo = 12;
                return codigo;
            }
            else if (User.IsInRole("Inversiones"))
            {
                codigo = 13;
                return codigo;
            }
            else if (User.IsInRole("Marketing"))
            {
                codigo = 14;
                return codigo;
            }
            else if (User.IsInRole("Negocios Internacionales"))
            {
                codigo = 15;
                return codigo;
            }
            else if (User.IsInRole("Pequeña Empresa"))
            {
                codigo = 16;
                return codigo;
            }
            else if (User.IsInRole("Mesa de Dinero"))
            {
                codigo = 17;
                return codigo;
            }
            else if (User.IsInRole("RRHH"))
            {
                codigo = 18;
                return codigo;
            }
            else if (User.IsInRole("Serviestado"))
            {
                codigo = 19;
                return codigo;
            }
            else if (User.IsInRole("Sucursales"))
            {
                codigo = 20;
                return codigo;
            }
            else if (User.IsInRole("Transito"))
            {
                codigo = 21;
                return codigo;
            }


            else { return codigo; }
        }
        #endregion
    }
}
