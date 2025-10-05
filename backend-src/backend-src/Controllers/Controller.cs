    using backend_src.Kalkulatorki;
    using backend_src.Modele;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace backend_src.Controllers
    {
	    [ApiController]
	    [Route("[controller]")]
	    public class Controller : ControllerBase
	    {
		    private readonly AppDbContext db;
		    IKalkulatorEmerytury kalkulatorEmerytury;

		    public Controller(AppDbContext db, IKalkulatorEmerytury kalkulatorEmerytury)
		    {
			    this.db = db;
			    this.kalkulatorEmerytury = kalkulatorEmerytury;
		    }

            [HttpPost("PostPrognozujEmeryture")]
            public ActionResult<PostPrognozujEmerytureResponse> PostPrognozujEmeryture(PostPrognozujEmerytureRequest request)
            {
                PostPrognozujEmerytureResponse wynik = new PostPrognozujEmerytureResponse();

                var kwotaNaKoncie = kalkulatorEmerytury.ObliczKwoteNaKoncie(request.WynagrodzeniaBrutto, request.WskaznikWaloryzacjiKonta);
                var kwotaNaSubkoncie = kalkulatorEmerytury.ObliczKwoteNaSubkoncie(request.WynagrodzeniaBrutto, request.WskaznikWaloryzacjiSubkonta);

                var przewidywaneKonto = kalkulatorEmerytury.PrzewidzWartoscNaKoncie(kwotaNaKoncie, request.WynagrodzeniaBrutto.Keys.Max(), 2100, request.WskaznikWaloryzacjiKonta);
                var przewidywaneSubkonto = kalkulatorEmerytury.PrzewidzWartoscNaSubkoncie(kwotaNaSubkoncie, request.WynagrodzeniaBrutto.Keys.Max(), 2100, request.WskaznikWaloryzacjiSubkonta);

                var inflacja = 1m;
                for (int i = przewidywaneKonto.Keys.Min(); i <= przewidywaneKonto.Keys.Max(); i++)
                {
                    var wartosc = kalkulatorEmerytury.ObliczEmeryture(przewidywaneSubkonto[i], przewidywaneKonto[i], request.KapitalPoczatkowy, Dane.PrzewidywanaDlugoscTrwaniaZycia2024[request.Wiek] * 12);
                    
                    wynik.PrzewidywanaEmerytura.Add(i, new PrzewidywanaEmerytura()
                    {
                        WysokoscRzeczywista = wartosc,
                        WysokoscUrealniona = wartosc / inflacja,
                        NaKoncie = przewidywaneKonto[i],
                        NaSubkoncie = przewidywaneSubkonto[i],
                    });

                    inflacja *= Dane.Inflacja[i];
                }

                var rokPrzejsciaNaEmeryture = DateTime.Now.Year - request.Wiek + request.WiekPrzejsciaNaEmeryture;

                var emeryturaWWiekuPrzejscia = wynik.PrzewidywanaEmerytura[rokPrzejsciaNaEmeryture];

                var raport = new Raport()
                {
                    Data = DateTime.Now,
                    OczekiwanaEmerytura = request.OczekiwanaEmerytura,
                    Wiek = request.Wiek,
                    CzyMezczyzna = request.CzyMezczyzna,
                    Wynagrodzenie = request.WynagrodzeniaBrutto[request.WynagrodzeniaBrutto.Keys.Max()],
                    CzyUwzglednialOkresChoroby = false,
                    SrodkiNaKoncie = emeryturaWWiekuPrzejscia.NaKoncie,
                    SrodkiNaSubkoncie = emeryturaWWiekuPrzejscia.NaSubkoncie,
                    EmeryturaRzeczywista = emeryturaWWiekuPrzejscia.WysokoscRzeczywista,
                    EmeryturaUrealniona = emeryturaWWiekuPrzejscia.WysokoscUrealniona,
                    KodPocztowy = request.KodPocztowy,
                };

                db.Raports.Add(raport);
                db.SaveChangesAsync();

                return Ok(wynik);
            }


            [HttpPost("PostRaporty")]
		    public ActionResult<List<Raport>> PostRaporty(GetRaportyRequest request)
		    {
			    var raport = db.Raports.AsNoTracking().Where(r => r.Data >= request.Od && r.Data <= request.Do).ToList();
			    return raport is null ? NotFound() : Ok(raport);
		    }
	    }
    }
