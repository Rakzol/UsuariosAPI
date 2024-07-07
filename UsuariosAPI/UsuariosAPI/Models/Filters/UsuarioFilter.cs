using System.ComponentModel.DataAnnotations;

namespace UsuariosAPI.Models.Filters
{
    public class UsuarioFilter
    {
        public uint Id { get; set; }
        public string? Email { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        [Display(Name = "Sucursal")]
        public uint IdSucursal { get; set; }
		[Display(Name = "Rol")]
		public uint IdRol { get; set; }
    }
}
