namespace THT_Wave_soldering.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wawev : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Wave_Log", "DefectsId", "dbo.Wave_Defects");
            DropIndex("dbo.Wave_Log", new[] { "DefectsId" });
            AddColumn("dbo.Wave_Log", "Defects", c => c.String());
            DropColumn("dbo.Wave_Log", "DefectsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wave_Log", "DefectsId", c => c.Int());
            DropColumn("dbo.Wave_Log", "Defects");
            CreateIndex("dbo.Wave_Log", "DefectsId");
            AddForeignKey("dbo.Wave_Log", "DefectsId", "dbo.Wave_Defects", "ID");
        }
    }
}
