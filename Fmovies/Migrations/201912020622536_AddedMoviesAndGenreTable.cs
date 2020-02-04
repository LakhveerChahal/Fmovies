namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMoviesAndGenreTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        genreId = c.Int(nullable: false, identity: true),
                        genreName = c.String(),
                    })
                .PrimaryKey(t => t.genreId);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        movieId = c.Int(nullable: false, identity: true),
                        movieName = c.String(),
                        genreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.movieId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Movies");
            DropTable("dbo.Genres");
        }
    }
}
