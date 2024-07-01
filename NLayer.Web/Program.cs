using Autofac.Extensions.DependencyInjection;
using Autofac;
using NLayer.Web.Modules;
using Microsoft.EntityFrameworkCore;
using NLayer.Repository;
using System.Reflection;
using NLayer.Service.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Automapper dahil etme
builder.Services.AddAutoMapper(typeof(MapProfile));


//Veri tabaný baglantisini sagladik
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), options =>
    {
        //AppDbContext'in bulundugu yeri dinamik olarak verdik
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});


//Autofac ekledik
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//olusturdugumuz RepoServiceModule yi program.cs e bildirdik
builder.Host.ConfigureContainer<ContainerBuilder>
    (containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
