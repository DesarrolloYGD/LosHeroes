using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BancoEstadoBodega.Models;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;

namespace BancoEstadoBodega.Controllers
{
    public class ExportExcelController : Controller
    {
        private LosHeroesEntities1 db = new LosHeroesEntities1();
        // GET: ExcelStock
        public ActionResult Index()
        {
            return View();
        }

        //Funcion para exportar el stock de productos
        public ActionResult ExportDataProductos()
        {
            string condicion = "";
            if (User.IsInRole("administradores"))
            {
                condicion = "";
            }else
            {
                int cod = EnRol();
                condicion = "WHERE idclientefk = " + cod;
            }

            String constring = ConfigurationManager.ConnectionStrings["Hola"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            string query = "select codigo, Nombre, cantidadTotal from Producto";
            DataTable dt = new DataTable();
            dt.TableName = "PRODUCTO";
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= StockProductos.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", "ExportData");
        }
        //metodo exportar productos con stock critico
        public ActionResult ExportDataCritico()
        {
            string condicion = "";
            if (User.IsInRole("administradores"))
            {
                condicion = "where ";
            }
            else
            {
                int cod = EnRol();
                condicion = "WHERE idclientefk = " + cod + "and";
            }

            String constring = ConfigurationManager.ConnectionStrings["Hola"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            string query = "select codigo, Nombre, cantidadTotal from Producto " + condicion + "(cantidadtotal*100)/(stock_ideal) <= 20";
            DataTable dt = new DataTable();
            dt.TableName = "PRODUCTO";
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= StockProductos.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", "ExportData");
        }



        //Funcion para exportar las solicitudes
        public ActionResult ExportDataSolicitudes()
        {
            String constring = ConfigurationManager.ConnectionStrings["Hola"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);
            string query = "select * from SolicitudPedido";
            DataTable dt = new DataTable();
            dt.TableName = "SolicitudPedido";
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= Solicitudes.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", "ExportData");
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



        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}