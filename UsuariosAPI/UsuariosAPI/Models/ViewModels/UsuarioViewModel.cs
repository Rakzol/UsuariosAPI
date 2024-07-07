using System.ComponentModel.DataAnnotations;

namespace UsuariosAPI.Models.ViewModels
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "El campo Id es requerido")]
        [Display(Name = "Id del usuario")]
        public uint Id { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [Display(Name = "Correo electronico")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "El campo Nombres es requerido")]
        [Display(Name = "Nombres")]
        [StringLength(255, ErrorMessage = "El campo Nombres debe maximo 255 caracteres")]
        [RegularExpression(@"^(?! )[A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+(?: [A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+)*(?<! )$", ErrorMessage = "El campo Nombres no tiene un formato valido")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El campo Apellidos es requerido")]
        [Display(Name = "Apellidos")]
        [StringLength(255, ErrorMessage = "El campo Apellidos debe maximo 255 caracteres")]
        [RegularExpression(@"^(?! )[A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+(?: [A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+)*(?<! )$", ErrorMessage = "El campo Apellidos no tiene un formato valido")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El campo IdSucursal es requerido")]
        [Display(Name = "Id de la sucursal")]
        public uint IdSucursal { get; set; }

        [Required(ErrorMessage = "El campo IdUsuario es requerido")]
        [Display(Name = "Id del rol")]
        public uint IdRol { get; set; }
    }
}
