using System;
using System.Collections.Generic;

namespace UsuariosAPI.Models;

public partial class Sucursal
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
