using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UsuariosAPI.Models.ViewModels;
using UsuariosAPI.Models;

namespace UsuariosAPI.Controllers
{
	public class RolesController : Controller
	{
		private readonly DatosUsuarioContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public RolesController(DatosUsuarioContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index()
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

				return View(JsonConvert.DeserializeObject<List<RolViewModel>>(json_roles));
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
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Agregar(RolViewModel rolViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var cliente = _httpClientFactory.CreateClient("ApiClient");
					var contenido = new StringContent(JsonConvert.SerializeObject(rolViewModel), Encoding.UTF8, "application/json");
					var respuesta_roles = await cliente.PostAsync("api/ApiRoles", contenido);
					var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
					Response.StatusCode = (int)respuesta_roles.StatusCode;
					if (!respuesta_roles.IsSuccessStatusCode)
					{
						var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
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

				var respuesta_roles = await cliente.GetAsync("api/ApiRoles/" + id);
				var json_roles = await respuesta_roles.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_roles.StatusCode;
				if (!respuesta_roles.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_roles);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var rol = JsonConvert.DeserializeObject<RolViewModel>(json_roles);
				if (rol == null)
				{
					Response.StatusCode = 400;
					ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "El Rol no existe" } }
							};

					return View();
				}

				return View(rol);
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
		public async Task<IActionResult> Editar(uint id, RolViewModel rolViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var cliente = _httpClientFactory.CreateClient("ApiClient");
					var contenido = new StringContent(JsonConvert.SerializeObject(rolViewModel), Encoding.UTF8, "application/json");
					var respuesta_put = await cliente.PutAsync("api/ApiRoles/" + id, contenido);
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

				return View(rolViewModel);
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
				var respuesta = await cliente.DeleteAsync("api/ApiRoles/" + id);
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
