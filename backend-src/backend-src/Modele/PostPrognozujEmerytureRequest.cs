namespace backend_src.Modele
{
    public class PostPrognozujEmerytureRequest
    {
        public int Wiek { get; set; }
        public bool CzyMezczyzna { get; set; }
        public decimal OczekiwanaEmerytura { get; set; }
        public int WiekPrzejsciaNaEmeryture { get; set; }
        public decimal KapitalPoczatkowy { get; set; }
        public string KodPocztowy { get; set; } = "";
        public decimal? WskaznikWaloryzacjiKonta { get; set; }
        public decimal? WskaznikWaloryzacjiSubkonta { get; set; }
        public Dictionary<int, decimal> WynagrodzeniaBrutto { get; set; } = new Dictionary<int, decimal>();
    }
}
