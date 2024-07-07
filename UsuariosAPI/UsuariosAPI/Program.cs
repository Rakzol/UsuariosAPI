using Microsoft.EntityFrameworkCore;
using UsuariosAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatosUsuarioContext>(opciones =>
{
    opciones.UseMySql( builder.Configuration.GetConnectionString("DatosUsuarioContext"), ServerVersion.Parse("8.4.1-mysql"));
});

builder.Services.AddHttpClient("ApiClient", cliente =>
{
    cliente.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSettings")["baseUrl"]);
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Index}/{id?}");

app.Run();
