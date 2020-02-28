namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCartTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthenticatedCarts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        movieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.UnauthenticatedCarts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        cartId = c.Int(nullable: false),
                        movieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UnauthenticatedCarts");
            DropTable("dbo.AuthenticatedCarts");
        }
    }
}
