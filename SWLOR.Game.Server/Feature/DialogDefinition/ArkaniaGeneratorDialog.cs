using SWLOR.Game.Server.Service;
using SWLOR.Game.Server.Service.DialogService;

namespace SWLOR.Game.Server.Feature.DialogDefinition
{
    public class ArkaniaGeneratorDialog : DialogBase
    {
        private const string MainPageId = "MAIN_PAGE";
        private const string FuelPageId = "FUEL_PAGE";
        private const string RepairPageId = "REPAIR_PAGE";
        private const string SupportPageId = "SUPPORT_PAGE";

        public override PlayerDialog SetUp(uint player)
        {
            var builder = new DialogBuilder()
                .AddPage(MainPageId, MainPageInit)
                .AddPage(FuelPageId, FuelPageInit)
                .AddPage(RepairPageId, RepairPageInit)
                .AddPage(SupportPageId, SupportPageInit);

            return builder.Build();
        }

        private void MainPageInit(DialogPage page)
        {
            page.Header = ColorToken.Green("Arkanian Thermal Generator\n\n") +
                          "The generator shudders beneath layers of frost-covered plating. " +
                          "Every gauge is another reminder that the city survives only while the heat holds.\n\n" +
                          ArkaniaGenerator.BuildStatusText();

            page.AddResponse("Contribute fuel", () => ChangePage(FuelPageId));
            page.AddResponse("Contribute repair parts", () => ChangePage(RepairPageId));
            page.AddResponse("Support the wards or forecast team", () => ChangePage(SupportPageId));
        }

        private void FuelPageInit(DialogPage page)
        {
            var player = GetPC();
            page.Header = ColorToken.Green("Fuel Intake\n\n") +
                          "Coal keeps the exchangers alive. Heat cores are reserved for emergencies.\n\n" +
                          GetInventorySummary(player);

            page.AddResponse("Deposit one coal crate", () => Deposit(player, ArkaniaGenerator.CoalResref, 1, "coal crate"));
            page.AddResponse("Deposit five coal crates", () => Deposit(player, ArkaniaGenerator.CoalResref, 5, "coal crates"));
            page.AddResponse("Deposit one heat core", () => Deposit(player, ArkaniaGenerator.HeatCoreResref, 1, "heat core"));
            page.AddResponse("Return to generator status", () => ChangePage(MainPageId));
        }

        private void RepairPageInit(DialogPage page)
        {
            var player = GetPC();
            page.Header = ColorToken.Green("Maintenance Intake\n\n") +
                          "The generator can burn fuel only as long as its regulators and conduits hold.\n\n" +
                          GetInventorySummary(player);

            page.AddResponse("Deposit one machine part", () => Deposit(player, ArkaniaGenerator.MachinePartResref, 1, "machine part"));
            page.AddResponse("Deposit five machine parts", () => Deposit(player, ArkaniaGenerator.MachinePartResref, 5, "machine parts"));
            page.AddResponse("Return to generator status", () => ChangePage(MainPageId));
        }

        private void SupportPageInit(DialogPage page)
        {
            var player = GetPC();
            page.Header = ColorToken.Green("Civil Support\n\n") +
                          "The generator keeps bodies warm. The wards and forecast station keep hope alive.\n\n" +
                          GetInventorySummary(player);

            page.AddResponse("Deposit one medical supply pack", () => Deposit(player, ArkaniaGenerator.MedicalSuppliesResref, 1, "medical supply pack"));
            page.AddResponse("Deposit one forecast data spool", () => Deposit(player, ArkaniaGenerator.ForecastDataResref, 1, "forecast data spool"));
            page.AddResponse("Return to generator status", () => ChangePage(MainPageId));
        }

        private string GetInventorySummary(uint player)
        {
            return
                $"You are carrying:\n" +
                $"Coal: {CountItems(player, ArkaniaGenerator.CoalResref)}\n" +
                $"Machine Parts: {CountItems(player, ArkaniaGenerator.MachinePartResref)}\n" +
                $"Heat Cores: {CountItems(player, ArkaniaGenerator.HeatCoreResref)}\n" +
                $"Medical Supplies: {CountItems(player, ArkaniaGenerator.MedicalSuppliesResref)}\n" +
                $"Forecast Data: {CountItems(player, ArkaniaGenerator.ForecastDataResref)}";
        }

        private void Deposit(uint player, string resref, int amount, string displayName)
        {
            if (!RemoveItems(player, resref, amount))
            {
                FloatingTextStringOnCreature($"You do not have enough {displayName}.", player, false);
                return;
            }

            if (resref == ArkaniaGenerator.CoalResref)
            {
                ArkaniaGenerator.AddCoal(amount);
            }
            else if (resref == ArkaniaGenerator.HeatCoreResref)
            {
                ArkaniaGenerator.AddHeatCores(amount);
            }
            else if (resref == ArkaniaGenerator.MachinePartResref)
            {
                ArkaniaGenerator.AddMachineParts(amount);
            }
            else if (resref == ArkaniaGenerator.MedicalSuppliesResref)
            {
                ArkaniaGenerator.AddMedicalSupplies(amount);
            }
            else if (resref == ArkaniaGenerator.ForecastDataResref)
            {
                ArkaniaGenerator.AddForecastData(amount);
            }

            FloatingTextStringOnCreature($"The city receives {amount} {displayName}.", player, false);
            ChangePage(MainPageId, false);
        }

        private int CountItems(uint player, string resref)
        {
            var count = 0;
            for (var item = GetFirstItemInInventory(player); GetIsObjectValid(item); item = GetNextItemInInventory(player))
            {
                if (GetResRef(item) == resref)
                {
                    count += GetItemStackSize(item);
                }
            }

            return count;
        }

        private bool RemoveItems(uint player, string resref, int amount)
        {
            if (CountItems(player, resref) < amount)
                return false;

            var remaining = amount;
            var item = GetFirstItemInInventory(player);
            while (GetIsObjectValid(item) && remaining > 0)
            {
                var nextItem = GetNextItemInInventory(player);

                if (GetResRef(item) == resref)
                {
                    var stackSize = GetItemStackSize(item);
                    var reduceBy = stackSize > remaining ? remaining : stackSize;
                    Item.ReduceItemStack(item, reduceBy);
                    remaining -= reduceBy;
                }

                item = nextItem;
            }

            return true;
        }
    }
}
