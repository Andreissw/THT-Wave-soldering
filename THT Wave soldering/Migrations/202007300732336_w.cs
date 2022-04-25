namespace THT_Wave_soldering.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class w : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Wave_Models", "Objective", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Wave_Models", "Objective", c => c.Short(nullable: false));
        }
    }
}
