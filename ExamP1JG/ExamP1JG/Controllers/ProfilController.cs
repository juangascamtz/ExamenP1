using ExamP1JG.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ExamP1JG.Controllers
{
    public class ProfilController : Controller
    {
        examenDBEntities db = new examenDBEntities();


        // GET: Profil
        public ActionResult MostrarGrafico()
        {
            ViewBag.sinavNo = -1;
            var sinavlar = (db.tbl_genelResultados.Distinct()).ToList();
            return View(sinavlar);

        }

        public ActionResult GrafikGoster_(int? sinavNo)
        {
            ViewBag.sinavNo = sinavNo;
            var sinavlar = (db.tbl_genelResultados.Distinct()).ToList();

            return View("GrafikGoster", sinavlar);

        }

        //Tek bir sınav sonucu grafiği oluşturma - default son sınav
        public ActionResult AnlikGrafik(int? sinavNo)
        {
            List<string> sinavPuan = GetSinavSonuc(sinavNo);

            var chartSonuc = new Chart(500, 400);
            chartSonuc.AddTitle("Sınav Sonuc Grafiği").AddLegend("Kategori")
                .AddSeries("Puan",
                xValue: new[] { "Anlatım Bozukluğu", "Ekler", "Yazım Kuralları", "Ses Bilgisi", "Sözcükte Anlam", "Sözcük Türleri" },
                yValues: sinavPuan)
                .Write();

            var grafik = File(chartSonuc.ToWebImage().GetBytes(), "image/jpeg");
            var sonuc = "<img src='" + grafik + "'/>";
            return grafik;

        }
        //Tüm sınavların ortalama sonuc grafiği oluşturma
        public ActionResult GenelGrafik()
        {

            List<string> genelPuan = GetGenelSonuc();

            var chartSonuc = new Chart(500, 400);
            chartSonuc.AddTitle("Genel Sonuc Grafiği").AddLegend("Kategori")
                .AddSeries("Puan",
                xValue: new[] { "Anlatım Bozukluğu", "Ekler", "Yazım Kuralları", "Ses Bilgisi", "Sözcükte Anlam", "Sözcük Türleri" },
                yValues: genelPuan)
                .Write();

            return File(chartSonuc.ToWebImage().GetBytes(), "image/jpeg");
        }

        //Veritabanından istenilen sınav numarasına göre sonuc çekme
        private List<string> GetSinavSonuc(int? Sinav_no)
        {


            List<string> returnSonuc = new List<string>();

            int? dbSinav;
            if (Sinav_no == -1)
            {
                dbSinav = ((from ss in db.tbl_resultadoExam select ss.examenNo).Distinct()).Max();

            }
            else
            {
                dbSinav = Sinav_no;
            }

            for (int i = 1; i < 7; i++)
            {
                var dbQuiz = (from ss in db.tbl_resultadoExam
                              join k in db.tbl_categoria on ss.id_categoria equals k.id_categoria
                              where ss.examenNo == dbSinav && ss.id_categoria == i

                              select new
                              {
                                  quizPuan = ss.punto,

                              }).ToList();

                if (dbQuiz != null)
                {
                    var puanToplam = dbQuiz.AsEnumerable().Sum(q => q.quizPuan);
                    string b = puanToplam.ToString();
                    returnSonuc.Add(b);
                }
                else returnSonuc.Add(" ");

            }

            return returnSonuc;


        }

        //Veritabanından tüm sınav sınavların sonucuna göre ortalama sonuc çekme
        private List<string> GetGenelSonuc()
        {

            List<string> returnSonuc = new List<string>();

            var dbSinavSayisi = (from ss in db.tbl_resultadoExam select ss.examenNo).Max();

            for (int i = 1; i < 7; i++)
            {
                int? puanToplam = 0;
                for (int j = 1; j < dbSinavSayisi + 1; j++)
                {
                    var dbQuiz = from ss in db.tbl_resultadoExam
                                 join gs in db.tbl_genelResultados on ss.examenNo equals gs.quizCount
                                 where ss.examenNo == j && ss.id_categoria== i

                                 select new
                                 {
                                     quizPuan = ss.punto,

                                 };

                    foreach (var puan in dbQuiz)
                    {
                        puanToplam += puan.quizPuan;

                    }


                }

                if (puanToplam != 0)
                {
                    puanToplam = puanToplam / dbSinavSayisi;
                    string b = puanToplam.ToString();
                    returnSonuc.Add(b);
                }
                else returnSonuc.Add(" ");


            }

            return returnSonuc;

        }
    }
}
