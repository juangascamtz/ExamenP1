using ExamP1JG.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamP1JG.ViewModel
{
    public class resultadosExamViewModel
    {
        public tbl_pregunta Soru { get; set; }
        public tbl_opcion Secenek { get; set; }
    }
}