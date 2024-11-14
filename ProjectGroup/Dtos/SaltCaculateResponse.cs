using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGroup.Dtos
{
    public class SaltCalculationResult
    {
        public float AmountOfSalt { get; set; }
        public float? AmountOfSaltRefill { get; set; }
        public int? NumberOfChanges { get; set; }
    }
}
