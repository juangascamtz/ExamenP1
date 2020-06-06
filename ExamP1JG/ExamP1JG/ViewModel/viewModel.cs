using ExamP1JG.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamP1JG.ViewModel
{
    public class viewModel
    {

        public IEnumerable<tbl_categoria> Kategori { get; set; }
        public tbl_pregunta Soru { get; set; }
        public tbl_opcion Secenek { get; set; }
    }
}