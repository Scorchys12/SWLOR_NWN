using System.Collections.Generic;
using SWLOR.Game.Server.Service.CraftService;
using SWLOR.Game.Server.Service.PerkService;
using SWLOR.Game.Server.Service.SkillService;

namespace SWLOR.Game.Server.Feature.RecipeDefinition.BiotechnologyRecipeDefinition
{
    public class BiologicalComponentRecipes : IRecipeListDefinition
    {
        private readonly RecipeBuilder _builder = new();

        public Dictionary<RecipeType, RecipeDetail> BuildRecipes()
        {
            Components();
            return _builder.Build();
        }

        private void Components()
        {
            _builder.Create(RecipeType.PlantCulture, SkillType.Biotechnology)
                .Category(RecipeCategoryType.BiologicalComponent).Resref("bio_plantcult").Level(5).Quantity(1)
                .RequirementPerk(PerkType.BiologicalAnalysis, 1)
                .Component("herb_m", 2).Component("night_flowerlily", 1);

            _builder.Create(RecipeType.FungalCulture, SkillType.Biotechnology)
                .Category(RecipeCategoryType.BiologicalComponent).Resref("bio_fungalcult").Level(8).Quantity(1)
                .RequirementPerk(PerkType.BiologicalAnalysis, 1)
                .Component("mushroom", 3).Component("herb_question", 1);

            _builder.Create(RecipeType.ProteinExtract, SkillType.Biotechnology)
                .Category(RecipeCategoryType.BiologicalComponent).Resref("bio_proteinext").Level(12).Quantity(1)
                .RequirementPerk(PerkType.BiologicalAnalysis, 2)
                .Component("wild_meat", 2).Component("mynock_meat", 1);

            _builder.Create(RecipeType.AntivenomBase, SkillType.Biotechnology)
                .Category(RecipeCategoryType.BiologicalComponent).Resref("bio_antivenom").Level(18).Quantity(1)
                .RequirementPerk(PerkType.BiologicalAnalysis, 2)
                .Component("mserp_meat", 1).Component("herb_digested", 2);

            _builder.Create(RecipeType.IncubationMedium, SkillType.Biotechnology)
                .Category(RecipeCategoryType.BiologicalComponent).Resref("bio_incubmed").Level(25).Quantity(1)
                .RequirementPerk(PerkType.AdvancedBiologicalProcessing, 1)
                .Component("bio_proteinext", 1).Component("bio_plantcult", 1).Component("hydrolase_orange", 1);
        }
    }
}
