using backend_src.Modele;

namespace backend_src.Kalkulatorki
{
    public class KalkulatorEmerytury : IKalkulatorEmerytury
    {
        public decimal ObliczEmeryture(decimal kwotaBazowa, decimal kwotaNaKoncie, decimal kwotaNaSubkoncie, decimal przewidywanaLiczbaMiesiecyZycia)
        {
            return (kwotaBazowa + kwotaNaKoncie + kwotaNaSubkoncie) / przewidywanaLiczbaMiesiecyZycia;
        }

        public decimal ObliczKwoteNaKoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaKonta = null, decimal? sredniaIlosciDniL4WRoku = null)
        {
            if (wynagrodzenia == null || wynagrodzenia.Count == 0)
            {
                return 0;
            }

            decimal wynik = 0m;
            decimal proporcja = 1m - ((sredniaIlosciDniL4WRoku ?? 0m) / 365m);

            for (int rok = wynagrodzenia.Keys.Min(); rok <= wynagrodzenia.Keys.Max(); rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaKonta != null)
                {
                    czynnik = 1m + (waloryzacjaKonta.Value / 100m);
                }
                else if (Dane.WaloryzacjeKonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                wynik = Math.Round(wynik * czynnik, 2, MidpointRounding.AwayFromZero);
                if (wynagrodzenia.TryGetValue(rok, out var wynagrodzenie))
                {
                    wynik += wynagrodzenie * proporcja * (Dane.SkladkaEmerytalnaProcentKonto / 100m);
                }
            }

            return wynik;
        }

        public decimal ObliczKwoteNaSubkoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaSubkonta = null, decimal? sredniaIlosciDniL4WRoku = null)
        {
            if (wynagrodzenia == null || wynagrodzenia.Count == 0)
            {
                return 0;
            }

            decimal wynik = 0m;
            decimal proporcja = 1m - ((sredniaIlosciDniL4WRoku ?? 0m) / 365m);

            for (int rok = wynagrodzenia.Keys.Min(); rok <= wynagrodzenia.Keys.Max(); rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaSubkonta != null)
                {
                    czynnik = 1m + (waloryzacjaSubkonta.Value / 100m);
                }
                else if (Dane.WaloryzacjeSubkonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                wynik = Math.Round(wynik * czynnik, 2, MidpointRounding.AwayFromZero);
                if (wynagrodzenia.TryGetValue(rok, out var wynagrodzenie))
                {
                    wynik += wynagrodzenie * proporcja  * (Dane.SkladkaEmerytalnaProcentSubkonto / 100m);
                }
            }

            return wynik;
        }

        public Dictionary<int, decimal> PrzewidzWynagrodzenia(decimal wyplata, int obecnyRok, int przewidywaneDo)
        {
            var wynik = new Dictionary<int, decimal>();
            wynik.Add(obecnyRok, wyplata);

            for (int rok = obecnyRok + 1; rok <= przewidywaneDo; rok++)
            {
                wyplata *= Dane.WzrostyPlac[rok];

                wynik.Add(rok, wyplata);
            }

            return wynik;
        }

        public Dictionary<int, decimal> PrzewidzWartoscNaKoncie(decimal obecnaWartosc, decimal wyplata, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaKonta = null, decimal? sredniaIlosciDniL4WRoku = null)
        {
            var wynik = new Dictionary<int, decimal>();
            wynik.Add(obecnyRok, obecnaWartosc);
            decimal proporcja = 1m - ((sredniaIlosciDniL4WRoku ?? 0m) / 365m);

            for (int rok = obecnyRok + 1; rok <= przewidywaneDo; rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaKonta != null)
                {
                    czynnik = 1m + (waloryzacjaKonta.Value / 100m);
                }
                else if (Dane.WaloryzacjeKonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                obecnaWartosc = Math.Round(obecnaWartosc * czynnik, 2, MidpointRounding.AwayFromZero);

                wyplata *= Dane.WzrostyPlac[rok];

                obecnaWartosc += wyplata * proporcja * 12m * (Dane.SkladkaEmerytalnaProcentKonto / 100m);

                wynik.Add(rok, obecnaWartosc);
            }

            return wynik;
        }

        public Dictionary<int, decimal> PrzewidzWartoscNaSubkoncie(decimal obecnaWartosc, decimal wyplata, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaSubkonta = null, decimal? sredniaIlosciDniL4WRoku = null)
        {
            var wynik = new Dictionary<int, decimal>();
            wynik.Add(obecnyRok, obecnaWartosc);
            decimal proporcja = 1m - ((sredniaIlosciDniL4WRoku ?? 0m) / 365m);

            for (int rok = obecnyRok + 1; rok <= przewidywaneDo; rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaSubkonta != null)
                {
                    czynnik = 1m + (waloryzacjaSubkonta.Value / 100m);
                }
                else if (Dane.WaloryzacjeSubkonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                obecnaWartosc = Math.Round(obecnaWartosc * czynnik, 2, MidpointRounding.AwayFromZero);

                wyplata *= Dane.WzrostyPlac[rok];

                obecnaWartosc += wyplata * proporcja * 12m * (Dane.SkladkaEmerytalnaProcentSubkonto / 100m);

                wynik.Add(rok, obecnaWartosc);
            }

            return wynik;
        }
    }
}
