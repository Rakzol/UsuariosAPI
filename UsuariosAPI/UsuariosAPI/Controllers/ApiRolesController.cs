using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Diagnostics;
using UsuariosAPI.Models;
using UsuariosAPI.Models.ViewModels;

namespace UsuariosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRolesController : ControllerBase
    {
        private readonly DatosUsuarioContext _context;

        public ApiRolesController(DatosUsuarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RolViewModel), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RolViewModel>> PostRol(RolViewModel rolViewModel)
        {
            try
            {
                var rol = new Rol
                {
                    Id = rolViewModel.Id,
                    Nombre = rolViewModel.Nombre
                };

                _context.Add(rol);
                await _context.SaveChangesAsync();

                rolViewModel.Id = rol.Id;
                return CreatedAtAction("GetRol", new { id = rol.Id }, rolViewModel);
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
                                { "ErrorValidacion", new[] { "Id o Nombre duplicado" } }
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
        [ProducesResponseType(typeof(IEnumerable<RolViewModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RolViewModel>>> GetRoles()
        {
            try
            {
                return Ok(await (
                    from rol in _context.Roles
                    orderby rol.Nombre
                    select new RolViewModel
                    {
                        Id = rol.Id,
                        Nombre = rol.Nombre
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolViewModel), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RolViewModel>> GetRol(uint id)
        {
            try
            {
                var rol = await ( from r in _context.Roles where r.Id == id select r).SingleOrDefaultAsync();

                if (rol == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Rol no encontrado" } }
                        }
                    });
                }

                return Ok(new RolViewModel 
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre
                });
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
        public async Task<ActionResult> PutRol(uint id, RolViewModel rolViewModel)
        {
            try
            {
                if (id != rolViewModel.Id)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Las Id no coinciden" } }
                        }
                    });
                }

                _context.Entry( new Rol 
                {
                    Id = rolViewModel.Id,
                    Nombre = rolViewModel.Nombre
                } ).State = EntityState.Modified;

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
                                { "ErrorValidacion", new[] { "Nombre duplicado" } }
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
                if (await (from r in _context.Roles where r.Id == id select r).SingleOrDefaultAsync() == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "Rol no encontrado" } }
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

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteRol(uint id)
        {
            try
            {
                var rol = await (from r in _context.Roles where r.Id == id select r).SingleOrDefaultAsync();

                if (rol == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Rol no encontrado" } }
                        }
                    });
                }

                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
				if (ex.InnerException is MySqlException mySqlException)
				{
					if (mySqlException.Number == 1451)
					{
						return StatusCode(StatusCodes.Status409Conflict, new ValidationProblemDetails(ModelState)
						{
							Errors = new Dictionary<string, string[]>
							{
								{ "ErrorValidacion", new[] { "El Rol esta siendo usado" } }
							}
						});
					}
				}
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
