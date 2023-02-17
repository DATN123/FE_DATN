using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BackpackingTourism.Extentions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services, ConfigurationManager configuration)
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var entryAssembly = Assembly.GetEntryAssembly();
			services.AddAutoMapper(configuration =>
			{
				configuration.AddExpressionMapping();
			}, executingAssembly, entryAssembly);
            //services.AddDbContext<BEFUNContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));

            //services.AddScoped<IOrganizationLevelRepository, OrganizationLevelRepository>();
            services.AddHttpContextAccessor();
			services.AddHttpClient();
			return services;
		}
	}
}
