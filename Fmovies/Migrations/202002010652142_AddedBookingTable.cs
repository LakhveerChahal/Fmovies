namespace Fmovies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBookingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        bookingDate = c.DateTime(nullable: false),
                        totalPrice = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Bookings");
        }
    }
}
