using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using UserAuthentication;
using TaskManager;
using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Services;

var builder = WebApplication.CreateBuilder(args);

// Add framework services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

// Add your application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserValidator, UserValidator>();
builder.Services.AddScoped<ICredentialStorage, DatabaseCredentialStorage>();
builder.Services.AddScoped<IFamilyGroupManager, FamilyGroupManager>();
builder.Services.AddScoped<IFamily>(provider =>
{
    var credentialStorage = provider.GetRequiredService<ICredentialStorage>();
    var userCredentials = credentialStorage.LoadCredentials();
    return new GetFamilies(userCredentials);
});
builder.Services.AddScoped<ILogin, LoginManager>();
builder.Services.AddScoped<IRegister, RegisterManager>();
builder.Services.AddScoped<ITaskStorage, DatabaseTaskStorage>();
builder.Services.AddScoped<AdminTaskManager>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddDbContext<ShoppingListDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

