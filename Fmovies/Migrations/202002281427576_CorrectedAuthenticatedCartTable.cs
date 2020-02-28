namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectedAuthenticatedCartTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AuthenticatedCarts", "userId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AuthenticatedCarts", "userId", c => c.Int(nullable: false));
        }
    }
}
