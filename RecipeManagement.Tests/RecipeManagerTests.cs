using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void AddRecipe_ValidRecipeIncreasesCount()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipe(new Recipe { Id = 30, Title = "Recipe C" }));
        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_DuplicateIdReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = 10, Title = "Another Recipe" }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_NonPositiveIdReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = 0, Title = "Invalid Recipe" }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_BlankTitleReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = 30, Title = "   " }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_NullRecipeThrowsArgumentNullException()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    public void FindRecipe_MissingIdReturnsNull()
    {
        var manager = CreateManager();

        Assert.Null(manager.FindRecipe(30));
    }

    [Fact]
    public void RemoveRecipe_ExistingRecipeSucceeds()
    {
        var manager = CreateManager();

        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_MissingRecipeReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveRecipe(30));
    }

    [Fact]
    public void Constructor_DuplicateIdsThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]
        {
            new Recipe { Id = 10, Title = "Recipe A" },
            new Recipe { Id = 10, Title = "Recipe B" }
        }));
    }

    [Fact]
    public void Constructor_NonPositiveIdThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]
        {
            new Recipe { Id = 0, Title = "Invalid Recipe" }
        }));
    }

    [Fact]
    public void Constructor_BlankTitleThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]
        {
            new Recipe { Id = 10, Title = "   " }
        }));
    }

    [Fact]
    public void Constructor_NullRecipesThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
