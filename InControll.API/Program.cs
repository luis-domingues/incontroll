using InControll.CrossCutting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (FluentValidation.ValidationException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/problem+json";
        var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "One or more validation errors occurred.",
            status = 400,
            detail = "See the errors property for details.",
            errors = errors
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
    catch (ApplicationException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/problem+json";
        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "Business rule violation.",
            status = 400,
            detail = ex.Message
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
    catch (Exception ex)
    {
        // tratamente genérico
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";
        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "An unexpected error occurred.",
            status = 500,
            detail = ex.Message
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
});

app.MapControllers();

app.Run();