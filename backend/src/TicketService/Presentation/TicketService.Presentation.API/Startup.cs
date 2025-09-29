using TicketService.Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace TicketService.Presentation.API;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<TicketDbContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("TicketDb"));
            options.UseLazyLoadingProxies();
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi();
        }
        else
        {
            app.UseHsts();
        }


        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}