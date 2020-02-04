namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPriceColumnInMovie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "moviePrice", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "moviePrice");
        }
    }
}
