using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class SolicitudViewModel
    {
        public SolicitudPedido solicitud { get; set; }
        public List<SelectListItem> PRODUCTO { get; set; }
        public List<ProductoSolicitud> productosSeleccionados { get; set; }



        public String numeroOC { get; set; }
        public String detalleDestino { get; set; }

        public String Comprador { get; set; }

        public String Solicitante { get; set; }
        public String Observacion { get; set; }


        public IEnumerable<SelectListItem> Destinos
        {
            get
            {
                return new[]
                {
                new SelectListItem { Value = "Holanda 100 Sócalo", Text = "Holanda 100 Sócalo" },
                new SelectListItem { Value = "Holanda 100 Piso 5", Text = "Holanda 100 Piso 5" },
                new SelectListItem { Value = "Holanda 64", Text = "Holanda 64" },
                new SelectListItem { Value = "Sucursal Independencia", Text = "Sucursal Independencia" },
                new SelectListItem { Value = "Prilogic", Text = "Prilogic" },
            };
            }
        }

        public string DestinoSeleccionado { get; set; }
    }
}