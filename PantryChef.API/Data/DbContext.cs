// Configure relations (e.g. HasMany().WithMany() via RecipeIngredient)

using Microsoft.EntityFrameworkCore;
using PantryChef.API.Models;

namespace PantryChef.API.Data;

public class PantryChefDbContext : DbContext
{
    public PantryChefDbContext(DbContextOptions<PantryChefDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure composite primary key for join table
        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        // Relationer
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId);
    }
}