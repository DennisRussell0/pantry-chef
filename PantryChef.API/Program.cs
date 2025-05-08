using Microsoft.EntityFrameworkCore;
using PantryChef.API.Data;
using PantryChef.API.Services;
using PantryChef.API.Interfaces;
using PantryChef.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<PantryChefDbContext>(options =>
{
    // Configure the database connection string from appsettings.json
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Disable sensitive data logging to avoid exposing sensitive information in logs
    options.EnableSensitiveDataLogging(false);

    // Disable logging entirely to improve performance during large operations like seeding
    options.LogTo(Console.WriteLine, LogLevel.None);
});

// Add controller support for handling HTTP requests
builder.Services.AddControllers();

// Register services in the dependency injection container
// IngredientParser: Used for parsing ingredients during seeding
builder.Services.AddScoped<IngredientParser>();

// CsvSeeder: Used for seeding the database with data from the CSV file
builder.Services.AddScoped<CsvSeeder>();

// Repositories: Abstract database access logic for recipes and ingredients
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();

// RecipeService: Handles business logic for recipes (e.g., matching recipes)
builder.Services.AddScoped<RecipeService>();

// Add Swagger for API documentation and testing
builder.Services.AddEndpointsApiExplorer(); // Enables API endpoint discovery for Swagger
builder.Services.AddSwaggerGen(); // Adds Swagger generator for API documentation

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Replace with your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Activate Swagger in development mode
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
app.UseAuthorization();

// Map controller endpoints to handle API requests
app.MapControllers();

// Temporary: Seed the database with data from the CSV file
using (var scope = app.Services.CreateScope())
{
    // Resolve the CsvSeeder service from the dependency injection container
    var seeder = scope.ServiceProvider.GetRequiredService<CsvSeeder>();

    // Run the seeding process asynchronously
    // await seeder.SeedAsync(); // Comment this line to disable seeding
}

app.Run();
