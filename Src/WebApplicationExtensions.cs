using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

using RichillCapital.Studio.Web.Components;

namespace RichillCapital.Studio.Web;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "9999";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "http://localhost:10000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "9999";
                options.ClientSecret = "secret";

                // code flow + PKCE (PKCE is turned on by default)
                options.ResponseType = "code";
                options.UsePkce = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");

                // not mapped by default
                options.ClaimActions.MapJsonKey("picture", "picture");

                // keeps id_token smaller
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role",
                };
            });

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .RequireAuthorization();

        return app;
    }
}