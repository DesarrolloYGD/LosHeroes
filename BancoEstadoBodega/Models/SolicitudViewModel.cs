using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BancoEstadoBodega.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;



namespace BancoEstadoBodega.Models
{
    public class SolicitudViewModel
    {
        public SolicitudPedido solicitud { get; set; }
        public ProductosEnBodega bodega { get; set; }
        public List<SelectListItem> PRODUCTO { get; set; }
        public List<ProductosEnBodega> productosSeleccionados { get; set; }
        public List<ProductosEnBodega> productosSeleccionados2 { get; set; }
        public List<SelectListItem> UserSoloVista { get; set; }
        public List<SelectListItem> ProductosEnBodega { get; set; }
        public List<ProductoSolicitud> ProductoSolicitud2 { get; set; }
        public List<ProductoSolicitud> ProductoSolicitud3 { get; set; }
        public List<SelectListItem> ProductoSolicitud { get; set; }


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


        public List<PRODUCTO> lista { get; set; }
        public string sumasolicitdada { get; set; }

        public List<ProductoSolicitud> lista2 { get; set; }
    }
}