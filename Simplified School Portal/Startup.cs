using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Simplified_School_Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IdentityModel.Tokens.Jwt;

[assembly: OwinStartupAttribute(typeof(Simplified_School_Portal.Startup))]
namespace Simplified_School_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();

            /*
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "i387766-simplified",
                Authority = "https://identity.fhict.nl/connect/authorize",
                RedirectUri = "http://i387766.venus.fhict.nl/StandardServices/Studentenplein", 
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
