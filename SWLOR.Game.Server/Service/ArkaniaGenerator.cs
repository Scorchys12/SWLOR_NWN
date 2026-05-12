using System;
using SWLOR.Game.Server.Entity;

namespace SWLOR.Game.Server.Service
{
    public static class ArkaniaGenerator
    {
        public const string GeneratorId = "ARKANIA_GENERATOR";
        public const string CoalResref = "ark_coal";
        public const string MachinePartResref = "ark_machine_part";
        public const string HeatCoreResref = "ark_heat_core";
        public const string MedicalSuppliesResref = "ark_med_supplies";
        public const string ForecastDataResref = "ark_forecast_data";

        private const int MaximumHeatReserve = 100;
        private const int MaximumIntegrity = 100;
        private const int MaximumMorale = 100;

        public static ArkaniaGeneratorState GetState()
        {
            var state = DB.Get<ArkaniaGeneratorState>(GeneratorId);
            if (state != null)
                return state;

            state = new ArkaniaGeneratorState();
            DB.Set(state);

            return state;
        }

        public static ArkaniaGeneratorState AddCoal(int amount)
        {
            var safeAmount = Math.Max(0, amount);
            var state = GetState();
            state.Coal += safeAmount;
            state.HeatReserve = Math.Min(MaximumHeatReserve, state.HeatReserve + safeAmount);
            state.LastUpdated = DateTime.UtcNow;
            DB.Set(state);

            return state;
        }

        public static ArkaniaGeneratorState AddHeatCores(int amount)
        {
            var safeAmount = Math.Max(0, amount);
            var state = GetState();
            state.HeatCores += safeAmount;
            state.HeatReserve = Math.Min(MaximumHeatReserve, state.HeatReserve + safeAmount * 10);
            state.LastUpdated = DateTime.UtcNow;
            DB.Set(state);

            return state;
        }

        public static ArkaniaGeneratorState AddMachineParts(int amount)
        {
            var safeAmount = Math.Max(0, amount);
            var state = GetState();
            state.MachineParts += safeAmount;
            state.Integrity = Math.Min(MaximumIntegrity, state.Integrity + safeAmount * 5);
            state.LastUpdated = DateTime.UtcNow;
            DB.Set(state);

            return state;
        }

        public static ArkaniaGeneratorState AddMedicalSupplies(int amount)
        {
            var safeAmount = Math.Max(0, amount);
            var state = GetState();
            state.MedicalSupplies += safeAmount;
            state.Morale = Math.Min(MaximumMorale, state.Morale + safeAmount * 2);
            state.LastUpdated = DateTime.UtcNow;
            DB.Set(state);

            return state;
        }

        public static ArkaniaGeneratorState AddForecastData(int amount)
        {
            var safeAmount = Math.Max(0, amount);
            var state = GetState();
            state.ForecastData += safeAmount;
            state.LastUpdated = DateTime.UtcNow;
            DB.Set(state);

            return state;
        }

        public static string GetCondition(ArkaniaGeneratorState state)
        {
            if (state.HeatReserve >= 70) return "Stable";
            if (state.HeatReserve >= 40) return "Strained";
            if (state.HeatReserve >= 15) return "Failing";

            return "Critical";
        }

        public static string BuildStatusText()
        {
            var state = GetState();
            return
                $"Condition: {GetCondition(state)}\n" +
                $"Heat Reserve: {state.HeatReserve}/100\n" +
                $"Integrity: {state.Integrity}/100\n" +
                $"Coal Stockpile: {state.Coal}\n" +
                $"Machine Parts: {state.MachineParts}\n" +
                $"Heat Cores: {state.HeatCores}\n" +
                $"Medical Supplies: {state.MedicalSupplies}\n" +
                $"Forecast Data: {state.ForecastData}\n" +
                $"Morale: {state.Morale}/100";
        }
    }
}
