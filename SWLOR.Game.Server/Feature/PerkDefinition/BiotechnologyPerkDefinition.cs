using System.Collections.Generic;
using SWLOR.Game.Server.Service.PerkService;
using SWLOR.Game.Server.Service.SkillService;

namespace SWLOR.Game.Server.Feature.PerkDefinition
{
    public class BiotechnologyPerkDefinition : IPerkListDefinition
    {
        private readonly PerkBuilder _builder = new();

        public Dictionary<PerkType, PerkDetail> BuildPerks()
        {
            BiologicalAnalysis();
            LaboratoryTechniques();
            SamplePreservation();
            AdvancedBiologicalProcessing();
            ResearchProjects();

            return _builder.Build();
        }

        private void BiologicalAnalysis()
        {
            _builder.Create(PerkCategoryType.Biotechnology, PerkType.BiologicalAnalysis)
                .Name("Biological Analysis")
                .AddPerkLevel().Description("Grants access to tier 1 biological processing recipes.").Price(1)
                .AddPerkLevel().Description("Grants access to tier 2 biological processing recipes.").Price(1).RequirementSkill(SkillType.Biotechnology, 10)
                .AddPerkLevel().Description("Grants access to tier 3 biological processing recipes.").Price(2).RequirementSkill(SkillType.Biotechnology, 20)
                .AddPerkLevel().Description("Grants access to tier 4 biological processing recipes.").Price(3).RequirementSkill(SkillType.Biotechnology, 30)
                .AddPerkLevel().Description("Grants access to tier 5 biological processing recipes.").Price(3).RequirementSkill(SkillType.Biotechnology, 40);
        }

        private void LaboratoryTechniques()
        {
            _builder.Create(PerkCategoryType.Biotechnology, PerkType.LaboratoryTechniques)
                .Name("Laboratory Techniques")
                .AddPerkLevel().Description("Improves biological processing efficiency through better lab handling.").Price(1).RequirementSkill(SkillType.Biotechnology, 5)
                .AddPerkLevel().Description("Further improves biological processing efficiency through precise lab technique.").Price(2).RequirementSkill(SkillType.Biotechnology, 25);
        }

        private void SamplePreservation()
        {
            _builder.Create(PerkCategoryType.Biotechnology, PerkType.SamplePreservation)
                .Name("Sample Preservation")
                .AddPerkLevel().Description("Gives a small chance to preserve a biological sample during processing.").Price(1).RequirementSkill(SkillType.Biotechnology, 8)
                .AddPerkLevel().Description("Increases the chance to preserve a biological sample during processing.").Price(2).RequirementSkill(SkillType.Biotechnology, 28);
        }

        private void AdvancedBiologicalProcessing()
        {
            _builder.Create(PerkCategoryType.Biotechnology, PerkType.AdvancedBiologicalProcessing)
                .Name("Advanced Biological Processing")
                .AddPerkLevel().Description("Unlocks advanced biological component processing.").Price(2).RequirementSkill(SkillType.Biotechnology, 20)
                .AddPerkLevel().Description("Unlocks expert biological component processing.").Price(3).RequirementSkill(SkillType.Biotechnology, 40);
        }

        private void ResearchProjects()
        {
            _builder.Create(PerkCategoryType.Biotechnology, PerkType.BiotechnologyResearchProjects)
                .Name("Research Projects")
                .AddPerkLevel().Description("Allows one additional Biotechnology processing or research project to be managed.").Price(2).RequirementSkill(SkillType.Biotechnology, 15)
                .AddPerkLevel().Description("Allows two additional Biotechnology processing or research projects to be managed.").Price(3).RequirementSkill(SkillType.Biotechnology, 35);
        }
    }
}
