using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Services;
using PantryChef.API.Interfaces;
using PantryChef.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context with PostgreSQL
builder.Services.AddDbContext<PantryChefDbContext>(options =>
{
    // Use the connection string from appsettings.json
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Disable sensitive data logging to avoid exposing sensitive information in logs
    options.EnableSensitiveDataLogging(false);

    // Disable logging entirely to improve performance during large operations like seeding
    options.LogTo(Console.WriteLine, LogLevel.None);
});

// Add controller support for handling HTTP requests
builder.Services.AddControllers();

// Register services and repositories in the dependency injection container
builder.Services.AddScoped<IngredientParser>(); // Service for parsing ingredients
builder.Services.AddScoped<CsvSeeder>(); // Service for seeding the database with CSV data
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>(); // Recipe repository
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>(); // Ingredient repository
builder.Services.AddScoped<RecipeService>(); // Service for recipe-related business logic

// Add Swagger for API documentation and testing
builder.Services.AddEndpointsApiExplorer(); // Enables API endpoint discovery for Swagger
builder.Services.AddSwaggerGen(); // Adds Swagger generator for API documentation

// Configure CORS to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger middleware
    app.UseSwaggerUI(); // Enable Swagger UI for testing the API
}

// Enable CORS
app.UseCors();

// Enable HTTPS redirection for secure communication
app.UseHttpsRedirection();

// Enable authorization middleware (if applicable)
// app.UseAuthorization();

// Map controller endpoints to handle API requests
app.MapControllers();

// Temporary: Seed the database with data from the CSV file
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CsvSeeder>();
    // Uncomment the following line to seed the database
    await seeder.SeedAsync();
}

app.Run();