using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGroup.Dtos
{
    public class PondDtosResponse
    {
        public int PondId { get; set; }
        public string Name { get; set; }

        public double Volume { get; set; }

        public int Depth { get; set; }

        public int PumpingCapacity { get; set; }

        public int Drain { get; set; }

        public int Skimmer { get; set; }

        public string? Note { get; set; }

    }
    public class PondDtosRequest
    {
        public string userId { get; set; }
        public string Name { get; set; }

        public double Volume { get; set; }

        public int Depth { get; set; }

        public int PumpingCapacity { get; set; }

        public int Drain { get; set; }

        public int Skimmer { get; set; }

        public string? Note { get; set; }

    }

}
