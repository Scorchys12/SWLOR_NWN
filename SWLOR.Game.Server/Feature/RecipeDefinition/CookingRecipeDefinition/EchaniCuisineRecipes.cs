using System.Collections.Generic;
using SWLOR.Game.Server.Service.CraftService;
using SWLOR.Game.Server.Service.SkillService;

namespace SWLOR.Game.Server.Feature.RecipeDefinition.CookingRecipeDefinition
{
    public class EchaniCuisineRecipes : IRecipeListDefinition
    {
        private readonly RecipeBuilder _builder = new();

        public Dictionary<RecipeType, RecipeDetail> BuildRecipes()
        {
            CreateFood(RecipeType.EchaniFoldedDumplings, "esh_dumpling", 6, 1,
                ("esh_pearlgrain", 3), ("esh_fowl_meat", 2), ("esh_silvleaf", 1));
            CreateFood(RecipeType.SilverleafBrothNoodles, "esh_brothnoodle", 10, 1,
                ("esh_pearlgrain", 3), ("esh_silvleaf", 2), ("esh_frostroot", 1));
            CreateFood(RecipeType.FirebrandChiliNoodles, "fire_noodles", 16, 1,
                ("esh_pearlgrain", 3), ("esh_firepepper", 2), ("esh_syrup", 1));
            CreateFood(RecipeType.MoonSteamedBuns, "moon_buns", 21, 2,
                ("esh_pearlgrain", 3), ("esh_fowl_meat", 2), ("esh_mooncap", 1));
            CreateFood(RecipeType.SilverleafFlatbread, "silver_flatbread", 25, 2,
                ("esh_pearlgrain", 3), ("esh_silvleaf", 2), ("esh_fowl_egg", 1));
            CreateFood(RecipeType.EshanHearthpot, "eshan_hearthpot", 30, 2,
                ("esh_fowl_meat", 3), ("esh_frostroot", 2), ("esh_mooncap", 1));
            CreateFood(RecipeType.FrostglazeClaypotRice, "esh_clayrice", 34, 2,
                ("esh_pearlgrain", 3), ("esh_fowl_egg", 2), ("esh_syrup", 1));
            CreateFood(RecipeType.WatchersRicePorridge, "watch_porridge", 38, 2,
                ("esh_pearlgrain", 3), ("esh_fowl_egg", 2), ("esh_frostroot", 1));
            CreateFood(RecipeType.MarbledTeaEggs, "marbled_eggs", 42, 2,
                ("esh_fowl_egg", 3), ("esh_tealeaf", 2), ("esh_frostroot", 1));
            CreateFood(RecipeType.GingerSteamedFish, "ginger_fish", 46, 2,
                ("shining_trout", 3), ("esh_frostroot", 2), ("esh_silvleaf", 1));
            CreateFood(RecipeType.StickyGlazedFowl, "glazed_fowl", 49, 2,
                ("esh_fowl_meat", 3), ("esh_syrup", 2), ("esh_firepepper", 1));

            return _builder.Build();
        }

        private void CreateFood(
            RecipeType recipe,
            string resref,
            int level,
            int enhancementSlots,
            params (string Resref, int Quantity)[] components)
        {
            var recipeBuilder = _builder.Create(recipe, SkillType.Agriculture)
                .Category(RecipeCategoryType.Food)
                .Resref(resref)
                .Level(level)
                .Quantity(1)
                .EnhancementSlots(RecipeEnhancementType.Food, enhancementSlots);

            foreach (var component in components)
            {
                recipeBuilder.Component(component.Resref, component.Quantity);
            }
        }
    }
}
