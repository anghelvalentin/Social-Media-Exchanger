namespace Social_Media_Exchanger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSocialNetworks : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO SocialNetworks (Name) VALUES ('Facebook Likes')");
            Sql("INSERT INTO SocialNetworks (Name) VALUES ('Twitter Followers')");
            Sql("INSERT INTO SocialNetworks (Name) VALUES ('Youtube Subscribers')");
        }

        public override void Down()
        {
        }
    }
}
