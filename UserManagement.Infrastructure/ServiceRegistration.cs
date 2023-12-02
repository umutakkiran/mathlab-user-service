using Application.Abstraction.AbsToken;
using Blog.Infrastructure.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UserManagement.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection service)
        {

            service.AddScoped<ITokenHandler, TokenHandler>();


        }

    }
}
