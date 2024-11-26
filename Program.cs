using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using UserAuthentication;
using TaskManager;

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
builder.Services.AddScoped<ICredentialStorage, FileCredentialStorage>();
builder.Services.AddScoped<IFamilyGroupManager, FamilyGroupManager>();
builder.Services.AddScoped<ILogin, UserPassword>();
builder.Services.AddScoped<IRegister, UserPassword>();
builder.Services.AddScoped<ITaskStorage, FileTaskStorage>();
builder.Services.AddScoped<AdminTaskManager>();

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
