using Microsoft.EntityFrameworkCore;

namespace ChessUI
{
    public class ChessGamesContext : DbContext
    {
        public DbSet<ChessGame> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=chessgames.db");
        }
    }
}
