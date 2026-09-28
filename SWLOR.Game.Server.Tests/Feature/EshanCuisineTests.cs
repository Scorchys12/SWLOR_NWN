using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using SWLOR.Game.Server.Feature.LootTableDefinition;
using SWLOR.Game.Server.Feature.RecipeDefinition.CookingRecipeDefinition;
using SWLOR.Game.Server.Feature.SpawnDefinition;
using SWLOR.Game.Server.Service.CraftService;
using SWLOR.Game.Server.Service.SkillService;

namespace SWLOR.Game.Server.Tests.Feature;

public class EshanCuisineTests
{
    [Test]
    public void EchaniCuisine_UsesRegionalIngredients()
    {
        var recipes = new EchaniCuisineRecipes().BuildRecipes();
        var regionalIngredients = new HashSet<string>
        {
            "esh_firepepper", "esh_fowl_egg", "esh_fowl_meat", "esh_frostroot",
            "esh_mooncap", "esh_pearlgrain", "esh_silvleaf", "esh_syrup", "esh_tealeaf"
        };

        recipes.Should().HaveCount(11);
        recipes.Values.Should().OnlyContain(recipe =>
            recipe.Skill == SkillType.Agriculture &&
            recipe.Category == RecipeCategoryType.Food &&
            recipe.Components.Keys.Any(regionalIngredients.Contains));
    }

    [Test]
    public void WildFowl_SpawnsInTheSilverwoodAndDropsEggsAndMeat()
    {
        var spawns = new EshanSpawnDefinition()
            .BuildSpawnTables()["ESHAN_SILVERWOOD_EXPANSE"]
            .Spawns;
        var loot = new EshanLootTableDefinition().BuildLootTables()["ESHAN_WILD_FOWL"];

        spawns.Should().ContainSingle(spawn => spawn.Resref == "esh_wildfowl");
        loot.Should().ContainSingle(item => item.Resref == "esh_fowl_egg");
        loot.Should().ContainSingle(item => item.Resref == "esh_fowl_meat");

        var creaturePath = Path.Combine(
            FindRepositoryRoot().FullName, "Module", "utc", "esh_wildfowl.utc.json");
        using var creature = JsonDocument.Parse(File.ReadAllText(creaturePath));
        creature.RootElement.GetProperty("Appearance_Type").GetProperty("value").GetInt32()
            .Should().Be(6408, "the wild chicken should use the warocas creature model");
    }

    [Test]
    public void EchaniCuisine_CombatReadinessBonusScalesByRecipeTier()
    {
        var expectedBonuses = new Dictionary<string, int>
        {
            ["esh_dumpling"] = 3,
            ["esh_brothnoodle"] = 3,
            ["fire_noodles"] = 6,
            ["moon_buns"] = 9,
            ["silver_flatbread"] = 9,
            ["eshan_hearthpot"] = 9,
            ["esh_clayrice"] = 12,
            ["watch_porridge"] = 12,
            ["marbled_eggs"] = 15,
            ["ginger_fish"] = 15,
            ["glazed_fowl"] = 15,
        };
        var root = FindRepositoryRoot();

        foreach (var (resref, expectedBonus) in expectedBonuses)
        {
            var itemPath = Path.Combine(root.FullName, "Module", "uti", $"{resref}.uti.json");
            using var item = JsonDocument.Parse(File.ReadAllText(itemPath));
            var readinessProperties = item.RootElement
                .GetProperty("PropertiesList")
                .GetProperty("value")
                .EnumerateArray()
                .Where(property =>
                    property.GetProperty("PropertyName").GetProperty("value").GetInt32() == 106 &&
                    property.GetProperty("Subtype").GetProperty("value").GetInt32() == 9)
                .ToList();

            readinessProperties.Should().ContainSingle($"{resref} should have one Combat Readiness food bonus");
            readinessProperties[0].GetProperty("CostValue").GetProperty("value").GetInt32()
                .Should().Be(expectedBonus, $"{resref} should scale with its recipe tier");
        }
    }

    private static DirectoryInfo FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "SWLOR.Game.Server.sln")))
            directory = directory.Parent;

        return directory ?? throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
