using LMS.Application.Services.Account;
using LMS.Application.Services.AI;
using LMS.Application.Services.Book;
using LMS.Application.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IAiQueryService, AiQueryService>();
            services.AddHttpClient<IOllamaQueryService, OllamaQueryService>();

            return services;
        }
    }
}
