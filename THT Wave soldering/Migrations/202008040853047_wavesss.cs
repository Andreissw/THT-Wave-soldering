namespace THT_Wave_soldering.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wavesss : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wave_Log", "Remove", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wave_Log", "Remove");
        }
    }
}
