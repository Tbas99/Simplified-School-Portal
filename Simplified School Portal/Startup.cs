using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Simplified_School_Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Host.SystemWeb;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityModel.Client;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;

[assembly: OwinStartupAttribute(typeof(Simplified_School_Portal.Startup))]
namespace Simplified_School_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                //CookieManager = new SystemWebChunkingCookieManager()           
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "i387766-simplified",
                Authority = "https://identity.fhict.nl/connect/authorize",
                RedirectUri = Uri.EscapeUriString("https://localhost:44363/​"),
                ResponseType = "code",
                Scope = "fhict, fhict_personal, openid, profile, email, roles",

                UseTokenLifetime = true,
                SignInAsAuthenticationType = "Cookies"
            });

            /*
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                AuthenticationMode = AuthenticationMode.Passive
            });
            */

            /*
            var options = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get())
                            .UseInMemoryUsers(new List<InMemoryUser>()),

                RequireSsl = false
            };

            app.UseIdentityServer(options);

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Simplified School Portal",

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(StandardScopes.All),

                    RequireSsl = false
                });
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "i387766-simplified",
                Authority = "https://identity.fhict.nl/connect/authorize",
                RedirectUri = "http://localhost:54680/StandardServices/Studentenplein",
                ResponseType = "id_token",
                Scope = "fhict, fhict_personal, openid, profile, email, roles",

                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies"

                
        });

        */
        }

        public void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Check if the user has already been created
            if (!roleManager.RoleExists("Admin"))
            {
                // Create an admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "fhictBeheer";
                user.Email = "admin@fhict.nl";

                string userPassword = "Fhictportalmanager123!";

                var chkUser = UserManager.Create(user, userPassword);

                if (chkUser.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Admin");
                }
            }

        }

    }
}
