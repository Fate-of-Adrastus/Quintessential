using System;

namespace Quintessential;
public static class QApi {

    public static void AddRecipe(this QuintessentialMod mod, GlyphRecipe recipe, Identifier recipeId, Identifier partId) {
        recipe.RecipeId = recipeId;
        recipe.RecipeGlyphId = partId;
        if(!GlyphRecipe.Recipes.TryGetValue(partId, out var glyphRecipes)) {
            glyphRecipes = []; 
            GlyphRecipe.Recipes[partId] = glyphRecipes;
        }
        if (glyphRecipes.ContainsKey(recipeId))
            throw new Exception($"Recipe with id '{recipeId}' cannot be added, recipe already contained in recipes for part '{partId}'");
        glyphRecipes.Add(recipeId, recipe);
    }


    public static void BindGlyphCylce(PartType partType, PartCycleDelegate cycleDelegate) {
        if (partType.Id.namespc == "om") throw new Exception("Cannot bind cycle code to part type from the base game: '" + partType.Id + "'");
        if (((patch_PartType)(object)partType).CycleDelegate != null) throw new Exception("A delegate was already bound to PartType: '" + partType.Id + "'");
        ((patch_PartType)(object)partType).CycleDelegate = cycleDelegate;
    }


    public static bool InvokeAndClear(this RecipePredicate del, patch_Sim sim, Part part) {
        sim.RecipeInputs.Clear();
        sim.RecipeOutputs.Clear();
        return del.Invoke(sim, part);
    }
}
