namespace BoardGamesRankingTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Plye : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "OwnerId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "OwnerId");
        }
    }
}
