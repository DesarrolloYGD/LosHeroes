using System;
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

namespace BancoEstadoBodega.Controllers
{
    [Authorize]
    public class SolicitudController : Controller
    {
        private LosHeroesEntities db = new LosHeroesEntities();


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
                        db.ProductoSolicitud.Add(ProductoSolicitud);
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

                mmsg.Body = "Hemos recibido su pedido numero: " + idSol + "\nCon fecha: " + fech + " \nSu pedido será despachado en un plazo de 1 a 2 días hábiles" + "\nPara mas detalles porfavor visitar la página sección solicitudes ";
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
