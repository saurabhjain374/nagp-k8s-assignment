using EmployeeApi.Repositories;
using Dapper;

namespace EmployeeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Repository
            builder.Services.AddScoped<EmployeeRepository>();

            var app = builder.Build();
            // Add health check endpoint
            app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
