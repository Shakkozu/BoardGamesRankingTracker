namespace BoardGamesRankingTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Player1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "GamesPlayed", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "GamesWon", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "GamesTied", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "WinPercentage", c => c.Single(nullable: false));
            AddColumn("dbo.Players", "Joined", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Players", "OwnerId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Players", "OwnerId", c => c.Int(nullable: false));
            DropColumn("dbo.Players", "Joined");
            DropColumn("dbo.Players", "WinPercentage");
            DropColumn("dbo.Players", "GamesTied");
            DropColumn("dbo.Players", "GamesWon");
            DropColumn("dbo.Players", "GamesPlayed");
        }
    }
}
