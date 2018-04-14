using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Simplified_School_Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(Simplified_School_Portal.Startup))]
namespace Simplified_School_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
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
