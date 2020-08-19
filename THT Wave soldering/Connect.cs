using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THT_Wave_soldering.Base;

namespace THT_Wave_soldering
{
    class Connect : DbContext
    {
        public Connect() : base("RealBase")
        {

        }

         public DbSet<Wave_Users> Users { get; set; }

         public  DbSet<Wave_Defects> Defects { get; set; }
     
         public  DbSet<Wave_Log> Logs { get; set; }
         
         public  DbSet<Wave_Models> Models { get; set; }           
         
         public  DbSet<Wave_Spec> Specs { get; set; }  

    }

}
