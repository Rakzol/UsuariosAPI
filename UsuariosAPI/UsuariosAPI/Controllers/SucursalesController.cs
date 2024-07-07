using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UsuariosAPI.Models.ViewModels;
using UsuariosAPI.Models;

namespace UsuariosAPI.Controllers
{
	public class SucursalesController : Controller
	{
		private readonly DatosUsuarioContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public SucursalesController(DatosUsuarioContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var cliente = _httpClientFactory.CreateClient("ApiClient");
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

				return View(JsonConvert.DeserializeObject<List<SucursalViewModel>>(json_sucursales));
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
		public async Task<IActionResult> Agregar(SucursalViewModel sucursalViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var cliente = _httpClientFactory.CreateClient("ApiClient");
					var contenido = new StringContent(JsonConvert.SerializeObject(sucursalViewModel), Encoding.UTF8, "application/json");
					var respuesta_sucursales = await cliente.PostAsync("api/ApiSucursales", contenido);
					var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
					Response.StatusCode = (int)respuesta_sucursales.StatusCode;
					if (!respuesta_sucursales.IsSuccessStatusCode)
					{
						var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
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

				var respuesta_sucursales = await cliente.GetAsync("api/ApiSucursales/" + id);
				var json_sucursales = await respuesta_sucursales.Content.ReadAsStringAsync();
				Response.StatusCode = (int)respuesta_sucursales.StatusCode;
				if (!respuesta_sucursales.IsSuccessStatusCode)
				{
					var error = JsonConvert.DeserializeObject<ValidationProblemDetails>(json_sucursales);
					Response.StatusCode = (int)error.Status;
					ViewData["errors"] = error.Errors;
					return View();
				}

				var sucursales = JsonConvert.DeserializeObject<SucursalViewModel>(json_sucursales);
				if (sucursales == null)
				{
					Response.StatusCode = 400;
					ViewData["errors"] = new Dictionary<string, string[]>
							{
								{ "ErrorServidor", new[] { "La sucursal no existe" } }
							};

					return View();
				}

				return View(sucursales);
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
		public async Task<IActionResult> Editar(uint id, SucursalViewModel sucursalViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var cliente = _httpClientFactory.CreateClient("ApiClient");
					var contenido = new StringContent(JsonConvert.SerializeObject(sucursalViewModel), Encoding.UTF8, "application/json");
					var respuesta_put = await cliente.PutAsync("api/ApiSucursales/" + id, contenido);
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

				return View(sucursalViewModel);
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
				var respuesta = await cliente.DeleteAsync("api/ApiSucursales/" + id);
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
