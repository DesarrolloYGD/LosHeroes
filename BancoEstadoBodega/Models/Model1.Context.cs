﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class LosHeroesEntities1 : DbContext
    {
        public LosHeroesEntities1()
            : base("name=LosHeroesEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<BODEGA> BODEGA { get; set; }
        public virtual DbSet<CATEGORIA> CATEGORIA { get; set; }
        public virtual DbSet<CIUDAD> CIUDAD { get; set; }
        public virtual DbSet<CLIENTE> CLIENTE { get; set; }
        public virtual DbSet<COMUNA> COMUNA { get; set; }
        public virtual DbSet<estadoSolicitud> estadoSolicitud { get; set; }
        public virtual DbSet<Mecanizado> Mecanizado { get; set; }
        public virtual DbSet<PRODUCTO> PRODUCTO { get; set; }
        public virtual DbSet<PRODUCTOBODEGA> PRODUCTOBODEGA { get; set; }
        public virtual DbSet<ProductosEnBodega> ProductosEnBodega { get; set; }
        public virtual DbSet<ProductoSolicitud> ProductoSolicitud { get; set; }
        public virtual DbSet<REGION> REGION { get; set; }
        public virtual DbSet<SolicitudPedido> SolicitudPedido { get; set; }
        public virtual DbSet<TipoDespacho> TipoDespacho { get; set; }
        public virtual DbSet<TipoEmpaque> TipoEmpaque { get; set; }
        public virtual DbSet<TipoEncomienda> TipoEncomienda { get; set; }
        public virtual DbSet<TipoPedido> TipoPedido { get; set; }
        public virtual DbSet<TrasladoDespacho> TrasladoDespacho { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
        public virtual DbSet<UserSoloVista> UserSoloVista { get; set; }
    
        public virtual ObjectResult<sp_listarProductos_Result> sp_listarProductos()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_listarProductos_Result>("sp_listarProductos");
        }
    
        public virtual int sp_obtenerDistribucion()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_obtenerDistribucion");
        }
    
        public virtual int sp_vincularProductosSolicitud()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_vincularProductosSolicitud");
        }
    }
}
