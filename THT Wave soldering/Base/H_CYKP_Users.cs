using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace THT_Wave_soldering.Base
{
    class Wave_Users
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string RFID { get; set; }
        [Required]
        public string UserName { get; set; }

        public ICollection<Wave_Log> Logs { get; set; }
    }
      
}
