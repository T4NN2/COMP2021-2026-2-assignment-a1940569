# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

For the saved/favourite recipes feature, I selected `HashSet<int>` stored in the private `readonly` field `_savedRecipes`. A mathematical set only stores unique values, which makes it ideal for duplicate prevention: calling `_savedRecipes.Add(recipeId)` automatically rejects an ID that is already present and returns `false` without needing a separate preliminary lookup. Because `HashSet<int>` uses an internal hash table, adding (`Add`), removing (`Remove`), and checking membership (`Contains`) compute the recipe ID's hash bucket directly in expected average `O(1)` time instead of scanning through elements sequentially in `O(n)` time. Since no specific output order is required for saved recipe IDs, `GetSavedRecipes()` simply copies `_savedRecipes` into a new `List<int>` snapshot so external callers cannot mutate the internal `HashSet<int>`.

## 2. Integration

All Part B methods are implemented directly inside `RecipeManager` alongside the five Part A collection fields without altering any existing Part A behaviour or public interface signatures. The LINQ search and reporting methods (`SearchByTitle`, `SearchByIngredient`, and `GetHighestProteinRecipes`) query `_recipes.Values` from the Part A `Dictionary<int, Recipe>` catalogue and materialise new `List<Recipe>` results, leaving the stored `Recipe` instances, ingredient lists, and `NutritionInfo` values untouched. Similarly, `AddSavedRecipe` validates against the Part A catalogue by checking `_recipes.ContainsKey(recipeId)` before inserting into `_savedRecipes`, ensuring that only valid recipe IDs present in the catalogue can be saved.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | HashSet<int> | Average O(1) | Hash-based bucket lookup on the recipe ID rather than scanning saved IDs sequentially. |
