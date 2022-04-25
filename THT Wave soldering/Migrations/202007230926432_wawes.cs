namespace THT_Wave_soldering.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wawes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wave_Defects",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Defect = c.String(maxLength: 5),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Wave_Log",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        DateFact = c.DateTime(nullable: false),
                        ControllerId = c.Int(nullable: false),
                        Mate_ControllerId = c.Int(nullable: false),
                        ModelsId = c.Int(nullable: false),
                        Line = c.Byte(nullable: false),
                        Selection = c.Int(nullable: false),
                        DefectsId = c.Int(),
                        Position = c.String(),
                        Lot = c.String(),
                        Time = c.String(maxLength: 9),
                        count = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Wave_Users", t => t.ControllerId, cascadeDelete: true)
                .ForeignKey("dbo.Wave_Defects", t => t.DefectsId)
                .ForeignKey("dbo.Wave_Models", t => t.ModelsId, cascadeDelete: true)
                .Index(t => t.ControllerId)
                .Index(t => t.ModelsId)
                .Index(t => t.DefectsId);
            
            CreateTable(
                "dbo.Wave_Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RFID = c.String(nullable: false, maxLength: 15),
                        UserName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Wave_Models",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Product = c.String(),
                        SpecId = c.Int(nullable: false),
                        Opportunity = c.Int(nullable: false),
                        Objective = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Wave_Spec",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Cust = c.String(),
                        Name = c.String(),
                        Position = c.String(),
                        NumSpec = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Wave_Log", "ModelsId", "dbo.Wave_Models");
            DropForeignKey("dbo.Wave_Log", "DefectsId", "dbo.Wave_Defects");
            DropForeignKey("dbo.Wave_Log", "ControllerId", "dbo.Wave_Users");
            DropIndex("dbo.Wave_Log", new[] { "DefectsId" });
            DropIndex("dbo.Wave_Log", new[] { "ModelsId" });
            DropIndex("dbo.Wave_Log", new[] { "ControllerId" });
            DropTable("dbo.Wave_Spec");
            DropTable("dbo.Wave_Models");
            DropTable("dbo.Wave_Users");
            DropTable("dbo.Wave_Log");
            DropTable("dbo.Wave_Defects");
        }
    }
}
