using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;

namespace SWLOR.Game.Server.Tests.Feature;

public class EchaniTempleMonksTests
{
    private static string Root => FindRoot();
    private static string FindRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "Module")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException();
    }
    private static JsonElement Read(string path) => JsonDocument.Parse(File.ReadAllBytes(Path.Combine(Root, path))).RootElement;
    private static JsonElement Value(JsonElement element, string field) => element.GetProperty(field).GetProperty("value");
    private static Dictionary<string, JsonElement> Variables(JsonElement element) => Value(element, "VarTable").EnumerateArray()
        .ToDictionary(variable => Value(variable, "Name").GetString()!, variable => Value(variable, "Value"));

    [Test]
    public void Monks_AreOptedIntoOnlyTheirOwnControllerAndHaveProtectedEquipment()
    {
        var placed = Value(Read("Module/git/pw_ar_sc_eshanci.git.json"), "Creature List").EnumerateArray().ToArray();
        foreach (var tag in new[] { "es_monk_master", "es_monk_ren", "es_monk_sai" })
        {
            var blueprint = Read($"Module/utc/{tag}.utc.json");
            var npc = placed.Single(npc => Value(npc, "Tag").GetString() == tag);
            foreach (var creature in new[] { blueprint, npc })
            {
                Variables(creature)["ECHANI_MONK"].GetInt32().Should().Be(1);
                Variables(creature)["AI_FLAGS"].GetInt32().Should().Be(0);
                Variables(creature).Should().NotContainKey("ESHAN_PATROL_ROUTE");
                Value(creature, "Appearance_Type").GetInt32().Should().Be(10009);
                Value(creature, "Phenotype").GetInt32().Should().Be(0);
                Value(creature, "Plot").GetInt32().Should().Be(1);
            }
            foreach (var equipment in Value(blueprint, "Equip_ItemList").EnumerateArray())
            {
                var item = Read($"Module/uti/{Value(equipment, "EquippedRes").GetString()}.uti.json");
                Value(item, "Plot").GetInt32().Should().Be(1);
                Variables(item)["NO_ECONOMY"].GetInt32().Should().Be(1);
                var instance = Value(npc, "Equip_ItemList").EnumerateArray()
                    .Single(entry => entry.GetProperty("__struct_id").GetInt32() == equipment.GetProperty("__struct_id").GetInt32());
                Value(instance, "Dropable").GetInt32().Should().Be(0);
                foreach (var field in item.EnumerateObject().Where(field => field.Name != "__data_type"))
                    JsonElement.DeepEquals(field.Value, instance.GetProperty(field.Name)).Should().BeTrue(field.Name);
            }
        }
    }

    [Test]
    public void Outfits_UseNativeGffLabelsAndConsistentArmorParts()
    {
        foreach (var name in new[] { "master", "ren", "sai" })
        foreach (var kind in new[] { "wear", "hood" })
        {
            var item = Read($"Module/uti/esm_{name}_{kind}.uti.json");
            foreach (var field in item.EnumerateObject().Where(field => field.Name != "__data_type"))
            {
                field.Name.Length.Should().BeLessThanOrEqualTo(16, "NWN GFF labels are at most 16 bytes");
                if (!field.Name.StartsWith("ArmorPart_")) continue;
                var extended = "x" + field.Name;
                if (extended.Length > 16) extended = extended[..16];
                Value(item, extended).GetInt32().Should().Be(Value(item, field.Name).GetInt32(),
                    $"{name} {field.Name} must match its native extended field");
            }
        }
    }

    [Test]
    public void Route_IsOneClosedLoopAndEveryAreaChangeHasAnExistingTransferDestination()
    {
        var points = new Dictionary<string, (string Area, JsonElement Point)>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(Root, "Module/git"), "*.git.json"))
        {
            // Only Eshan areas carry the authored monk route.
            if (!new[] { "esh", "es_", "sc_es" }.Any(part => Path.GetFileName(file).Contains(part))) continue;
            var area = JsonDocument.Parse(File.ReadAllBytes(file)).RootElement;
            if (!area.TryGetProperty("WaypointList", out var list)) continue;
            foreach (var point in list.GetProperty("value").EnumerateArray())
            {
                var tag = Value(point, "Tag").GetString()!;
                if (tag.StartsWith("es_monk_path_")) points.TryAdd(tag, (file, point)).Should().BeTrue();
                else if (!points.ContainsKey(tag)) points.Add(tag, (file, point));
            }
        }
        var visited = new HashSet<string>();
        var current = "es_monk_path_000";
        while (visited.Add(current))
        {
            var (area, point) = points[current];
            var variables = Variables(point);
            var next = variables["ECHANI_NEXT"].GetString()!;
            points.Should().ContainKey(next);
            if (variables.TryGetValue("ECHANI_TRANSFER", out var transfer))
            {
                points.Should().ContainKey(transfer.GetString()!);
                points[transfer.GetString()!].Area.Should().Be(points[next].Area);
            }
            else points[next].Area.Should().Be(area, "ordinary movement cannot cross an area boundary");
            current = next;
        }
        current.Should().Be("es_monk_path_000");
        visited.Count.Should().Be(points.Keys.Count(tag => tag.StartsWith("es_monk_path_")));
    }
}
