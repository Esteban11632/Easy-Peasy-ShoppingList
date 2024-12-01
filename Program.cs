using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using UserAuthentication;
using TaskManager;
using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Data;

var builder = WebApplication.CreateBuilder(args);

// Add framework services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

// Add your application services
builder.Services.AddScoped<IUserValidator, UserValidator>();
builder.Services.AddScoped<ICredentialStorage, DatabaseCredentialStorage>();
builder.Services.AddScoped<IFamilyGroupManager, FamilyGroupManager>();
builder.Services.AddScoped<ILogin, UserPassword>();
builder.Services.AddScoped<IRegister, UserPassword>();
builder.Services.AddScoped<ITaskStorage, FileTaskStorage>();
builder.Services.AddScoped<AdminTaskManager>();

builder.Services.AddDbContext<ShoppingListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
