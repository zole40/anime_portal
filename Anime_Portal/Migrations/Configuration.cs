namespace Anime_Portal.Migrations
{
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Anime_Portal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Anime_Portal.Models.ApplicationDbContext";
        }

        protected override void Seed(Anime_Portal.Models.ApplicationDbContext context)
        {

            if (!context.Roles.Any(r => r.Name == "Administrators"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Administrators" };
                manager.Create(role);
            }
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "zole40@gmail.com",
                    DisplayName = "Béres Zoltán",
                    PictureUrl = "admin.png",
                    Avatar = "adminmini.png",
                    RegisterDate = DateTime.Now,
                    EmailConfirmed = true
                };
                manager.Create(user, "Password1");
                manager.AddToRole(user.Id, "Administrators");
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
