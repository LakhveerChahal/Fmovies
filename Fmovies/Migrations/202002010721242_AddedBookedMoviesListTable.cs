namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBookedMoviesListTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookedMoviesLists",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bookingId = c.Int(nullable: false),
                        movieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BookedMoviesLists");
        }
    }
}
