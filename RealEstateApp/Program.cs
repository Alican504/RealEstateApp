using Microsoft.EntityFrameworkCore;
using RealEstateApp.DAL; // DbContext'imizin bulunduðu katman
using RealEstateApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- BÝZÝM EKLEDÝÐÝMÝZ VERÝTABANI BAÐLANTI AYARI ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ---------------------------------------------------

builder.Services.AddScoped<IPropertyService, PropertyService>();

builder.Services.AddSession();
// DÝKKAT: Üstteki ayar mutlaka bu Build() satýrýndan önce olmalý!
var app = builder.Build();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();