namespace Anime_Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomUserData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PictureUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DisplayName");
            DropColumn("dbo.AspNetUsers", "PictureUrl");
        }
    }
}
