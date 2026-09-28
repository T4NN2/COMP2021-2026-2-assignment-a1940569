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
    public void AddIngredientsToShoppingList_ValidRecipeReturnsIngredientCount()
    {
        var manager = CreateManager();

        Assert.Equal(2, manager.AddIngredientsToShoppingList(20));
    }

    [Fact]
    public void AddIngredientsToShoppingList_PreservesIngredientOrder()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(20);

        Assert.Equal(new[] { "2 carrots", "1 onion" }, manager.GetShoppingList());
    }

    [Fact]
    public void ShoppingItemCount_UpdatesWhenIngredientsAreAdded()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(20);

        Assert.Equal(2, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_MissingRecipeReturnsZeroAndChangesNothing()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.AddIngredientsToShoppingList(30));
        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_TwiceAppendsDuplicates()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(20);
        manager.AddIngredientsToShoppingList(20);

        Assert.Equal(new[] { "2 carrots", "1 onion", "2 carrots", "1 onion" }, manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_EmptiesListAndResetsCount()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(20);

        manager.ClearShoppingList();

        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void ClearShoppingList_DoesNotRemoveRecipes()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(20);

        manager.ClearShoppingList();

        Assert.Equal(2, manager.RecipeCount);
        Assert.NotNull(manager.FindRecipe(20));
    }

    [Fact]
    public void GetShoppingList_ReturnsSnapshotThatCannotModifyManager()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(20);
        var shoppingList = manager.GetShoppingList();

        var mutableSnapshot = Assert.IsAssignableFrom<IList<string>>(shoppingList);
        mutableSnapshot.Add("extra item");

        Assert.Equal(2, manager.ShoppingItemCount);
        Assert.Equal(new[] { "2 carrots", "1 onion" }, manager.GetShoppingList());
    }

    [Fact]
    public void AddRecipeToCookingPlan_ExistingRecipesPreserveInsertionOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));

        Assert.Equal(new[] { 10, 20 }, manager.GetCookingPlan());
        Assert.Equal(2, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_MissingRecipeReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.AddRecipeToCookingPlan(30));
        Assert.Empty(manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_DuplicateRecipeReturnsFalse()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_SuccessUpdatesPlanAndStack()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Empty(manager.GetCookingPlan());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_MissingRecipeDoesNotChangeStack()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void PeekLastRemovedRecipe_EmptyStackReturnsNull()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_EmptyStackReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_UsesLifoOrder()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_AppendsRecipeToPlanEnd()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void GetCookingPlan_ReturnsIsolatedSnapshot()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        var cookingPlan = manager.GetCookingPlan();

        var mutableSnapshot = Assert.IsAssignableFrom<IList<int>>(cookingPlan);
        mutableSnapshot.Add(20);

        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipe_RefusesRecipeCurrentlyInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
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
                Title = "Recipe B",
                Ingredients = new() { "2 carrots", "1 onion" }
            }
        });
    }
}
