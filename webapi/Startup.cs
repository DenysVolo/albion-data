public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
        services.AddScoped<IQueryCatalogue, QueryCatalogue>();

        services.AddSingleton<ILocationCatalogue, LocationCatalogue>();
        services.AddSingleton<IItemCatalogue, ItemCatalogue>();
        services.AddSingleton<ISessionDatabase, InMemorySessionDatabase>();

        services.AddControllers();

        services.AddLogging();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
