using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UsuariosAPI.Models;
using UsuariosAPI.Models.ViewModels;

namespace UsuariosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSucursalesController : ControllerBase
    {
        private readonly DatosUsuarioContext _context;

        public ApiSucursalesController(DatosUsuarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SucursalViewModel), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SucursalViewModel>> PostSucursal(SucursalViewModel sucursalViewModel)
        {
            try
            {
                var sucursal = new Sucursal
                {
                    Id = sucursalViewModel.Id,
                    Nombre = sucursalViewModel.Nombre
                };

                _context.Add(sucursal);
                await _context.SaveChangesAsync();

                sucursalViewModel.Id = sucursal.Id;
                return CreatedAtAction("GetSucursal", new { id = sucursal.Id }, sucursalViewModel);
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
        [ProducesResponseType(typeof(IEnumerable<SucursalViewModel>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SucursalViewModel>>> GetSucursales()
        {
            try
            {
                return Ok(await (
                    from sucursal in _context.Sucursales
                    orderby sucursal.Nombre
                    select new SucursalViewModel
                    {
                        Id = sucursal.Id,
                        Nombre = sucursal.Nombre
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
        [ProducesResponseType(typeof(SucursalViewModel), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SucursalViewModel>> GetSucursal(uint id)
        {
            try
            {
                var sucursal = await (from s in _context.Sucursales where s.Id == id select s).SingleOrDefaultAsync();

                if (sucursal == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Sucursal no encontrada" } }
                        }
                    });
                }

                return Ok(new SucursalViewModel
                {
                    Id = sucursal.Id,
                    Nombre = sucursal.Nombre
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
        public async Task<ActionResult> PutSucursal(uint id, SucursalViewModel sucursalViewModel)
        {
            try
            {
                if (id != sucursalViewModel.Id)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Las Id no coinciden" } }
                        }
                    });
                }

                _context.Entry(new Sucursal
                {
                    Id = sucursalViewModel.Id,
                    Nombre = sucursalViewModel.Nombre
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
                if (await (from s in _context.Sucursales where s.Id == id select s).SingleOrDefaultAsync() == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                            {
                                { "ErrorValidacion", new[] { "Sucursal no encontrada" } }
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
        public async Task<ActionResult> DeleteSucursal(uint id)
        {
            try
            {
                var sucursal = await (from s in _context.Sucursales where s.Id == id select s).SingleOrDefaultAsync();

                if (sucursal == null)
                {
                    return NotFound(new ValidationProblemDetails(ModelState)
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ErrorValidacion", new[] { "Sucursal no encontrada" } }
                        }
                    });
                }

                _context.Sucursales.Remove(sucursal);
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
								{ "ErrorValidacion", new[] { "La sucursal esta siendo usado" } }
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
