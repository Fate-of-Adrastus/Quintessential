using System.Collections.Generic;

namespace Quintessential;

public class GlyphRecipe {

    public GlyphRecipe() { }
    public GlyphRecipe(RecipePredicate predicate) { Predicate = predicate; }

    public static Dictionary<Identifier, OrderedDictionary<Identifier, GlyphRecipe>> Recipes = [];

    public Identifier RecipeId;
    public Identifier RecipeGlyphId;
    public RecipePredicate Predicate;
    public object CustomData;

}

public delegate bool RecipePredicate(patch_Sim sim, Part part);
