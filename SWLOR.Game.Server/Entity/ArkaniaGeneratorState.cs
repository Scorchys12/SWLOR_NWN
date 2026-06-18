using System;

namespace SWLOR.Game.Server.Entity
{
    public class ArkaniaGeneratorState : EntityBase
    {
        public ArkaniaGeneratorState()
        {
            Id = "ARKANIA_GENERATOR";
            HeatReserve = 50;
            Integrity = 100;
            Morale = 50;
            LastUpdated = DateTime.UtcNow;
        }

        public int HeatReserve { get; set; }
        public int Integrity { get; set; }
        public int Coal { get; set; }
        public int MachineParts { get; set; }
        public int HeatCores { get; set; }
        public int MedicalSupplies { get; set; }
        public int ForecastData { get; set; }
        public int Morale { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
