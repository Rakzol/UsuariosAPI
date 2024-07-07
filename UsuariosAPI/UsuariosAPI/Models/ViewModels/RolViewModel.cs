﻿using System.ComponentModel.DataAnnotations;

namespace UsuariosAPI.Models.ViewModels
{
    public class RolViewModel
    {
        [Required(ErrorMessage = "El campo Id es requerido")]
        [Display(Name = "Id del Rol")]
        public uint Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [Display(Name = "Nombre del Rol")]
        [StringLength(255, ErrorMessage = "El campo Nombre debe maximo 255 caracteres")]
        [RegularExpression(@"^(?! )[A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+(?: [A-Za-zñÑáéíóúÁÉÍÓÚüÜ]+)*(?<! )$", ErrorMessage = "El Nombre no tiene un formato valido")]
        public string Nombre { get; set; }
    }
}
