namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBookingTable1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "userId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "userId", c => c.Int(nullable: false));
        }
    }
}
