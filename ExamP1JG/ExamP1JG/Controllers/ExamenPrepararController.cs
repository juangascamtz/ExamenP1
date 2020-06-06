using ExamP1JG.Models.EntityFramework;
using ExamP1JG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamP1JG.Controllers
{
    public class ExamenPrepararController : Controller
    {
        examenDBEntities db = new examenDBEntities();
        // GET: ExamenPreparar
        public ActionResult CrearCuestionario()
        {
            var model = new viewModel()
            {
                Kategori = db.tbl_categoria.ToList(),
                Soru = new tbl_pregunta(),
                Secenek = new tbl_opcion()
            };
            return View("CrearCuestionario", model);
        }
        [ValidateAntiForgeryToken]
        public ActionResult GuardarPregunta(tbl_pregunta soru, tbl_opcion secenek)
        {
            mesajViewModel mesajModel = new mesajViewModel();

            if (!ModelState.IsValid)
            {
                var model_ = new viewModel()
                {
                    Kategori = db.tbl_categoria.ToList(),
                    Soru = new tbl_pregunta(),
                    Secenek = new tbl_opcion()

                };

                return View("QuizOlustur", model_);
            }
            else
            {
                soru.preguntaUniq = Guid.NewGuid();
                secenek.preguntaUniq = soru.preguntaUniq;
                soru.descripcion = 0;
                db.tbl_pregunta.Add(soru);
                db.tbl_opcion.Add(secenek);


            }
            db.SaveChanges();

            mesajModel.Mesaj = "Pregunta Agregada";
            mesajModel.Status = 1;
            mesajModel.LinkText = "Añadir otra pregunta";
            mesajModel.Url = "/ExamenPreparar/CrearCuestionario";


            return View("_mesaj", mesajModel);


        }

        public ActionResult ListarPreguntas()
        {

            var model = db.tbl_opcion.Include("tbl_pregunta").ToList();

            return View(model);
        }


    }
}