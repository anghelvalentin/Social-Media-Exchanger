namespace Social_Media_Exchanger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexReffid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DailyBonusTaken", c => c.Boolean());
            AlterColumn("dbo.Refferals", "ReffId", c => c.String(nullable: false, maxLength: 300));
            CreateIndex("dbo.Refferals", "ReffId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Refferals", new[] { "ReffId" });
            AlterColumn("dbo.Refferals", "ReffId", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "DailyBonusTaken");
        }
    }
}
