namespace Social_Media_Exchanger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyBonusandRefferalTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Refferals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        ReffId = c.String(nullable: false),
                        Activate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "ClicksToday", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ClicksToday");
            DropTable("dbo.Refferals");
        }
    }
}
