using Microsoft.EntityFrameworkCore;
using SmartHome.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısını (Connection String) Al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. DbContext'i ve MySQL Sürücüsünü Kaydet
builder.Services.AddDbContext<SmartHomeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. MVC (Model-View-Controller) Servislerini Ekle
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. HTTP İstek Kanalı (Middleware Pipeline) Yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

// 5. Varsayılan Rota Tanımı
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();