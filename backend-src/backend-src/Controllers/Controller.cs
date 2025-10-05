using backend_src.Kalkulatorki;
using backend_src.Modele;
using ClosedXML.Excel;
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

            var kwotaNaKoncie = kalkulatorEmerytury.ObliczKwoteNaKoncie(request.WynagrodzeniaBrutto, request.WskaznikWaloryzacjiKonta, request.sredniaIlosciDniL4WRoku);
            var kwotaNaSubkoncie = kalkulatorEmerytury.ObliczKwoteNaSubkoncie(request.WynagrodzeniaBrutto, request.WskaznikWaloryzacjiSubkonta, request.sredniaIlosciDniL4WRoku);

            var przewidywaneWyplaty = kalkulatorEmerytury.PrzewidzWynagrodzenia(request.WynagrodzeniaBrutto[request.WynagrodzeniaBrutto.Keys.Max()], request.WynagrodzeniaBrutto.Keys.Max(), 2100);

            var przewidywaneKonto = kalkulatorEmerytury.PrzewidzWartoscNaKoncie(kwotaNaKoncie, request.WynagrodzeniaBrutto[request.WynagrodzeniaBrutto.Keys.Max()], request.WynagrodzeniaBrutto.Keys.Max(), 2100, request.WskaznikWaloryzacjiKonta, request.sredniaIlosciDniL4WRoku);
            var przewidywaneSubkonto = kalkulatorEmerytury.PrzewidzWartoscNaSubkoncie(kwotaNaSubkoncie, request.WynagrodzeniaBrutto[request.WynagrodzeniaBrutto.Keys.Max()], request.WynagrodzeniaBrutto.Keys.Max(), 2100, request.WskaznikWaloryzacjiSubkonta, request.sredniaIlosciDniL4WRoku);

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
                    PrzewidywanaWyplata = przewidywaneWyplaty[i],
                    RR = (wartosc / przewidywaneWyplaty[i]) * 100
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


        [HttpPost("PostRaport")]
        public ActionResult<List<Raport>> PostRaport(RaportRequest request)
        {
            var raport = db.Raports.AsNoTracking().Where(r => r.Data >= request.Od && r.Data <= request.Do).ToList();
            return raport is null ? NotFound() : Ok(raport);
        }


        [HttpGet("ExportRaportExcel")]
        public async Task<IActionResult> ExportRaportExcel([FromQuery] DateTime od, [FromQuery] DateTime do_)
        {
            var raporty = await db.Raports.AsNoTracking()
                .Where(r => r.Data >= od && r.Data <= do_)
                .OrderBy(r => r.Data)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Raporty");

            var row = 1;
            ws.Cell(row, 1).Value = "Data";
            ws.Cell(row, 2).Value = "OczekiwanaEmerytura";
            ws.Cell(row, 3).Value = "Wiek";
            ws.Cell(row, 4).Value = "CzyMezczyzna";
            ws.Cell(row, 5).Value = "Wynagrodzenie";
            ws.Cell(row, 6).Value = "CzyUwzglednialOkresChoroby";
            ws.Cell(row, 7).Value = "SrodkiNaKoncie";
            ws.Cell(row, 8).Value = "SrodkiNaSubkoncie";
            ws.Cell(row, 9).Value = "EmeryturaRzeczywista";
            ws.Cell(row, 10).Value = "EmeryturaUrealniona";
            ws.Cell(row, 11).Value = "KodPocztowy";
            ws.Range(row, 1, row, 11).Style.Font.SetBold(true);
            row++;

            foreach (var r in raporty)
            {
                ws.Cell(row, 1).Value = r.Data;
                ws.Cell(row, 1).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                ws.Cell(row, 2).Value = r.OczekiwanaEmerytura;
                ws.Cell(row, 3).Value = r.Wiek;
                ws.Cell(row, 4).Value = r.CzyMezczyzna;
                ws.Cell(row, 5).Value = r.Wynagrodzenie;
                ws.Cell(row, 6).Value = r.CzyUwzglednialOkresChoroby;
                ws.Cell(row, 7).Value = r.SrodkiNaKoncie;
                ws.Cell(row, 8).Value = r.SrodkiNaSubkoncie;
                ws.Cell(row, 9).Value = r.EmeryturaRzeczywista;
                ws.Cell(row, 10).Value = r.EmeryturaUrealniona;
                ws.Cell(row, 11).Value = r.KodPocztowy;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            var fileName = $"Raport_{od:yyyyMMdd}_{do_:yyyyMMdd}.xlsx";
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(ms.ToArray(), contentType, fileName);
        }
    }
}
