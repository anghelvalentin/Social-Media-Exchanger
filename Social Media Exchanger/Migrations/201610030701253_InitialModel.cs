namespace Social_Media_Exchanger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacebookLikeds",
                c => new
                    {
                        FacebookPageId = c.Long(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.FacebookPageId, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Points = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FacebookPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacebookCode = c.Long(nullable: false),
                        Url = c.String(nullable: false, maxLength: 350),
                        Name = c.String(nullable: false, maxLength: 50),
                        Cpc = c.Int(nullable: false),
                        NumberOfClicks = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.FacebookCode, unique: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TwitterFollowers",
                c => new
                    {
                        TwitterPageId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.TwitterPageId, t.UserId })
                .ForeignKey("dbo.TwitterPages", t => t.TwitterPageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TwitterPageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TwitterPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TwitterUsername = c.String(nullable: false, maxLength: 50),
                        Url = c.String(nullable: false, maxLength: 60),
                        Name = c.String(nullable: false, maxLength: 50),
                        Cpc = c.Int(nullable: false),
                        NumberOfClicks = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TwitterUsername, unique: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SocialNetworks",
                c => new
                    {
                        SocialNetworkId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.SocialNetworkId);
            
            CreateTable(
                "dbo.YoutubePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YoutubeUsername = c.String(nullable: false, maxLength: 50),
                        Url = c.String(nullable: false, maxLength: 80),
                        Name = c.String(nullable: false, maxLength: 50),
                        Cpc = c.Int(nullable: false),
                        NumberOfClicks = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.YoutubeSubscribtions",
                c => new
                    {
                        YoutubePageId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.YoutubePageId, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.YoutubePages", t => t.YoutubePageId, cascadeDelete: true)
                .Index(t => t.YoutubePageId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YoutubeSubscribtions", "YoutubePageId", "dbo.YoutubePages");
            DropForeignKey("dbo.YoutubeSubscribtions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.YoutubePages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TwitterFollowers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TwitterFollowers", "TwitterPageId", "dbo.TwitterPages");
            DropForeignKey("dbo.TwitterPages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FacebookPages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FacebookLikeds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.YoutubeSubscribtions", new[] { "UserId" });
            DropIndex("dbo.YoutubeSubscribtions", new[] { "YoutubePageId" });
            DropIndex("dbo.YoutubePages", new[] { "UserId" });
            DropIndex("dbo.TwitterPages", new[] { "UserId" });
            DropIndex("dbo.TwitterPages", new[] { "TwitterUsername" });
            DropIndex("dbo.TwitterFollowers", new[] { "UserId" });
            DropIndex("dbo.TwitterFollowers", new[] { "TwitterPageId" });
            DropIndex("dbo.FacebookPages", new[] { "UserId" });
            DropIndex("dbo.FacebookPages", new[] { "FacebookCode" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.FacebookLikeds", new[] { "UserId" });
            DropTable("dbo.YoutubeSubscribtions");
            DropTable("dbo.YoutubePages");
            DropTable("dbo.SocialNetworks");
            DropTable("dbo.TwitterPages");
            DropTable("dbo.TwitterFollowers");
            DropTable("dbo.FacebookPages");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.FacebookLikeds");
        }
    }
}
