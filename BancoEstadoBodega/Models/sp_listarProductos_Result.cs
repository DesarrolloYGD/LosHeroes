//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BancoEstadoBodega.Models
{
    using System;
    
    public partial class sp_listarProductos_Result
    {
        public int IDProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> IDCategoriaFK { get; set; }
        public Nullable<System.DateTime> FechaVencimiento { get; set; }
        public byte[] imagenProducto { get; set; }
        public string Posicion { get; set; }
        public Nullable<int> CantidadTotal { get; set; }
        public Nullable<int> UnidadesXCaja { get; set; }
        public Nullable<int> TotalCajas { get; set; }
        public Nullable<int> TotalSueltas { get; set; }
        public Nullable<int> IDClienteFK { get; set; }
        public string UrlImagen { get; set; }
        public Nullable<int> StockQl { get; set; }
        public Nullable<int> StockDÑ { get; set; }
        public byte[] foto { get; set; }
        public string ImageMimeType { get; set; }
        public Nullable<int> SueltasQL { get; set; }
        public Nullable<int> SueltasDÑ { get; set; }
        public Nullable<int> stock_ideal { get; set; }
    }
}
