using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:7139")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
});

builder.Services.AddSession();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["JwtToken"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }

    await next();
});

app.UseRouting();

app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
