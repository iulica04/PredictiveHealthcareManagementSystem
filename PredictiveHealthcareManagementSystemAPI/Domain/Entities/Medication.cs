using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum MedicationType
    {
        Tablet,
        Capsule,
        Syrup,
        Injection,
        Cream,
        Ointment,
        Drops,
        Inhaler,
        Spray,
        Patch,
        Suppository,
        Implant,
        Powder,
        Gel,
        Lotion,
        Lozenge,
        Solution,
        Suspension,
        Syringe,
        Other
    }
    public class Medication
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public MedicationType Type { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> AdverseEffects { get; set; }
    }
}
