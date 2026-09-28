using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingItems = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _cookingInstructions = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);

        foreach (var recipe in recipes)
        {
            if (recipe is null || recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Each recipe must have a positive ID and a non-blank title.", nameof(recipes));
            }

            if (!_recipes.TryAdd(recipe.Id, recipe))
            {
                throw new ArgumentException("Recipe IDs must be unique.", nameof(recipes));
            }
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingItems.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }

        return _recipes.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId) =>
        _recipes.TryGetValue(recipeId, out var recipe) ? recipe : null;

    public bool RemoveRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        return _recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return 0;
        }

        _shoppingItems.AddRange(recipe.Ingredients);
        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList() =>
        new List<string>(_shoppingItems);

    public void ClearShoppingList() =>
        _shoppingItems.Clear();

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }

        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        var recipeId = _removedRecipes.Pop();
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe() =>
        _removedRecipes.Count == 0 ? null : _removedRecipes.Peek();

    public IReadOnlyList<int> GetCookingPlan() =>
        new List<int>(_cookingPlan);

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
