using FluentValidation;
using InControll.Application;
using InControll.Application.Validators;
using InControll.Domain;
using InControll.Infrastrucuture;
using InControll.Infrastrucuture.Data.MongoDB;
using InControll.Infrastrucuture.Repositories;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InControll.CrossCutting;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //SQL Server config connection
        services.AddDbContext<PaymentDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        //MongoBB config connection
        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection");
            var databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");

            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("MongoDB ConnectionString is not configured.");
            if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException("MongoDB DatabaseName is not configured.");

            return new MongoDbContext(connectionString, databaseName);
        });
        services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();
        
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessPaymentCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(ProcessPaymentCommandValidator).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}