using backend_src.Modele;

namespace backend_src.Kalkulatorki
{
    public interface IKalkulatorEmerytury
    {
        public decimal ObliczEmeryture(decimal kwotaBazowa, decimal kwotaNaKoncie, decimal kwotaNaSubkoncie, decimal przewidywanaLiczbaMiesiecyZycia);
        public decimal ObliczKwoteNaKoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaKonta = null, decimal? sredniaIlosciDniL4WRoku = null);
        public decimal ObliczKwoteNaSubkoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaSubkonta = null, decimal? sredniaIlosciDniL4WRoku = null);
        public Dictionary<int, decimal> PrzewidzWynagrodzenia(decimal wyplata, int obecnyRok, int przewidywaneDo);
        public Dictionary<int, decimal> PrzewidzWartoscNaKoncie(decimal obecnaWartosc, decimal wyplata, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaKonta = null, decimal? sredniaIlosciDniL4WRoku = null);
        public Dictionary<int, decimal> PrzewidzWartoscNaSubkoncie(decimal obecnaWartosc, decimal wyplata, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaSubkonta = null, decimal? sredniaIlosciDniL4WRoku = null);
    }
}