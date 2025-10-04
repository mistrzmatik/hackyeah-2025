using Microsoft.EntityFrameworkCore;

namespace backend_src
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Raport> Raports => Set<Raport>();
    }

    public class Raport
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal OczekiwanaEmerytura { get; set; }
        public int Wiek { get; set; }
        public bool CzyMezczyzna { get; set; }
        public decimal Wynagrodzenie { get; set; }
        public bool CzyUwzglednialOkresChoroby { get; set; }
        public decimal SrodkiNaKoncie { get; set; }
        public decimal SrodkiNaSubkoncie { get; set; }
        public decimal EmeryturaRzeczywista { get; set; }
        public decimal EmeryturaUrealniona { get; set; }
        public string KodPocztowy { get; set; } = "";
    }
}
