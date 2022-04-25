namespace THT_Wave_soldering.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class THTWave2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Wave_Log", "count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Wave_Log", "count", c => c.String(maxLength: 5));
        }
    }
}
