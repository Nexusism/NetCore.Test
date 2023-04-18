
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NetCore.Services.Data;
using NetCore.Services.interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region 서비스 의존성주입 설정
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
#endregion

#region DB 접속정보, Migrations 프로젝트 지정
//builder.Services.AddDbContext<CodeFirstDbContext>(options =>
//                    options.UseSqlServer(connectionString:builder.Configuration.GetConnectionString(name: "DefaultConnection"),
//                                         sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));


builder.Services.AddDbContext<DBFirstDbContext>(options =>
                                options.UseSqlServer(connectionString:builder.Configuration.GetConnectionString(name: "DBFirstDBConnection"),
                                sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));
#endregion

#region 로그인관련 설정
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

#region 데이터 보호
Common.SetDataProtection(builder.Services, @"C:\Users\LJG\Desktop\ERP개발\Project\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);
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

// 승인권한을 사용하기 위해 추가됨.
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();
//app.MapControllerRoute();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EUser}/{action=Login}/{id?}");

app.Run();
