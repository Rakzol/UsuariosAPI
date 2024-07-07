using System;
using System.Collections.Generic;

namespace UsuariosAPI.Models;

public partial class Usuario
{
    public uint Id { get; set; }

    public string Email { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public uint Rol { get; set; }

    public uint Sucursal { get; set; }

    public virtual Rol RolNavigation { get; set; } = null!;

    public virtual Sucursal SucursalNavigation { get; set; } = null!;
}
