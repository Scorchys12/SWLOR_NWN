using System.Collections.Generic;
using SWLOR.Game.Server.Service;
using SWLOR.Game.Server.Service.QuestService;

namespace SWLOR.Game.Server.Feature.QuestDefinition
{
    public class ArkaniaQuestDefinition : IQuestListDefinition
    {
        public Dictionary<string, QuestDetail> BuildQuests()
        {
            var builder = new QuestBuilder();

            TheCityMustSurvive(builder);
            ChildrenInTheIce(builder);
            CoalRunWhiteoutRoute(builder);
            TheGeneratorCoughs(builder);
            HeatForTheWards(builder);
            BlackIceSignal(builder);

            return builder.Build();
        }

        private void TheCityMustSurvive(QuestBuilder builder)
        {
            builder.Create("ark_city_survive", "The City Must Survive")
                .IsRepeatable()

                .AddState()
                .SetStateJournalText("Generator Steward Veyra Tann needs fuel and repair stock for the Arkanian Thermal Generator. Bring coal, a heat core, and machine parts back to the generator chamber.")
                .AddCollectItemObjective(ArkaniaGenerator.CoalResref, 5)
                .AddCollectItemObjective(ArkaniaGenerator.HeatCoreResref, 1)
                .AddCollectItemObjective(ArkaniaGenerator.MachinePartResref, 2)

                .AddState()
                .SetStateJournalText("You delivered emergency supplies for the Arkanian Thermal Generator. Report back to Generator Steward Veyra Tann.")

                .AddGoldReward(1500)
                .AddXPReward(1000)

                .OnCompleteAction((player, sourceObject) =>
                {
                    ArkaniaGenerator.AddCoal(5);
                    ArkaniaGenerator.AddHeatCores(1);
                    ArkaniaGenerator.AddMachineParts(2);
                });
        }

        private void ChildrenInTheIce(QuestBuilder builder)
        {
            builder.Create("ark_children_ice", "Children in the Ice")

                .AddState()
                .SetStateJournalText("A collapsed mining tunnel has trapped several children beyond the settlement shield line. Reach the first blocked tunnel marker and clear a path.")

                .AddState()
                .SetStateJournalText("The first obstruction is clear. Stabilize the second tunnel marker before the ice shifts again.")

                .AddState()
                .SetStateJournalText("The second tunnel marker is secure. Reach the survivor pocket and signal the children to move.")

                .AddState()
                .SetStateJournalText("The children have been found. Return to the foreman in the settlement before the storm worsens.")

                .AddGoldReward(2250)
                .AddXPReward(2500)
                .AddItemReward(ArkaniaGenerator.CoalResref, 3);
        }

        private void CoalRunWhiteoutRoute(QuestBuilder builder)
        {
            builder.Create("ark_coal_run", "Coal Run: Whiteout Route")
                .IsRepeatable()

                .AddState()
                .SetStateJournalText("The city needs coal from the exposed mining road. Gather ten coal crates and return them to the generator chamber.")
                .AddCollectItemObjective(ArkaniaGenerator.CoalResref, 10)

                .AddState()
                .SetStateJournalText("You recovered coal from the whiteout route. Report back to the generator chamber.")

                .AddGoldReward(1200)
                .AddXPReward(800)

                .OnCompleteAction((player, sourceObject) =>
                {
                    ArkaniaGenerator.AddCoal(10);
                });
        }

        private void TheGeneratorCoughs(QuestBuilder builder)
        {
            builder.Create("ark_generator_coughs", "The Generator Coughs")
                .PrerequisiteQuest("ark_city_survive")

                .AddState()
                .SetStateJournalText("The generator's pressure regulators are failing. Inspect the first gauge cluster in the generator chamber.")

                .AddState()
                .SetStateJournalText("The first gauge cluster confirms a regulator fault. Inspect the second gauge cluster.")

                .AddState()
                .SetStateJournalText("The regulator fault is worse than expected. Recover six machine parts from the ruined Arkanian maintenance depot.")
                .AddCollectItemObjective(ArkaniaGenerator.MachinePartResref, 6)

                .AddState()
                .SetStateJournalText("You recovered the replacement parts. Install them at the generator console.")

                .AddState()
                .SetStateJournalText("The generator repairs are complete. Report back to Generator Steward Veyra Tann.")

                .AddGoldReward(3000)
                .AddXPReward(3500)

                .OnCompleteAction((player, sourceObject) =>
                {
                    ArkaniaGenerator.AddMachineParts(6);
                });
        }

        private void HeatForTheWards(QuestBuilder builder)
        {
            builder.Create("ark_heat_wards", "Heat for the Wards")
                .IsRepeatable()

                .AddState()
                .SetStateJournalText("The med ward is running below safe temperature. Bring medical supply packs so the quartermaster can keep the refugees alive.")
                .AddCollectItemObjective(ArkaniaGenerator.MedicalSuppliesResref, 5)

                .AddState()
                .SetStateJournalText("You gathered supplies for the med ward. Return to the refugee quartermaster.")

                .AddGoldReward(1000)
                .AddXPReward(800)

                .OnCompleteAction((player, sourceObject) =>
                {
                    ArkaniaGenerator.AddMedicalSupplies(5);
                });
        }

        private void BlackIceSignal(QuestBuilder builder)
        {
            builder.Create("ark_black_ice_signal", "Black Ice Signal")
                .PrerequisiteQuest("ark_children_ice")

                .AddState()
                .SetStateJournalText("An old Arkanian transmitter is still broadcasting through the storm. Reach the buried relay and recover forecast data.")
                .AddCollectItemObjective(ArkaniaGenerator.ForecastDataResref, 3)

                .AddState()
                .SetStateJournalText("You recovered forecast data from the buried relay. Return it to the generator chamber so the city can predict the next whiteout.")

                .AddGoldReward(2500)
                .AddXPReward(3000)

                .OnCompleteAction((player, sourceObject) =>
                {
                    ArkaniaGenerator.AddForecastData(3);
                });
        }
    }
}
