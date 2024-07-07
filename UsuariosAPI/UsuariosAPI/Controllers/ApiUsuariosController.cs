using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using UsuariosAPI.Models.ViewModels;
using UsuariosAPI.Models;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UsuariosAPI.Models.Filters;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Diagnostics;

namespace UsuariosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsuariosController : ControllerBase
    {
        private readonly DatosUsuarioContext _context;

        public ApiUsuariosController(DatosUsuarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UsuarioViewModel), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioViewModel>> PostUsuario(UsuarioViewModel usuarioViewModel)
        {
            try
            {
                var usuario = new Usuario
                {
                    Id = usuarioViewModel.Id,
                    Email = usuarioViewModel.Email,
                    Contraseña = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(usuarioViewModel.Contraseña))).Replace("-","").ToLower(),
                    Nombres = usuarioViewModel.Nombres,
                    Apellidos = usuarioViewModel.Apellidos,
                    Rol = usuarioViewModel.IdRol,
                    Sucursal = usuarioViewModel.IdSucursal
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                usuarioViewModel.Id = usuario.Id;
                usuarioViewModel.Contraseña = usuario.Contraseña;
                return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuarioViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlException mySqlException)
                {
                    if (mySqlException.Number == 1062)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ValidationProblemDetails(ModelState)
                        {
                            Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "Id o Email duplicado" } }
                            }
                        });
                    }
                    if (mySqlException.Number == 1452)
                    {
                        return BadRequest( new ValidationProblemDetails(ModelState)
                        {
                            Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "El IdSucursal o IdRol no son validos" } }
                            }
                        });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
					{
						Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con la base de datos" } }
							}
					});
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
				{
					Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							}
				});
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioViewModel>), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<UsuarioViewModel>>> GetUsuarios([FromQuery] UsuarioFilter usuarioFilter )
        {
            try
            {
                IQueryable<Usuario> query = _context.Usuarios.AsQueryable();

                if (usuarioFilter.Id != 0)
                    query = query.Where(u => u.Id == usuarioFilter.Id);

                if (!string.IsNullOrEmpty(usuarioFilter.Email))
                    query = query.Where(u => u.Email.Contains(usuarioFilter.Email));

                if (!string.IsNullOrEmpty(usuarioFilter.Nombres))
                    query = query.Where(u => u.Nombres.Contains(usuarioFilter.Nombres));

                if (!string.IsNullOrEmpty(usuarioFilter.Apellidos))
                    query = query.Where(u => u.Apellidos.Contains(usuarioFilter.Apellidos));

                if (usuarioFilter.IdSucursal != 0)
                    query = query.Where(u => u.Sucursal == usuarioFilter.IdSucursal);

                if (usuarioFilter.IdRol != 0)
                    query = query.Where(u => u.Rol == usuarioFilter.IdRol);

                return Ok(await (
                    from user in query
                    orderby user.Nombres
                    select new UsuarioViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Contraseña = user.Contraseña,
                        Nombres = user.Nombres,
                        Apellidos = user.Apellidos,
                        IdSucursal = user.Sucursal,
                        IdRol = user.Rol
                    }
                ).ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
				{
					Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con la base de datos" } }
							}
				});
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> PutUsuario(uint id, UsuarioViewModel usuarioViewModel)
        {
            try
            {
                if (id != usuarioViewModel.Id)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Las Id no coinciden" } }
                        }
                    });
                }

                var contraseña = await (from u in _context.Usuarios where u.Id == id select u.Contraseña).SingleOrDefaultAsync();
				if (contraseña == null)
				{
					return NotFound(new ValidationProblemDetails(ModelState)
					{
						Errors = new Dictionary<string, string[]>
							{
								{ "ErrorValidacion", new[] { "El Usuario no existe" } }
							}
					});
				}

				_context.Entry(new Usuario
                {
                    Id = usuarioViewModel.Id,
                    Email = usuarioViewModel.Email,
                    Contraseña = (usuarioViewModel.Contraseña != contraseña ? BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(usuarioViewModel.Contraseña))).Replace("-", "").ToLower() : contraseña),
                    Nombres = usuarioViewModel.Nombres,
                    Apellidos = usuarioViewModel.Apellidos,
                    Rol = usuarioViewModel.IdRol,
                    Sucursal = usuarioViewModel.IdSucursal
                }).State = EntityState.Modified;

                await _context.SaveChangesAsync();

				return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlException mySqlException)
                {
                    if (mySqlException.Number == 1062)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ValidationProblemDetails(ModelState)
                        {
                            Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "Email duplicado" } }
                            }
                        });
                    }
                    if (mySqlException.Number == 1452)
                    {
                        return BadRequest(new ValidationProblemDetails(ModelState)
                        {
                            Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "El IdSucursal o IdRol no son validos" } }
                            }
                        });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
					{
						Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con la base de datos" } }
							}
					});
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
				{
					Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" + ex.Message } }
							}
				});
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteUsuario(uint id)
        {
            try
            {
                var usuario = await (from u in _context.Usuarios where u.Id == id select u).SingleOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Usuario no encontrado" } }
                        }
                    });
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ValidationProblemDetails(ModelState)
				{
					Errors = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con la base de datos" } }
							}
				});
            }
        }

    }
}
