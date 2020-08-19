

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace THT_Wave_soldering.Base
{
   class Wave_Models
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Product { get; set; }     

        public int SpecId { get; set; }

        public int Opportunity { get; set; }

        [MaxLength(4)]
        public string Objective { get; set; }               

        public virtual ICollection<Wave_Log> Logs { get; set; }

           

   

    }
}
