using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace THT_Wave_soldering.Base
{
    class Wave_Defects
    {
        public int ID { get; set; }

        [MaxLength(5)]

        public string Defect { get; set; }

        public string Description { get; set; }

    }
}
