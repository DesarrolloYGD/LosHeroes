﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BancoEstadoBodega.Models;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BancoEstadoBodega.Controllers
{
    [Authorize]
    public class SolicitudController : Controller
    {
        private LosHeroesEntities1 db = new LosHeroesEntities1();


        // GET: Solicitud
        public ActionResult Index()
        {

            var Solicitud = db.SolicitudPedido.Include(s => s.idSolicitud);
            return View();

        }

        //Obtiene los detalles de los productos y los muestra en el view Detalles
        public ViewResult Detalles(int id)
        {
            SolicitudPedido sp = db.SolicitudPedido.Find(id);
            return View(sp);
        }

        public ViewResult DetalleBodega(int id)
        {
            SolicitudPedido sp = db.SolicitudPedido.Find(id);
            return View(sp);
        }


        // Funcion que agrega los productos con imagen (ventana flotante) 
        //public ActionResult CrearSolicitud(int IdProducto)
        //{
        //    var details = db.PRODUCTO;
        //    //Hard coded for demo. You may replace it with data from db.
        //    return Json(details, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult CrearSolicitud2()
        {
            var model = new SolicitudViewModel();
            /*if (!User.IsInRole("administradores"))
            {
                int cod = EnRol();
                model.PRODUCTO = db.PRODUCTO.Where(p => p.IDClienteFK == cod).OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.IDProducto.ToString(), Text = p.Nombre }).ToList();
            }
            else
            {
                model.PRODUCTO = db.PRODUCTO.OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.IDProducto.ToString(), Text = p.Nombre }).ToList();
            }*/
            model.PRODUCTO = db.PRODUCTO.OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.IDProducto.ToString(), Text = p.Nombre }).ToList();
            model.UserSoloVista = db.UserSoloVista.OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.Nombre.ToString(), Text = p.Nombre }).ToList();
            return View(model);
        }
        [HttpPost]

        public ActionResult CrearSolicitud2(SolicitudViewModel objeto)
        {
            try
            {
                string destino = " ";
                if (objeto.detalleDestino != null)
                {
                    destino = objeto.DestinoSeleccionado + " " + objeto.detalleDestino;
                }
                else
                {
                    destino = objeto.DestinoSeleccionado;
                }


                var Solicitud = new SolicitudPedido
                {
                    descripcion = objeto.numeroOC,
                    idTipoDespacho = 1,
                    destino = destino,
                    fechaSolicitud = DateTime.Now,
                    usuarioMandante = objeto.Comprador,
                    usuarioReceptor = objeto.Solicitante,
                    estado = "Solicitada",
                    correo = User.Identity.GetUserName(),
                    cod_estado = 1
                };

                db.SolicitudPedido.Add(Solicitud);
                db.SaveChanges();
                var idSol = Solicitud.idSolicitud;

                var lista = objeto.productosSeleccionados.ToList();

            
                if (lista.Count > 0)
                {
                    foreach (var item in lista)
                    {
                        var ProductoSolicitud = new ProductoSolicitud
                        {
                            idSolicitud = idSol,
                            idProducto = item.idProducto,
                            cantidad = item.cantidad,
                            NombreFK = item.NombreFK
                        };
                        var ProductoEnBodega = new ProductosEnBodega
                        {
                            idSolicitud = idSol,
                            idProducto = item.idProducto,
                            cantidad = item.cantidad,
                            NombreFK = item.NombreFK
                        };
                        db.ProductoSolicitud.Add(ProductoSolicitud);
                        db.ProductosEnBodega.Add(ProductoEnBodega);
                        db.SaveChanges();
                        PRODUCTO producto = db.PRODUCTO.Find(item.idProducto);
                        db.Entry(producto).State = EntityState.Modified;
                        var stock = producto.CantidadTotal;
                        producto.CantidadTotal = stock - item.cantidad;
                        db.SaveChanges();
                    }

                }
                /*-------------------------MENSAJE DE CORREO----------------------*/

                //Creamos un nuevo Objeto de mensaje
                System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                //if (Request.IsAuthenticated)
                //{
                //mmsg.To.Add(User.Identity.GetUserName());
                //}
                mmsg.To.Add("lbasic@promomas.cl");
                //mmsg.To.Add("marco.molina@probag.cl");

                //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                //Asunto
                mmsg.Subject = "Solicitud recepcionada";
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Direccion de correo electronico que queremos que reciba una copia del mensaje
                mmsg.Bcc.Add("marco.molina@probag.cl");
                //mmsg.Bcc.Add("marco.molina.hidalgo@gmail.cl");

                //if (Request.IsAuthenticated)
                //{
                    //mmsg.Bcc.Add(User.Identity.GetUserName());
                //}

                mmsg.To.Add("lbasic@promomas.cl");
                mmsg.Bcc.Add("yessyca@probag.cl");
                mmsg.Bcc.Add("facturacion@promomas.cl");
                //mmsg.To.Add("marco.molina@probag.cl");
                //if (Request.IsAuthenticated)
                //{
                //mmsg.Bcc.Add(User.Identity.GetUserName());
                //}

                //mmsg.Bcc.Add(user); //Opcional;

                //LoginViewModel model3;

                //Cuerpo del Mensaje
                DateTime fecha = DateTime.Now;
                string fech = fecha.ToString("dd/MM/yyyy");
         
                string descripción = "<div><p>Empresa: Los Heroes</p> </br><p>OC: " + Solicitud.descripcion + "</p></ br><p>Comprador: " + Solicitud.usuarioMandante + "</p></ br><p>Solicitante: " + Solicitud.usuarioReceptor + "</p><p>Destino: " + Solicitud.destino + "</p></div>";
                string tabla = "<table align='center' border='0' width='80%'><tr bgcolor='#70bbd9'><th>Código</th><th>Descripción</th><th>Cantidad Solicitada</th></tr>";
                string list = "";
                int largo = lista.Count;
                int contador = 0;
                if (largo > 0)
                {
                    foreach (var item in lista)
                    {
                        var productoAux = db.PRODUCTO.Find(item.idProducto);
                        contador = contador + 1;
                        list = list + "<tr bgcolor ='#e8e8e8'><td>" + productoAux.Codigo + "</td><td>" + productoAux.Nombre + "</td><td>" + item.cantidad + "</td></tr>";
                        if (contador == largo)
                        {
                            list = list + "</table>";
                        }
                    }
                }
                string footer = "<p align='center'> para ver más detalles de la solicitud <a href='http://losheroes.micatalogo.cl/Solicitud/Detalles/" + Solicitud.idSolicitud+"'>pinche aquí­</a></p>";
                mmsg.Body = descripción + "</br>" + tabla + list + footer;
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = true; //Si no queremos que se envíe como HTML

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new System.Net.Mail.MailAddress("solicitudes@promomas.cl");

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                //Hay que crear las credenciales del correo emisor
                cliente.Credentials = new System.Net.NetworkCredential("solicitudes@promomas.cl", "probag63");

                //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                /*
                cliente.Port = 587;
                cliente.EnableSsl = true;
                */
                cliente.Host = "mail.promomas.cl"; //Para Gmail "smtp.gmail.com";

                /*-------------------------ENVIO DE CORREO----------------------*/

                try
                {
                    //Enviamos el mensaje      
                    cliente.Send(mmsg);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //Aquí gestionamos los errores al intentar enviar el correo
                }

                return RedirectToAction("SolicitudPedido");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult CrearSolicitud()
        {
            var model = new ProductoViewModel();

            if (!User.IsInRole("administradores"))
            {
                int cod = EnRol();
                model.PRODUCTO = db.PRODUCTO.Where(p => p.IDClienteFK == cod).OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.IDProducto.ToString(), Text = p.Nombre }).ToList();
            }
            else
            {
                model.PRODUCTO = db.PRODUCTO.OrderBy(p => p.Nombre).Select(p => new SelectListItem { Value = p.IDProducto.ToString(), Text = p.Nombre }).ToList();
            }
            



            List<PRODUCTO> list = new List<PRODUCTO>();
            ViewBag.cantidadProducto = list;

            ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
            ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
            ViewBag.idMecanizado = new SelectList(db.Mecanizado.ToList(), "idMeca", "Descripcion");
            return View(model);
        }

        [HttpPost]
        public ActionResult CrearSolicitud([Bind(Include = "idSolicitud,descripcion,idTipoDespacho,destino,idArea,usuarioMandante,usuarioReceptor,observacion,estado,correo")]SolicitudPedido model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //model.estado = "Solicitada";
                    model.fechaSolicitud = DateTime.Now;
                    model.correo = User.Identity.GetUserName();
                    //model2.NombreFK = model2.idProducto;
                    db.SolicitudPedido.Add(model);
                    db.SaveChanges();

                    db.sp_vincularProductosSolicitud();

                    /*-------------------------MENSAJE DE CORREO----------------------*/

                    //Creamos un nuevo Objeto de mensaje
                    System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                    //Direccion de correo electronico a la que queremos enviar el mensaje
                    //if (Request.IsAuthenticated)
                    //{
                    //mmsg.To.Add(User.Identity.GetUserName());
                    //}
                    mmsg.To.Add("logistica@promomas.cl");

                    //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                    //Asunto
                    mmsg.Subject = "Solicitud recepcionada";
                    mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                    //Direccion de correo electronico que queremos que reciba una copia del mensaje
                    if (Request.IsAuthenticated)
                    {
                        mmsg.Bcc.Add(User.Identity.GetUserName());
                    }
                    else
                    {
                        mmsg.To.Add("kubeira@promomas.cl");
                    }


                    //mmsg.Bcc.Add(user); //Opcional;

                    //LoginViewModel model3;

                    //Cuerpo del Mensaje
                    DateTime fecha = DateTime.Now;
                    string fech = fecha.ToString("dd/MM/yyyy");

                    mmsg.Body = "Hemos recibido su pedido numero: " + model.idSolicitud + "\nCon fecha: " + fech + " \nSu pedido será despachado en un plazo de 1 a 2 días hábiles" + "\nPara mas detalles porfavor visitar la página sección solicitudes ";
                    mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                    mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

                    //Correo electronico desde la que enviamos el mensaje
                    mmsg.From = new System.Net.Mail.MailAddress("solicitudes@promomas.cl");

                    /*-------------------------CLIENTE DE CORREO----------------------*/

                    //Creamos un objeto de cliente de correo
                    System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                    //Hay que crear las credenciales del correo emisor
                    cliente.Credentials = new System.Net.NetworkCredential("solicitudes@promomas.cl", "medalla24");

                    //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                    /*
                    cliente.Port = 587;
                    cliente.EnableSsl = true;
                    */
                    cliente.Host = "mail.promomas.cl"; //Para Gmail "smtp.gmail.com";

                    /*-------------------------ENVIO DE CORREO----------------------*/

                    try
                    {
                        //Enviamos el mensaje      
                        cliente.Send(mmsg);
                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        //Aquí gestionamos los errores al intentar enviar el correo
                    }


                    return RedirectToAction("SolicitudPedido");
                }
                catch (DbEntityValidationException ex)
                {
                }
            }
            return View(model);
        

    }

        //public ActionResult AgregarSolicitud([Bind(Include = "idSolicitud,descripcion,idTipoDespacho,destino,idArea,usuarioMandante,usuarioReceptor,observacion,estado,correo")]SolicitudPedido model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //model.estado = "Solicitada";
        //            model.fechaSolicitud = DateTime.Now;
        //            model.correo = User.Identity.GetUserName();
        //            //model2.NombreFK = model2.idProducto;
        //            db.SolicitudPedido.Add(model);
        //            db.SaveChanges();

        //            db.sp_vincularProductosSolicitud();

        //            /*-------------------------MENSAJE DE CORREO----------------------*/

        //            //Creamos un nuevo Objeto de mensaje
        //            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

        //            //Direccion de correo electronico a la que queremos enviar el mensaje
        //            //if (Request.IsAuthenticated)
        //            //{
        //            //mmsg.To.Add(User.Identity.GetUserName());
        //            //}
        //            mmsg.To.Add("logistica@promomas.cl");

        //            //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

        //            //Asunto
        //            mmsg.Subject = "Solicitud recepcionada";
        //            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

        //            //Direccion de correo electronico que queremos que reciba una copia del mensaje
        //            if (Request.IsAuthenticated)
        //            {
        //                mmsg.Bcc.Add(User.Identity.GetUserName());
        //            }
        //            else
        //            {
        //                mmsg.To.Add("kubeira@promomas.cl");
        //            }


        //            //mmsg.Bcc.Add(user); //Opcional;

        //            //LoginViewModel model3;

        //            //Cuerpo del Mensaje
        //            DateTime fecha = DateTime.Now;
        //            string fech = fecha.ToString("dd/MM/yyyy");

        //            mmsg.Body = "Hemos recibido su pedido numero: " + model.idSolicitud + "\nCon fecha: " + fech + " \nSu pedido será despachado en un plazo de 24 a 48 horas hábiles"+"\nPara mas detalles porfavor visitar la página ";
        //            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
        //            mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

        //            //Correo electronico desde la que enviamos el mensaje
        //            mmsg.From = new System.Net.Mail.MailAddress("solicitudes@promomas.cl");

        //            /*-------------------------CLIENTE DE CORREO----------------------*/

        //            //Creamos un objeto de cliente de correo
        //            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

        //            //Hay que crear las credenciales del correo emisor
        //            cliente.Credentials = new System.Net.NetworkCredential("solicitudes@promomas.cl", "medalla24");

        //            //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
        //            /*
        //            cliente.Port = 587;
        //            cliente.EnableSsl = true;
        //            */
        //            cliente.Host = "mail.promomas.cl"; //Para Gmail "smtp.gmail.com";

        //            /*-------------------------ENVIO DE CORREO----------------------*/

        //            try
        //            {
        //                //Enviamos el mensaje      
        //                cliente.Send(mmsg);
        //            }
        //            catch (System.Net.Mail.SmtpException ex)
        //            {
        //                //Aquí gestionamos los errores al intentar enviar el correo
        //            }


        //            return RedirectToAction("SolicitudPedido");
        //        }
        //        catch (DbEntityValidationException ex)
        //        {
        //        }
        //    }
        //    return View(model);
        //}


        public ActionResult SolicitudPedido(int? idSolicitud)
        {
            ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
            ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
            ViewBag.idProducto = new SelectList(db.PRODUCTO.ToList(), "IDProducto", "Nombre");
            //ViewBag.cantidad = ViewBag.IDCategoria;
            //ViewBag.IDClienteFK = ViewBag.IDEmpresa;
            List<SolicitudPedido> lista = db.SolicitudPedido.ToList();

            if (!User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.correo == User.Identity.GetUserName()).ToList();
            }
            

            return View(lista);
        }


        public ActionResult ProductoBodega(int? idSolicitud, SolicitudViewModel sol)
        {
            ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
            ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
            ViewBag.idProducto = new SelectList(db.PRODUCTO.ToList(), "IDProducto", "Nombre");
            //ViewBag.cantidad = ViewBag.IDCategoria;
            //ViewBag.IDClienteFK = ViewBag.IDEmpresa;
            var context = new ApplicationDbContext();
            var username = User.Identity.Name;

            var user = context.Users.SingleOrDefault(u => u.UserName == username);
            string fullName = string.Concat(new string[] { user.Nombre});
            List<SolicitudPedido> lista = db.SolicitudPedido.Where(x => x.ProductosEnBodega.Sum(y => y.cantidad) > 0).ToList();
            if (!User.IsInRole("administradores"))
            {
                    lista = lista.Where(r => r.usuarioReceptor == fullName).ToList();
                
            }


            return View(lista);
        }

        //public ActionResult AsignarProducto(int? idSolicitud, int? idProducto)
        //{
        //    //ViewBag.IDEmpresa = new SelectList(db.CLIENTE.ToList(), "IDCliente", "Alias", IDEmpresa);
        //    //ViewBag.IDCategoria = new SelectList(db.CATEGORIA.ToList(), "IDCategoria", "Nombre", IDCategoria);
        //    //ViewBag.IDCategoriaFK = ViewBag.IDCategoria;
        //    //ViewBag.IDClienteFK = ViewBag.IDEmpresa;
        //    List<SolicitudPedido> lista = db.SolicitudPedido.ToList();
        //    return View(lista);

        //}


        // GET: Solicitud/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Solicitud/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Solicitud/Create
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

        // GET: PRODUCTOes/Edit/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPedido sol = db.SolicitudPedido.Find(id);
            if (sol == null)
            {
                return HttpNotFound();
            }
            ViewBag.idTipoEncomienda = new SelectList(db.TipoEncomienda.ToList(), "idTipoEncomienda", "tipoEncomienda");
            ViewBag.idTipoEmpaque = new SelectList(db.TipoEmpaque.ToList(), "idTipoEmpaque", "descripcion");
            ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
            ViewBag.idTrasladoDespacho = new SelectList(db.TrasladoDespacho.ToList(), "idTrasladoDespacho", "descripcion");
            ViewBag.idTipoPedido = new SelectList(db.TipoPedido.ToList(), "idTipoPedido", "descripcion");
            ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
            ViewBag.idProducto = new SelectList(db.PRODUCTO.ToList(), "IDProducto", "Nombre");
            return View(sol);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "idSolicitud,descripcion,idTipoEncomienda,idTipoEmpaque,origen,idTipoDespacho,idTrasladoDespacho,destino,fechaEntrega,codigoSeguimiento,idTipoPedido,idArea,usuarioMandante,usuarioReceptor,observacion,estado,correo")] SolicitudPedido sol)
        {

            if (ModelState.IsValid)
            {
                db.Entry(sol).State = EntityState.Modified;
                ViewBag.idTipoEncomienda = new SelectList(db.TipoEncomienda.ToList(), "idTipoEncomienda", "tipoEncomienda");
                ViewBag.idTipoEmpaque = new SelectList(db.TipoEmpaque.ToList(), "idTipoEmpaque", "descripcion");
                ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
                ViewBag.idTrasladoDespacho = new SelectList(db.TrasladoDespacho.ToList(), "idTrasladoDespacho", "descripcion");
                ViewBag.idTipoPedido = new SelectList(db.TipoPedido.ToList(), "idTipoPedido", "descripcion");
                ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
                ViewBag.idProducto = new SelectList(db.PRODUCTO.ToList(), "IDProducto", "Nombre");
                db.SaveChanges();

                /*-------------------------MENSAJE DE CORREO----------------------*/

                //Creamos un nuevo Objeto de mensaje
                System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                //if (Request.IsAuthenticated)
                //{
                //mmsg.To.Add(User.Identity.GetUserName());
                //}
                mmsg.To.Add("logistica@promomas.cl");

                //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                //Asunto
                mmsg.Subject = "Solicitud despachada";
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Direccion de correo electronico que queremos que reciba una copia del mensaje
               
                mmsg.Bcc.Add(sol.correo);

                //mmsg.Bcc.Add(user); //Opcional;

                //LoginViewModel model3;

                //Cuerpo del Mensaje
                //DateTime fecha = sol.fechaEntrega;
                //string fech = sol.fechaEntrega.ToString("dd/mm/yyyy");
                DateTime fecha = sol.fechaEntrega.Value;
                string fech = fecha.ToString("dd/MM/yyyy");
                //DateTime fecha = sol.fechaEntrega.Value;
                mmsg.Body = "Hemos despachado su pedido numero: " + sol.idSolicitud + "\nCon fecha: " + fech + "\nTipo de despacho: " + sol.TipoDespacho.descripcion+ "\nNúmero de seguimiento: " + sol.codigoSeguimiento;
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new System.Net.Mail.MailAddress("solicitudes@promomas.cl");

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                //Hay que crear las credenciales del correo emisor
                cliente.Credentials = new System.Net.NetworkCredential("solicitudes@promomas.cl", "medalla24");

                //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                /*
                cliente.Port = 587;
                cliente.EnableSsl = true;
                */
                cliente.Host = "mail.promomas.cl"; //Para Gmail "smtp.gmail.com";

                /*-------------------------ENVIO DE CORREO----------------------*/

                try
                {
                    //Enviamos el mensaje      
                    cliente.Send(mmsg);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //Aquí gestionamos los errores al intentar enviar el correo
                }

                return RedirectToAction("SolicitudPedido");
            }
            return View(sol);
        }


        // GET: PRODUCTOes/Edit/5
        public ActionResult Despachar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new SolicitudViewModel();
            model.solicitud = db.SolicitudPedido.Find(id);
            //model.productosSeleccionados = model.solicitud.ProductosEnBodega.ToList();
            if (model.solicitud == null)
            {
                return HttpNotFound();
            }
            model.productosSeleccionados = db.ProductosEnBodega.Where(x => x.idSolicitud == model.solicitud.idSolicitud).ToList();
            model.productosSeleccionados2 = db.ProductosEnBodega.Where(x => x.idSolicitud == model.solicitud.idSolicitud).ToList();
            model.ProductoSolicitud2 = db.ProductoSolicitud.Where(x => x.idSolicitud == model.solicitud.idSolicitud).ToList();
            model.ProductoSolicitud3 = db.ProductoSolicitud.Where(x => x.idSolicitud == model.solicitud.idSolicitud).ToList();
            return View(model);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Despachar(SolicitudViewModel sol )
        {
            if (ModelState.IsValid)
            {
                for(int i = 0; i < sol.productosSeleccionados.Count; i++)
                {
                    int cant1 = sol.productosSeleccionados[i].cantidad;
                    int cant2 = sol.productosSeleccionados2[i].cantidad;

                    sol.productosSeleccionados2[i].cantidad = cant1 - cant2;

                    var id = sol.productosSeleccionados2[i].idProducto;
                    var id2 = sol.solicitud.idSolicitud;
                    var pbodega = db.ProductosEnBodega.Find(id2, id);
                    var queda = cant1 - cant2;
                    pbodega.cantidad = queda;
                    db.Entry(pbodega).State = EntityState.Modified;
                    
                    ViewBag.idproducto = new SelectList(db.PRODUCTO.ToList(), "idProducto", "Nombre");
                    db.SaveChanges();

                }
                
                return RedirectToAction("ProductoBodega");
            }
            return View(sol);
        }


        
        /*public ActionResult ProductosSolicitud(int? id , int? id2)
        {
            ViewBag.id = new SelectList(db.ProductoSolicitud.ToList(), id);
            ViewBag.id2 = new SelectList(db.ProductoSolicitud.ToList(), id2);
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelo = new SolicitudViewModel();

            modelo.lista2 = db.ProductoSolicitud.ToList();

            if (id != null)
            {
                modelo.lista2 = modelo.lista2.Where(r => r.despachada == 0).ToList();
            }
            return View(modelo);
        }
        */
        public ActionResult ProductosSolicitud(int? idSolicitud, SolicitudViewModel sol)
        {
            ViewBag.idTipoDespacho = new SelectList(db.TipoDespacho.ToList(), "idTipoDespacho", "descripcion");
            ViewBag.idArea = new SelectList(db.Area.ToList(), "idArea", "nombre");
            ViewBag.idProducto = new SelectList(db.PRODUCTO.ToList(), "IDProducto", "Nombre");
            //ViewBag.cantidad = ViewBag.IDCategoria;
            //ViewBag.IDClienteFK = ViewBag.IDEmpresa;
            List<ProductoSolicitud> lista = db.ProductoSolicitud.ToList();

            if (User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.despachada == 0).ToList();
            }

            
           

            return View(lista);
        }



        // GET: Solicitud/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Solicitud/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("SolicitudPedido");
            }
            catch
            {
                return View();
            }
        }

        // GET: Solicitud/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Solicitud/Delete/5
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

        public ActionResult StockMaximo(int id)
        {
            try
            {
                var stock = db.PRODUCTO.Where(x => x.IDProducto == id).FirstOrDefault();
                var data = stock.CantidadTotal;

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { }
            return null;
        }

        public ActionResult Confirmar(int idProducto, int idSolicitud, SolicitudViewModel sol)
        {
            var item = db.ProductoSolicitud.Find(idSolicitud,idProducto );
            
            db.Entry(item).State = EntityState.Modified;
            item.despachada = 1;
            
            db.SaveChanges();

            var item2 = db.SolicitudPedido.Where(x => x.ProductoSolicitud.Sum(y => y.despachada) > 0);
            if (1==1)
            {
                var cambio = db.SolicitudPedido.Find(idSolicitud);
                db.Entry(cambio).State = EntityState.Modified;
                cambio.cod_estado = 2;
                db.SaveChanges();
            }

            return RedirectToAction("ProductosSolicitud");
        }


        public ActionResult DownloadExcel2()
        {
            //lista temporal proveniente de la vista index libro
            List<ProductoSolicitud> sol = db.ProductoSolicitud.ToList();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            workSheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#B7DEE8"));

            workSheet.Cells["A1:K1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A1:K1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A1:K1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells["A1:K1"].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            workSheet.Cells["A1:K1"].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
            workSheet.Cells["A1:K1"].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            workSheet.Cells["A1:K1"].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

            //header
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "N° OC";
            workSheet.Cells[1, 2].Value = "DESTINO";
            workSheet.Cells[1, 3].Value = "FECHA SOLICITUD";
            workSheet.Cells[1, 4].Value = "N° GUIA";
            workSheet.Cells[1, 5].Value = "COMPRADOR";
            workSheet.Cells[1, 6].Value = "SOLICITANTE";
            workSheet.Cells[1, 7].Value = "OBSERVACIONES";
            workSheet.Cells[1, 8].Value = "ESTADO";
            workSheet.Cells[1, 9].Value = "CORREO";
            workSheet.Cells[1, 10].Value = "PRODUCTO";
            workSheet.Cells[1, 11].Value = "CANTIDAD";

            //cuerpo
            int index = 2;
            foreach (var p in sol)
            {
                string fechaen;
                if (p.SolicitudPedido.fechaSolicitud.HasValue)
                {
                    fechaen = p.SolicitudPedido.fechaSolicitud.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    fechaen = "";
                }


                workSheet.Cells[index, 1].Value = p.SolicitudPedido.descripcion;
                workSheet.Cells[index, 2].Value = p.SolicitudPedido.destino;
                workSheet.Cells[index, 3].Value = fechaen;
                workSheet.Cells[index, 4].Value = p.SolicitudPedido.codigoSeguimiento;
                workSheet.Cells[index, 5].Value = p.SolicitudPedido.usuarioMandante;
                workSheet.Cells[index, 6].Value = p.SolicitudPedido.usuarioReceptor;
                workSheet.Cells[index, 7].Value = p.SolicitudPedido.observacion;
                workSheet.Cells[index, 8].Value = p.SolicitudPedido.estado;
                workSheet.Cells[index, 9].Value = p.SolicitudPedido.correo;
                workSheet.Cells[index, 10].Value = p.PRODUCTO.Nombre;
                workSheet.Cells[index, 11].Value = p.cantidad;   
                index++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();
            workSheet.Column(7).AutoFit();
            workSheet.Column(8).AutoFit();
            workSheet.Column(9).AutoFit();
            workSheet.Column(10).AutoFit();
            workSheet.Column(11).AutoFit();
            workSheet.View.FreezePanes(2, 1);
            string excelName = "ReporteSolicitudes";

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            return View();
        }

        public ActionResult AdjuntarPDF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPedido SOL = db.SolicitudPedido.Find(id);
            if (SOL == null)
            {
                return HttpNotFound();
            }
            if (SOL.urlPdf == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/losheroesblob/" + SOL.urlPdf;
            }
            return View(SOL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdjuntarPDF(EditarPDFSolicitud sol, HttpPostedFileBase pdfSolicitud)
        {
            if (ModelState.IsValid)
            {
                var dbProd = db.SolicitudPedido.FirstOrDefault(p => p.idSolicitud == sol.idSolicitud);
                if (dbProd == null)
                {
                    return HttpNotFound();
                }
                string imgName = dbProd.descripcion + ".pdf";//variable local que concatena el codigo del producto mas .jpg(imagen)
                if (sol.urlPdf == null)
                {
                    sol.urlPdf = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                    new BlobService().AddPDFSol(pdfSolicitud, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }
                else
                {
                    new BlobService().AddPDFSol(pdfSolicitud, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }

                dbProd.urlPdf = sol.urlPdf;
                db.SaveChanges();

                return RedirectToAction("SolicitudPedido", "Solicitud");
            }
            return View(sol);
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
