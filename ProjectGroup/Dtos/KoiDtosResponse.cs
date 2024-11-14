using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGroup.Dtos
{
    public class KoiDtosResponse
    {
        public int KoiId { get; set; }
        public int Age { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public string Variety { get; set; }

        public string Physique { get; set; }

        public string Note { get; set; }

        public string Origin { get; set; }

        public int Length { get; set; }

        public float Weight { get; set; }

        public string Color { get; set; }

        public bool Status { get; set; }

    }

    public class KoiDtosRequest
    {
        public string UserId { get; set; }
        public int PondId { get; set; }
        public int Age { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public string Variety { get; set; }

        public string Physique { get; set; }

        public string Note { get; set; }

        public string Origin { get; set; }

        public int Length { get; set; }

        public float Weight { get; set; }

        public string Color { get; set; }

        public bool Status { get; set; }

    }
}
