using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace Easy_Peasy_ShoppingList.Services.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "username");
            
            if (string.IsNullOrEmpty(username))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            var identity = new ClaimsIdentity(claims, "LocalStorage");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
    }
} 