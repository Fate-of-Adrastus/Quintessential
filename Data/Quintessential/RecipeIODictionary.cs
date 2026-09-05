using System;
using System.Collections.Generic;

namespace Quintessential;

public class RecipeInputDictionary<TKey, TValue>(Sim sim) : RecipeIODictionary<TKey, TValue>(sim) where TValue : IRecipeInput {
    protected override string Name { get; init; } = "inputs";
}
public class RecipeOutputDictionary<TKey, TValue>(Sim sim) : RecipeIODictionary<TKey, TValue>(sim) where TValue : IRecipeOutput {
    protected override string Name { get; init; } = "outputs";
}

public abstract class RecipeIODictionary<TKey, TValue>(Sim sim) : Dictionary<TKey, TValue> {
    protected abstract string Name { get; init; }
    public readonly Sim sim = sim;
    public GlyphRecipe recipe;

    public new TValue this[TKey key] {
        get {
            try {
                return base[key];
            } catch (KeyNotFoundException e) {
                throw new Exception($"Failed to find IO for recipe '{recipe?.RecipeId ?? "null"}' in '{Name}' at '{key}'", e);
            }
        }
        set {
            base[key] = value;
        }
    }
}
