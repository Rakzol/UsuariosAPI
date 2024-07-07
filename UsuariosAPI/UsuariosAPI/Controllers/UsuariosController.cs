using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using UsuariosAPI.Models;
using UsuariosAPI.Models.Filters;
using UsuariosAPI.Models.ViewModels;

namespace UsuariosAPI.Controllers
{
	public class UsuariosController : Controller
	{
		private readonly DatosUsuarioContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public UsuariosController(DatosUsuarioContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index([FromQuery] UsuarioFilter usuarioFilter)
		{
			try
			{
				var query = "";
				if (usuarioFilter.Id != 0)
					query += "Id=" + usuarioFilter.Id + "&";
				if (!string.IsNullOrEmpty(usuarioFilter.Email))
					query += "Email=" + usuarioFilter.Email + "&";
				if (!string.IsNullOrEmpty(usuarioFilter.Nombres))
					query += "Nombres=" + usuarioFilter.Nombres + "&";
				if (!string.IsNullOrEmpty(usuarioFilter.Apellidos))
					query += "Apellidos=" + usuarioFilter.Apellidos + "&";
				if (usuarioFilter.IdSucursal != 0)
					query += "IdSucursal=" + usuarioFilter.IdSucursal + "&";
				if (usuarioFilter.IdRol != 0)
					query += "IdRol=" + usuarioFilter.IdRol + "&";

				var cliente = _httpClientFactory.CreateClient("ApiClient");
				var respuesta_usuarios = await cliente.GetAsync("api/ApiUsuarios?" + query);
				var json_usuarios = await respuesta_usuarios.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_usuarios.StatusCode;
				if (!respuesta_usuarios.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_usuarios);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				ViewData["roles"] = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				ViewData["sucursales"] = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				return View(JsonConvert.DeserializeObject<List<UsuarioViewModel>>(json_usuarios));
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		public async Task<IActionResult> Filtrar()
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				roles.Add(new RolViewModel {
					Id = 0,
					Nombre = ""
				});
				ViewData["roles"] = new SelectList(roles, "Id", "Nombre", 0);

				var sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				sucursales.Add(new SucursalViewModel
				{
					Id = 0,
					Nombre = ""
				});
				ViewData["sucursales"] = new SelectList(sucursales, "Id", "Nombre", 0);

				return View();
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		public async Task<IActionResult> Agregar()
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				ViewData["roles"] = new SelectList(roles, "Id", "Nombre", 0);

				var sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				ViewData["sucursales"] = new SelectList(sucursales, "Id", "Nombre", 0);

				return View();
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Agregar(UsuarioViewModel usuarioViewModel)
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				ViewData["roles"] = new SelectList(roles, "Id", "Nombre", usuarioViewModel.IdRol);

				var sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				ViewData["sucursales"] = new SelectList(sucursales, "Id", "Nombre", usuarioViewModel.IdSucursal);

				if (ModelState.IsValid)
				{
					var contenido = new StringContent(JsonConvert.SerializeObject(usuarioViewModel), Encoding.UTF8, "application/json");
					var respuesta_usuarios = await cliente.PostAsync("api/ApiUsuarios", contenido);
					var json_usuarios = await respuesta_usuarios.Content.ReadAsStringAsync();
					Response.StatusCode = (int)respuesta_usuarios.StatusCode;
					if (!respuesta_usuarios.IsSuccessStatusCode)
					{
						var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_usuarios);
						Response.StatusCode = (int)error.Status;
						ViewData["errors"] = error.Errors;
						return View();
					}
					return RedirectToAction(nameof(Index));
				}

				return View();
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Editar(uint id)
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");

				var respuesta_usuarios = await cliente.GetAsync("api/ApiUsuarios?Id=" + id);
				var json_usuarios = await respuesta_usuarios.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_usuarios.StatusCode;
				if (!respuesta_usuarios.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_usuarios);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var usuarios = JsonConvert.DeserializeObject<List<UsuarioViewModel>>(json_usuarios);
				if(usuarios.Count() == 0)
				{
					Response.StatusCode = 400;
					ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "El Usuario no existe" } }
							};

					ViewData["roles"] = new SelectList(JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles), "Id", "Nombre");
					ViewData["sucursales"] = new SelectList(JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales), "Id", "Nombre");
					return View();
				}

				var roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				ViewData["roles"] = new SelectList(roles, "Id", "Nombre", usuarios[0].IdRol);

				var sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				ViewData["sucursales"] = new SelectList(sucursales, "Id", "Nombre", usuarios[0].IdSucursal);
				return View(usuarios[0]);
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Editar(uint id, UsuarioViewModel usuarioViewModel)
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles");
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales");
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles);
				ViewData["roles"] = new SelectList(roles, "Id", "Nombre", usuarioViewModel.IdRol);

				var sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales);
				ViewData["sucursales"] = new SelectList(sucursales, "Id", "Nombre", usuarioViewModel.IdSucursal);

				if (ModelState.IsValid)
				{
					var contenido = new StringContent(JsonConvert.SerializeObject(usuarioViewModel), Encoding.UTF8, "application/json");
					var respuesta_put = await cliente.PutAsync("api/ApiUsuarios/" + id, contenido);
					var json_put = await respuesta_put.Content.ReadAsStringAsync();
					Response.StatusCode = (int)respuesta_put.StatusCode;
					if (!respuesta_put.IsSuccessStatusCode)
					{
						var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_put);
						Response.StatusCode = (int)error.Status;
						ViewData["errors"] = error.Errors;
						return View();
					}
					return RedirectToAction(nameof(Index));
				}

				return View(usuarioViewModel);
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

		public async Task<IActionResult> Eliminar(uint id)
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");
				var respuesta = await cliente.DeleteAsync("api/ApiUsuarios/" + id);
				var json = await respuesta.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta.StatusCode;
				if (!respuesta.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				Response.StatusCode = 500;
				ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "Error con el servidor" } }
							};
			}
			return View();
		}

	}
}
