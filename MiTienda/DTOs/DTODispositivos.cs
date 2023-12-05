using MiTienda.Models;

namespace MiTienda.DTOs
{
    public class DTODispositivos
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public bool Descatalogado { get; set; }
        public int MarcaId { get; set; }

    }
    public class DTODispositosDetalles
    {
        public int IdMarca { get; set; }
        public string NombreMarca { get; set; }
        public string NombreCategoria { get; set; }
        public decimal PromedioPrecio { get; set; }
        public int CuentaDispositivos { get; set; }
        public List<DTODispositosDetalles2> ListaDispositivos { get; set; }
    }

    public class DTODispositosDetalles2
    {
        public int IdDispositivo { get; set; }
        public string NombreDispositivo { get; set; }
        public decimal Precio { get; set; }
    }


}
