using System;
using System.Collections.Generic;

namespace MiTienda.Models;

public partial class Dispositivo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public bool Descatalogado { get; set; }

    public int MarcaId { get; set; }

    public virtual Marca Marca { get; set; } = null!;
}
