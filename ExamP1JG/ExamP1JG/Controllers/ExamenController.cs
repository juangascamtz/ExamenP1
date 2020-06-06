using ExamP1JG.Models.EntityFramework;
using ExamP1JG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamP1JG.Controllers
{
    public class ExamenController : Controller
    {
        [Authorize(Roles = "S")]
        public class QuizController : Controller
        {
            private examenDBEntities db = new examenDBEntities();
            private static int soruSayac = 0;
            private static int sureSayac = 0;

            [HttpGet]
            public ActionResult Quiz()
            {
                var dbgenelSonuc = new tbl_genelResultados();
                var sinavNo = (from gs in db.tbl_genelResultados select gs.quizCount).Max();


                if (sinavNo == null) dbgenelSonuc.quizCount = 1;
                else dbgenelSonuc.quizCount = sinavNo + 1;
                dbgenelSonuc.userid = 2;


                db.tbl_genelResultados.Add(dbgenelSonuc);
                db.SaveChanges();

                sureSayac = 100;
                TempData["sayac"] = sureSayac;


                return RedirectToAction("QuizStart");

            }


            [HttpPost]
            public ActionResult QuizSonuc(string soruUniq, bool sonuc, string sayac)
            {
                var soruID = Guid.Parse(soruUniq);
                var quizCount = (from gs in db.tbl_genelResultados select gs.quizCount).Max();
                var sinavUniq = (from gs in db.tbl_genelResultados where gs.quizCount == quizCount select gs.examenUniq).First();


                tbl_resultadoExam SinavSonuc = new tbl_resultadoExam();
                tbl_pregunta dbsoru = db.tbl_pregunta.Where(s => s.preguntaUniq == soruID).SingleOrDefault();

                if (sonuc) dbsoru.descripcion = 1;
                else dbsoru.descripcion = -1;

                SinavSonuc.preguntaUniq = dbsoru.preguntaUniq;
                SinavSonuc.esCorrecto = sonuc;

                if (sonuc) SinavSonuc.punto = 5;
                else SinavSonuc.punto = 0;

                SinavSonuc.id_categoria = dbsoru.id_categoria;
                SinavSonuc.historialExamen = DateTime.Now;
                SinavSonuc.examenNo = quizCount;
                SinavSonuc.examenUniq = sinavUniq;

                sureSayac = int.Parse(sayac);

                db.tbl_resultadoExam.Add(SinavSonuc);
                db.SaveChanges();

                return RedirectToAction("QuizStart");

            }

            public ActionResult QuizStart()
            {

                mesajViewModel mesajModel = new mesajViewModel();

                soruSayac++;

                while (soruSayac < 7)
                {


                    var quiz = ((from k in db.tbl_categoria
                                 join s in db.tbl_pregunta on k.id_categoria equals s.id_categoria
                                 join sc in db.tbl_opcion on s.preguntaUniq equals sc.preguntaUniq
                                 where s.descripcion == 0 && s.id_categoria == soruSayac

                                 select new
                                 {
                                     quizKategori = k.id_categoria,
                                     quizSoruUniq = s.preguntaUniq,
                                     quizSoru = s.pregunta,
                                     quizCvp1 = sc.respuesta1,
                                     quizCvp2 = sc.respuesta2,
                                     quizCvp3 = sc.respuesta3,
                                     quizCvp4 = sc.respuesta4,
                                     quizDogruCvp = sc.correctaRsp

                                 }).Take(1)).ToList();

                    if (quiz != null)
                    {
                        var model = new quizSonucViewModel()
                        {
                            Soru = new tbl_pregunta(),
                            Secenek = new tbl_opcion()
                        };
                        model.Soru.pregunta = quiz[0].quizSoru;
                        model.Secenek.respuesta1 = quiz[0].quizCvp1;
                        model.Secenek.respuesta2 = quiz[0].quizCvp2;
                        model.Secenek.respuesta3 = quiz[0].quizCvp3;
                        model.Secenek.respuesta4 = quiz[0].quizCvp4;
                        model.Secenek.correctaRsp = quiz[0].quizDogruCvp;
                        model.Soru.preguntaUniq = quiz[0].quizSoruUniq;
                        model.Soru.id_categoria = quiz[0].quizKategori;


                        TempData["sayac"] = sureSayac;
                        return View(model);

                    }
                    else
                    {
                        soruSayac++;
                    }

                }

                mesajModel.Mesaj = "Sınav Tamamlandı...";
                mesajModel.Status = 1;
                mesajModel.LinkText = "Sınav sonucu için profile git";
                mesajModel.Url = "/Profil/GrafikGoster";


                return View("_mesaj", mesajModel);


            }

            public ActionResult Route()
            {
                mesajViewModel mesajModel = new mesajViewModel();


                mesajModel.Mesaj = "Se terminó el tiempo, inicia de nuevo";
                mesajModel.Status = 0;
                mesajModel.LinkText = "Ir a página principal";
                mesajModel.Url = "/Home/Home";

                return View("_mesaj", mesajModel);


            }
        }
    }
}

        

    