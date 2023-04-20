
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NetCore.Services.Data;
using NetCore.Services.interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;
using NLog.Fluent;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
#region �α� ����
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

#region ���� ���������� ����
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DBFirstDbinitializer>();
#endregion

#region DB ��������, Migrations ������Ʈ ����
//builder.Services.AddDbContext<CodeFirstDbContext>(options =>
//                    options.UseSqlServer(connectionString:builder.Configuration.GetConnectionString(name: "DefaultConnection"),
//                                         sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));


builder.Services.AddDbContext<DBFirstDbContext>(options =>
                                options.UseSqlServer(connectionString:builder.Configuration.GetConnectionString(name: "DBFirstDBConnection"),
                                sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));
#endregion

#region �α��ΰ��� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/EUser/Login";
    options.LogoutPath = "/EUser/LogOut";
    options.AccessDeniedPath = "/EUser/Forbidden";
});
builder.Services.AddAuthorization();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/EUser/Login";
    options.LogoutPath = "/EUser/LogOut";
    options.AccessDeniedPath = "/EUser/Forbidden";
});
#endregion

#region ���� ����
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".NetCore.Session";
    // ���� ���ѽð�
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
#endregion

#region ������ ��ȣ
Common.SetDataProtection(builder.Services, @"C:\Users\LJG\Desktop\ERP����\Project\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);
#endregion

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

// ���α����� ����ϱ� ���� �߰���.
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.MapControllerRoute();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EUser}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    DBFirstDbinitializer initializer = scope.ServiceProvider
                                            .GetService<DBFirstDbinitializer>();

    int rowAffected = initializer.PlantSeedData();
}

app.Run();
