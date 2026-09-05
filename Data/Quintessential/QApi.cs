using System;

namespace Quintessential;
public static class QApi {

    public static void AddRecipe(this QuintessentialMod mod, GlyphRecipe recipe, string recipeId) {
        var id = new Identifier(recipeId);
        recipe.RecipeId = id;
        if (GlyphRecipe.GlyphRecipes.ContainsKey(id)) throw new Exception($"Recipe with id '{recipeId}' cannot be added, recipe already contained in recipes with full id '{id}'");
        GlyphRecipe.GlyphRecipes.Add(id, recipe);
    }

    public static bool InvokeAndClear(this GlyphRecipe.RecipePredicate del, patch_Sim sim, Part part) {
        sim.RecipeInputs.Clear();
        sim.RecipeOutputs.Clear();
        return del.Invoke(sim, part);
    }
}
