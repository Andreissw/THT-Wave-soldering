

using System;
using System.ComponentModel.DataAnnotations;

namespace THT_Wave_soldering.Base
{
    class Wave_Log
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateFact { get; set; }

        public int ControllerId { get; set; }

        public int Mate_ControllerId { get; set; }

        public int ModelsId { get; set; }

        public byte Line { get; set; }

        public int Selection { get; set; }

        public string Defects { get; set; }

        public string Position { get; set; }

        public string Lot { get; set; }

        public bool ?Remove { get; set; }

        [MaxLength(9)]
        public string Time { get; set; }

        [MaxLength(5)]
        public string count { get; set; }           
        

        public virtual Wave_Users Controller { get; set; }

        public virtual Wave_Models Models { get; set; }

       

    }
}
