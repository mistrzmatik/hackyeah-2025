namespace backend_src.Modele
{
    public class PostPrognozujEmerytureResponse
    {
        public Dictionary<int, PrzewidywanaEmerytura> PrzewidywanaEmerytura { get; set; } = new Dictionary<int, PrzewidywanaEmerytura>();
    }

    public class PrzewidywanaEmerytura
    {
        public decimal WysokoscRzeczywista { get; set; }
        public decimal WysokoscUrealniona { get; set; }
        public decimal NaKoncie { get; set; }
        public decimal NaSubkoncie { get; set; }
        public decimal PrzewidywanaWyplata { get; set; }
        public decimal RR { get; set; }
    }
}
