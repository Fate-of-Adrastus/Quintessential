using System.Collections.Generic;

namespace Quintessential;

public class GlyphRecipe {

    public static Dictionary<Identifier, GlyphRecipe> GlyphRecipes = [];

    public Identifier RecipeId;
    public Identifier RecipeGlyphId;
    public RecipePredicate Predicate;
    public object CustomData;

    public delegate bool RecipePredicate(patch_Sim sim, Part part);
}
