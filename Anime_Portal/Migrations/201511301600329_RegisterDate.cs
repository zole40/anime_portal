namespace Anime_Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegisterDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RegisterDate");
        }
    }
}
