using UserApi.Service;
using UserApi.Data;
using UserApi.Repository;
using UserApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Collect all validation error messages into one list
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = new ApiResponse(
            statusCode: 400,
            status: false,
            message: "Validation failed",
            error: string.Join(" | ", errors)
        );

        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false;
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = 
        System.Text.Json.Serialization.JsonIgnoreCondition.Never;
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.MapControllers();
app.Run();


