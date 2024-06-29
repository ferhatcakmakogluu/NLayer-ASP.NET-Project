using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.API.Modules;
using NLayer.Repository;
using NLayer.Service.Mapping;
using NLayer.Service.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute()))
    .AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//sistemin kendi filter'ini disable yapma
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Caching ekleme
builder.Services.AddMemoryCache();

//NotFoundFilteri tanýmladýk
builder.Services.AddScoped(typeof(NotFoundFilter<>));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Kendi yazdigimiz middleware i kullanmak icin
app.UseCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
