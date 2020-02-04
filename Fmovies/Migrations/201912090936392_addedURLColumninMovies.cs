namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedURLColumninMovies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "URL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "URL");
        }
    }
}
