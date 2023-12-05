using System;
using System.Collections.Generic;

namespace MiTienda.Models;

public partial class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Marca> Marcas { get; set; } = new List<Marca>();
}
