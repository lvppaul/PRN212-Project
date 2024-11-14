using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGroup.Dtos
{
    public class DecisionDtoRequest
    {

        public int PondId { get; set; }
        public float DesiredConcentration { get; set; }
        public float? CurrentConcentration { get; set; }
        public int? PercentWaterChange { get; set; }

    }
}
