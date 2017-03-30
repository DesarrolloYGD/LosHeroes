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
using System.Data.Entity.Validation;

namespace BancoEstadoBodega.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
    {

        private LosHeroesEntities db = new LosHeroesEntities();
        // GET: Administracion  
        public ActionResult Index()
        {
            var productos = db.PRODUCTO.Include(m => m.IDClienteFK);
            return View();
        }

        //Convertir imagen
        public ActionResult convertirImagen(string codigo)
        {
            try
            {
                var imagenProducto = db.PRODUCTO.Where(x => x.Codigo == codigo).FirstOrDefault();

                return File(imagenProducto.imagenProducto, "image/jpeg");
            }
            catch (Exception ex)
            { }
            return null;
        }


        // GET: PRODUCTO/Create
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
        public ActionResult Create([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,CostoUnid,IDCategoriaFK,TotalCajas,TotalSueltas,FechaVencimiento,IDClienteFK,UrlImagen")] PRODUCTO pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            if (imagenProducto != null && imagenProducto.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imagenProducto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imagenProducto.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                pRODUCTO.imagenProducto = imageData;
            }

            if (ModelState.IsValid)
            {
                db.PRODUCTO.Add(pRODUCTO);
                db.SaveChanges();
                return RedirectToAction("Producto");
            }

            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }



        #region CATEGORIA
        public ActionResult Categoria()
        {
            return View(/*db.CATEGORIA.ToList()*/);
        }

        [HttpPost]
        public ActionResult AgregarCategoria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ModificarCategoria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EliminarCategoria()
        {
            return View();
        }

        #endregion

        #region PRODUCTO
        public ActionResult Producto(int? IDEmpresa, int? IDCategoria)
        {
            //int codigo = EnRol();

            ViewBag.IDEmpresa = new SelectList(db.CLIENTE.ToList(), "IDCliente", "Alias", IDEmpresa);
            ViewBag.IDCategoria = new SelectList(db.CATEGORIA.ToList(), "IDCategoria", "Nombre", IDCategoria);
            ViewBag.IDCategoriaFK = ViewBag.IDCategoria;
            ViewBag.IDClienteFK = ViewBag.IDEmpresa;

            List<PRODUCTO> lista = db.PRODUCTO.ToList();
            if (IDEmpresa != null)
            {
                lista = lista.Where(r => r.IDClienteFK == IDEmpresa).ToList();
            }
            if (IDCategoria != null)
            {
                lista = lista.Where(r => r.IDCategoriaFK == IDCategoria).ToList();
            }

            if (User.IsInRole("Vista"))
            {
                lista = lista.Where(r => r.CantidadTotal != 0).ToList();

            }




            return View(lista);
        }




        // Funcion que agrega los productos con imagen (ventana flotante) 
        public ActionResult AgregarProducto([Bind(Include = "IDProducto,Descripcion,Codigo,Nombre,stock_ideal,CostoUnid,Posicion,FechaVencimiento,IDCategoriaFK,CantidadTotal,pendiente,ProductoConLogo,ProductoSinLogo,PrecioUni,TiempoReposicion,Packing,Vencimiento")]PRODUCTO model, FormCollection collection, HttpPostedFileBase imagenProducto)
        {
            PRODUCTOBODEGA item = new PRODUCTOBODEGA();
            PRODUCTOBODEGA item2 = new PRODUCTOBODEGA();

            //item.BODEGA_IDBodega = 1;
            //model.PRODUCTOBODEGA.Add(item);

            //item2.Sueltas = Convert.ToInt32(collection["Bodega2Cajas"]);
            //item2.Cajas = Convert.ToInt32(collection["Bodega2Sueltas"]);
            //item2.BODEGA_IDBodega = 3;
            //model.PRODUCTOBODEGA.Add(item);
            //model.UrlImagen = model.Codigo + ".jpg";



            if (imagenProducto != null && imagenProducto.ContentLength > 0)
            {
                /*byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imagenProducto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imagenProducto.ContentLength);
                }
                setear la imagen a la entidad que se creara
                model.imagenProducto = imageData;*/

                
                string imgName = model.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
                model.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService


            }

            db.PRODUCTO.Add(model);
            db.SaveChanges();
            return RedirectToAction("Producto");
        }

        //Obtiene los detalles de los productos y los muestra en el view Detalles
        public ViewResult Detalles(int id)
        {
            PRODUCTO producto = db.PRODUCTO.Find(id);
            //if que asigna una imagen por defecto en caso que el campo UrlImagen de la tabla productos sea nulo
            if (producto.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/losheroesblob/" + producto.Codigo + ".jpg";
            }
            return View(producto);
        }

        /*Obtiene la imagen asociada al producto y lo despliega en la grilla
        public ViewResult Producto(int id)
        {
            PRODUCTO producto = db.PRODUCTO.Find(id);
            ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/" + producto.Codigo + ".jpg";
            return ViewBag;
        }*/

        // GET: PRODUCTOes/Edit/5
        public ActionResult Editar(int? id)
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
            if (pRODUCTO.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/losheroesblob/" + pRODUCTO.Codigo + ".jpg";
            }

            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,StockQl,CantidadTotal,TotalCajas,TotalSueltas,SueltasQL,StockDÑ,SueltasDÑ,CostoUnid,Posicion,FechaVencimiento,stock_ideal,IDCategoriaFK,IDClienteFK,ProductoConLogo,ProductoSinLogo,pendiente,Descripcion,PrecioUni,TiempoReposicion,Packing,Vencimiento,Obs")] PRODUCTO pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            string imgName = pRODUCTO.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
            if (pRODUCTO.UrlImagen == null)
            {
                pRODUCTO.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
            }


            if (ModelState.IsValid)
            {
                db.Entry(pRODUCTO).State = EntityState.Modified;
                //pRODUCTO.TotalSueltas = pRODUCTO.SueltasDÑ.Value + pRODUCTO.SueltasQL.Value;
                //pRODUCTO.TotalCajas = pRODUCTO.StockDÑ.Value + pRODUCTO.StockQl.Value;
                //pRODUCTO.CantidadTotal = (pRODUCTO.TotalCajas * pRODUCTO.UnidadesXCaja) + pRODUCTO.TotalSueltas;

                db.SaveChanges();
                return RedirectToAction("Producto");
            }
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        public ActionResult EditarImagen(int? id)
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
            if (pRODUCTO.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/losheroesblob/" + pRODUCTO.UrlImagen;
            }
            return View(pRODUCTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarImagen(EditarImagenProductoViewModel pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            if (ModelState.IsValid)
            {
                var dbProd = db.PRODUCTO.FirstOrDefault(p => p.IDProducto == pRODUCTO.IdProducto);
                if (dbProd == null)
                {
                    return HttpNotFound();
                }
                string imgName = dbProd.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
                if (pRODUCTO.UrlImagen == null)
                {
                    pRODUCTO.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                    new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }
                else
                {
                    new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }

                dbProd.UrlImagen = pRODUCTO.UrlImagen;
                db.SaveChanges();

                return RedirectToAction("PRODUCTO", "Administracion");
            }
            return View(pRODUCTO);
        }





        //     [HttpPost]
        //    public ActionResult ModificarProducto()
        //     {
        //          return View();
        //     }

        [HttpPost]
        public ActionResult EliminarProducto()
        {
            return View();
        }



        #endregion

        #region AJAX
        [ActionName("DatosProducto")]
        public ActionResult DatosProductos(int IDProducto)
        {
            var producto = db.PRODUCTO.Find(IDProducto);

            var item = new
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                UnidadesXCajas = producto.UnidadesXCaja,
                StockQuilicura = producto.StockQl,
                StockDardignac = producto.StockDÑ,
                CantidadTotal = producto.CantidadTotal,
                TotalCajas = producto.TotalCajas,
                TotalSueltas = producto.TotalSueltas,
                FechaVencimiento = producto.FechaVencimiento,
                Imagen = producto.UrlImagen,
                Categoria = producto.CATEGORIA,
                Alias = producto.CLIENTE
            };
            return this.Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

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