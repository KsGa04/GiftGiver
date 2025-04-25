using GiftGiver.Controllers;
using GiftGiver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using GiftGiver;
using GiftGiver.Controllers;
using System;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddDbContext<giftgiverContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddScoped<AuthApi>();
builder.Services.AddScoped<giftgiverContext>();
builder.Services.AddTransient<AllProductsApi>();
builder.Services.AddTransient<WishListApi>();
builder.Services.AddTransient<TapeApi>();
builder.Services.AddTransient<RegApi>();
builder.Services.AddTransient<AddProductApi>();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:5000");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.Cookie.Name = "ClientData.Cookies";
        options.LoginPath = "/home/Authorization";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Authorization}/{id?}");

app.Run();
