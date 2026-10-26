using System.Collections.Generic;
using System.Linq;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class RecipeManagerPartBTests
{
    [Fact]
    public void SearchByTitle_PartialQuery_ReturnsMatchingRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("Chick");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Id == 1);
        Assert.Contains(results, r => r.Id == 2);
    }

    [Fact]
    public void SearchByTitle_DifferentCapitalization_MatchesCaseInsensitively()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("cHiCkEn");

        Assert.Equal(2, results.Count);
        Assert.Equal(new[] { 1, 2 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByTitle_MatchingQuery_ExcludesUnrelatedRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("Chicken");

        Assert.DoesNotContain(results, r => r.Id == 3);
        Assert.DoesNotContain(results, r => r.Title == "Blueberry Pancakes");
    }

    [Fact]
    public void SearchByTitle_NonMatchingQuery_ReturnsEmptyCollection()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("Lasagna");

        Assert.Empty(results);
    }

    [Fact]
    public void SearchByTitle_EmptyQuery_ReturnsAllRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle(string.Empty);

        Assert.Equal(3, results.Count);
        Assert.Equal(new[] { 1, 2, 3 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByTitle_WhitespaceOnlyQuery_ReturnsAllRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("   \t  ");

        Assert.Equal(3, results.Count);
        Assert.Equal(new[] { 1, 2, 3 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByTitle_MatchingQuery_DoesNotReturnDuplicateRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("a");
        var ids = results.Select(r => r.Id).ToList();

        Assert.Equal(ids.Distinct().Count(), ids.Count);
    }

    [Fact]
    public void SearchByTitle_ExecutingSearches_LeavesOriginalRecipeCountUnchanged()
    {
        var manager = CreateManager();

        _ = manager.SearchByTitle("Chicken");
        _ = manager.SearchByTitle("Lasagna");
        _ = manager.SearchByTitle(string.Empty);

        Assert.Equal(3, manager.RecipeCount);
        Assert.Equal("Roasted CHICKEN Pasta", manager.FindRecipe(2)?.Title);
    }

    [Fact]
    public void SearchByIngredient_PartialQuery_ReturnsMatchingRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("chick");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Id == 1);
        Assert.Contains(results, r => r.Id == 2);
    }

    [Fact]
    public void SearchByIngredient_DifferentCapitalization_MatchesCaseInsensitively()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("cHiCkEn");

        Assert.Equal(2, results.Count);
        Assert.Equal(new[] { 1, 2 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByIngredient_MatchingQuery_ExcludesUnrelatedRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("chicken");

        Assert.DoesNotContain(results, r => r.Id == 3);
        Assert.DoesNotContain(results, r => r.Title == "Blueberry Pancakes");
    }

    [Fact]
    public void SearchByIngredient_NonMatchingQuery_ReturnsEmptyCollection()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("avocado");

        Assert.Empty(results);
    }

    [Fact]
    public void SearchByIngredient_EmptyQuery_ReturnsAllRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient(string.Empty);

        Assert.Equal(3, results.Count);
        Assert.Equal(new[] { 1, 2, 3 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByIngredient_WhitespaceOnlyQuery_ReturnsAllRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("   \t  ");

        Assert.Equal(3, results.Count);
        Assert.Equal(new[] { 1, 2, 3 }, results.Select(r => r.Id).OrderBy(id => id));
    }

    [Fact]
    public void SearchByIngredient_RecipeWithMultipleMatchingIngredients_AppearsOnlyOnce()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("chicken");

        Assert.Single(results, r => r.Id == 1);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void SearchByIngredient_ExecutingSearches_DoesNotChangeRecipeCount()
    {
        var manager = CreateManager();

        _ = manager.SearchByIngredient("chicken");
        _ = manager.SearchByIngredient("avocado");
        _ = manager.SearchByIngredient(string.Empty);

        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void SearchByIngredient_ExecutingSearches_LeavesIngredientsUnchanged()
    {
        var manager = CreateManager();

        _ = manager.SearchByIngredient("CHICKEN");
        _ = manager.SearchByIngredient("GARLIC");

        Assert.Equal(
            new[] { "500g chicken breast", "2 garlic cloves", "1 cup chicken stock" },
            manager.FindRecipe(1)?.Ingredients);
        Assert.Equal(
            new[] { "300g pasta", "400g CHICKEN thigh" },
            manager.FindRecipe(2)?.Ingredients);
    }

    [Fact]
    public void GetHighestProteinRecipes_ValidCount_ReturnsRecipesOrderedByDescendingProtein()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(3);

        Assert.Equal(new[] { 20, 30, 10 }, results.Select(r => r.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes_RequestedCountSmallerThanQualifying_LimitsResult()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(2);

        Assert.Equal(2, results.Count);
        Assert.Equal(new[] { 20, 30 }, results.Select(r => r.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes_RecipeWithNullNutrition_IsExcludedWithoutThrowing()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(10);

        Assert.DoesNotContain(results, r => r.Id == 40);
    }

    [Fact]
    public void GetHighestProteinRecipes_RecipeWithNullProteinG_IsExcludedWithoutThrowing()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(10);

        Assert.DoesNotContain(results, r => r.Id == 50);
    }

    [Fact]
    public void GetHighestProteinRecipes_ZeroCount_ReturnsEmptyCollection()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(0);

        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_NegativeCount_ReturnsEmptyCollection()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(-5);

        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_CountLargerThanAvailable_ReturnsEveryQualifyingRecipe()
    {
        var manager = CreateProteinManager();

        var results = manager.GetHighestProteinRecipes(50);

        Assert.Equal(3, results.Count);
        Assert.Equal(new[] { 20, 30, 10 }, results.Select(r => r.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes_RunningReport_LeavesOriginalRecipeCountUnchanged()
    {
        var manager = CreateProteinManager();

        _ = manager.GetHighestProteinRecipes(2);
        _ = manager.GetHighestProteinRecipes(0);
        _ = manager.GetHighestProteinRecipes(10);

        Assert.Equal(5, manager.RecipeCount);
    }

    [Fact]
    public void GetHighestProteinRecipes_RunningReport_LeavesNutritionValuesUnchanged()
    {
        var manager = CreateProteinManager();

        _ = manager.GetHighestProteinRecipes(3);

        Assert.Equal(5.5, manager.FindRecipe(10)?.Nutrition?.ProteinG);
        Assert.Equal(42.0, manager.FindRecipe(20)?.Nutrition?.ProteinG);
        Assert.Equal(18.2, manager.FindRecipe(30)?.Nutrition?.ProteinG);
        Assert.Null(manager.FindRecipe(40)?.Nutrition);
        Assert.NotNull(manager.FindRecipe(50)?.Nutrition);
        Assert.Null(manager.FindRecipe(50)?.Nutrition?.ProteinG);
    }

    [Fact]
    public void AddSavedRecipe_ExistingRecipeId_ReturnsTrue()
    {
        var manager = CreateManager();

        Assert.True(manager.AddSavedRecipe(1));
    }

    [Fact]
    public void AddSavedRecipe_MissingRecipeId_ReturnsFalseAndDoesNotSave()
    {
        var manager = CreateManager();

        Assert.False(manager.AddSavedRecipe(999));
        Assert.False(manager.IsRecipeSaved(999));
        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void AddSavedRecipe_DuplicateRecipeId_ReturnsFalseOnSecondAttemptAndStoresOnce()
    {
        var manager = CreateManager();

        Assert.True(manager.AddSavedRecipe(1));
        Assert.False(manager.AddSavedRecipe(1));
        Assert.Single(manager.GetSavedRecipes(), 1);
    }

    [Fact]
    public void IsRecipeSaved_ReturnsTrueWhenSavedAndFalseWhenUnsavedOrMissing()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(2);

        Assert.True(manager.IsRecipeSaved(2));
        Assert.False(manager.IsRecipeSaved(1));
        Assert.False(manager.IsRecipeSaved(999));
    }

    [Fact]
    public void GetSavedRecipes_AfterSavingRecipe_ContainsSavedRecipeId()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(2);

        var saved = manager.GetSavedRecipes();

        Assert.Contains(2, saved);
        Assert.Single(saved);
    }

    [Fact]
    public void RemoveSavedRecipe_CurrentlySavedRecipe_ReturnsTrueAndRemovesFromSaved()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(1);

        Assert.True(manager.RemoveSavedRecipe(1));
        Assert.False(manager.IsRecipeSaved(1));
        Assert.DoesNotContain(1, manager.GetSavedRecipes());
    }

    [Fact]
    public void RemoveSavedRecipe_UnsavedOrMissingRecipeId_ReturnsFalse()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveSavedRecipe(1));
        Assert.False(manager.RemoveSavedRecipe(999));
    }

    [Fact]
    public void GetSavedRecipes_WhenNoneSaved_ReturnsEmptyCollection()
    {
        var manager = CreateManager();

        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void GetSavedRecipes_MultipleSavedRecipes_ReturnsAllSavedIdsRegardlessOfOrder()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(3);
        manager.AddSavedRecipe(1);

        var saved = manager.GetSavedRecipes();

        Assert.Equal(2, saved.Count);
        Assert.Equal(new[] { 1, 3 }, saved.OrderBy(id => id));
    }

    [Fact]
    public void GetSavedRecipes_ReturnsSnapshotThatCannotMutateInternalCollection()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(1);
        var savedSnapshot = manager.GetSavedRecipes();

        var mutableList = Assert.IsAssignableFrom<IList<int>>(savedSnapshot);
        mutableList.Add(3);

        Assert.False(manager.IsRecipeSaved(3));
        Assert.Equal(new[] { 1 }, manager.GetSavedRecipes());
    }

    [Fact]
    public void PartBSearchAndSavedWorkflow_IntegratesWithPartAShoppingPlanAndCooking()
    {
        var manager = CreateManager();

        var searchResults = manager.SearchByTitle("pasta");
        var matchedRecipe = Assert.Single(searchResults);
        var recipeId = matchedRecipe.Id;
        Assert.Equal(2, recipeId);

        Assert.True(manager.AddSavedRecipe(recipeId));
        Assert.True(manager.IsRecipeSaved(recipeId));
        Assert.Contains(recipeId, manager.GetSavedRecipes());

        Assert.True(manager.RemoveSavedRecipe(recipeId));
        Assert.False(manager.IsRecipeSaved(recipeId));
        Assert.DoesNotContain(recipeId, manager.GetSavedRecipes());

        Assert.Equal(2, manager.AddIngredientsToShoppingList(recipeId));
        Assert.Equal(2, manager.ShoppingItemCount);
        Assert.Equal(new[] { "300g pasta", "400g CHICKEN thigh" }, manager.GetShoppingList());

        Assert.True(manager.AddRecipeToCookingPlan(recipeId));
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(new[] { recipeId }, manager.GetCookingPlan());

        Assert.True(manager.StartCooking(recipeId));
        Assert.Equal(2, manager.PendingInstructionCount);
        Assert.Equal("Boil pasta", manager.PeekNextInstruction());
        Assert.Equal("Boil pasta", manager.CompleteNextInstruction());
        Assert.Equal("Roast chicken", manager.PeekNextInstruction());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Garlic Chicken Curry",
                Ingredients = new() { "500g chicken breast", "2 garlic cloves", "1 cup chicken stock" },
                Instructions = new() { "Simmer curry" }
            },
            new Recipe
            {
                Id = 2,
                Title = "Roasted CHICKEN Pasta",
                Ingredients = new() { "300g pasta", "400g CHICKEN thigh" },
                Instructions = new() { "Boil pasta", "Roast chicken" }
            },
            new Recipe
            {
                Id = 3,
                Title = "Blueberry Pancakes",
                Ingredients = new() { "1 cup flour", "1 cup blueberries" },
                Instructions = new() { "Flip pancakes" }
            }
        });
    }

    private static RecipeManager CreateProteinManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Garden Salad",
                Nutrition = new NutritionInfo { ProteinG = 5.5 }
            },
            new Recipe
            {
                Id = 20,
                Title = "Grilled Salmon",
                Nutrition = new NutritionInfo { ProteinG = 42.0 }
            },
            new Recipe
            {
                Id = 30,
                Title = "Lentil Soup",
                Nutrition = new NutritionInfo { ProteinG = 18.2 }
            },
            new Recipe
            {
                Id = 40,
                Title = "Mystery Smoothie",
                Nutrition = null
            },
            new Recipe
            {
                Id = 50,
                Title = "Fruit Bowl",
                Nutrition = new NutritionInfo { ProteinG = null, CaloriesKcal = 120.0 }
            }
        });
    }
}

