using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Persistence.context;
using Domain.Entities.user;


namespace UserManagement.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection service)
        {


            //alttaki 3 satırda extensions.configuration ve extensions.configuration.json kütüphanelerini kullanarak appsettings.json ın dosya yoluna ulaşıp oradaki connectionstring i kullanmaya hazır hale getiririz.
            ConfigurationManager configurationManager = new();
            configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../UserManagement.Presentation"));
            configurationManager.AddJsonFile("appsettings.json");

            service.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configurationManager.GetConnectionString("DbCon")));
            service.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        }



    }
}
