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
}

