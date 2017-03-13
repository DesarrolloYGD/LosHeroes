using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BancoEstadoBodega.Models;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class EditarPDFSolicitud
    {
        public string descripcion { get; set; }
        public int idSolicitud { get; set; }
        public string urlPdf { get; set; }
    }
}




