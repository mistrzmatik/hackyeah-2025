using backend_src.Modele;

namespace backend_src.Kalkulatorki
{
    public class KalkulatorEmerytury : IKalkulatorEmerytury
    {
        public decimal ObliczEmeryture(decimal kwotaBazowa, decimal kwotaNaKoncie, decimal kwotaNaSubkoncie, decimal przewidywanaLiczbaMiesiecyZycia)
        {
            return (kwotaBazowa + kwotaNaKoncie + kwotaNaSubkoncie) / przewidywanaLiczbaMiesiecyZycia;
        }

        public decimal ObliczKwoteNaKoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaKonta = null)
        {
            if (wynagrodzenia == null || wynagrodzenia.Count == 0)
            {
                return 0;
            }

            decimal wynik = 0m;

            for (int rok = wynagrodzenia.Keys.Min(); rok <= wynagrodzenia.Keys.Max(); rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaKonta != null)
                {
                    czynnik = waloryzacjaKonta.Value / 100m;
                }
                else if (Dane.WaloryzacjeKonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                wynik = Math.Round(wynik * czynnik, 2, MidpointRounding.AwayFromZero);
                if (wynagrodzenia.TryGetValue(rok, out var wynagrodzenie))
                {
                    wynik += wynagrodzenie * (Dane.SkladkaEmerytalnaProcentKonto / 100m);
                }
            }

            return wynik;
        }

        public decimal ObliczKwoteNaSubkoncie(Dictionary<int, decimal> wynagrodzenia, decimal? waloryzacjaSubkonta = null)
        {
            if (wynagrodzenia == null || wynagrodzenia.Count == 0)
            {
                return 0;
            }

            decimal wynik = 0m;

            for (int rok = wynagrodzenia.Keys.Min(); rok <= wynagrodzenia.Keys.Max(); rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaSubkonta != null)
                {
                    czynnik = waloryzacjaSubkonta.Value / 100m;
                }
                else if (Dane.WaloryzacjeSubkonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                wynik = Math.Round(wynik * czynnik, 2, MidpointRounding.AwayFromZero);
                if (wynagrodzenia.TryGetValue(rok, out var wynagrodzenie))
                {
                    wynik += wynagrodzenie * (Dane.SkladkaEmerytalnaProcentSubkonto / 100m);
                }
            }

            return wynik;
        }

        public Dictionary<int, decimal> PrzewidzWartoscNaKoncie(decimal obecnaWartosc, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaKonta = null)
        {
            var wynik = new Dictionary<int, decimal>();
            wynik.Add(obecnyRok, obecnaWartosc);

            for (int rok = obecnyRok + 1; rok <= przewidywaneDo; rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaKonta != null)
                {
                    czynnik = waloryzacjaKonta.Value / 100m;
                }
                else if (Dane.WaloryzacjeKonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                obecnaWartosc = Math.Round(obecnaWartosc * czynnik, 2, MidpointRounding.AwayFromZero);

                obecnaWartosc += Dane.PrzewidywaneMiesieczneZarobkiNaRok[rok] * 12m * (Dane.SkladkaEmerytalnaProcentKonto / 100m);

                wynik.Add(rok, obecnaWartosc);
            }

            return wynik;
        }

        public Dictionary<int, decimal> PrzewidzWartoscNaSubkoncie(decimal obecnaWartosc, int obecnyRok, int przewidywaneDo, decimal? waloryzacjaSubkonta = null)
        {
            var wynik = new Dictionary<int, decimal>();
            wynik.Add(obecnyRok, obecnaWartosc);

            for (int rok = obecnyRok + 1; rok <= przewidywaneDo; rok++)
            {
                decimal czynnik = 1m;
                if (waloryzacjaSubkonta != null)
                {
                    czynnik = waloryzacjaSubkonta.Value / 100m;
                }
                else if (Dane.WaloryzacjeSubkonta.TryGetValue(rok, out var procent))
                {
                    czynnik = procent / 100m;
                }

                obecnaWartosc = Math.Round(obecnaWartosc * czynnik, 2, MidpointRounding.AwayFromZero);

                obecnaWartosc += Dane.PrzewidywaneMiesieczneZarobkiNaRok[rok] * 12m * (Dane.SkladkaEmerytalnaProcentSubkonto / 100m);

                wynik.Add(rok, obecnaWartosc);
            }

            return wynik;
        }
    }
}
