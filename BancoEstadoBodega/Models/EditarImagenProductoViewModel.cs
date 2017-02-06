using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BancoEstadoBodega.Models;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class EditarImagenProductoViewModel
    {
        public string Codigo { get; set; }
        public int IdProducto { get; set; }
        public string UrlImagen { get; set; }
    }
}