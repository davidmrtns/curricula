using System.Collections.Generic;
using System.Web.Mvc;
using Curricula.Classes;

namespace Curricula.Models
{
    public class Itens
    {
        //lista de estados para popular tags select
        private SelectListItem[] estados = new SelectListItem[]
        {
            new SelectListItem { Value = "", Text = "--" },
            new SelectListItem { Value = "AL", Text = "AL" },
            new SelectListItem { Value = "AP", Text = "AP" },
            new SelectListItem { Value = "AM", Text = "AM" },
            new SelectListItem { Value = "BA", Text = "BA" },
            new SelectListItem { Value = "CE", Text = "CE" },
            new SelectListItem { Value = "DF", Text = "DF" },
            new SelectListItem { Value = "ES", Text = "ES" },
            new SelectListItem { Value = "GO", Text = "GO" },
            new SelectListItem { Value = "MA", Text = "MA" },
            new SelectListItem { Value = "MT", Text = "MT" },
            new SelectListItem { Value = "MS", Text = "MS" },
            new SelectListItem { Value = "MG", Text = "MG" },
            new SelectListItem { Value = "PA", Text = "PA" },
            new SelectListItem { Value = "PB", Text = "PB" },
            new SelectListItem { Value = "PR", Text = "PR" },
            new SelectListItem { Value = "PE", Text = "PE" },
            new SelectListItem { Value = "PI", Text = "PI" },
            new SelectListItem { Value = "RJ", Text = "RJ" },
            new SelectListItem { Value = "RN", Text = "RN" },
            new SelectListItem { Value = "RS", Text = "RS" },
            new SelectListItem { Value = "RO", Text = "RO" },
            new SelectListItem { Value = "RR", Text = "RR" },
            new SelectListItem { Value = "SC", Text = "SC" },
            new SelectListItem { Value = "SP", Text = "SP" },
            new SelectListItem { Value = "SE", Text = "SE" },
            new SelectListItem { Value = "TO", Text = "TO" },
        };

        //método para criar lista de cursos para select
        public SelectListItem[] cursos(List<Curso> listaCursos)
        {
            //criação da lista de itens do select e adição do valor padrão
            List<SelectListItem> lista = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "--" }
            };

            if (listaCursos != null && listaCursos.Count > 0)
            {
                foreach (Curso curso in listaCursos)
                {
                    //cria um item da tag select e adiciona à lista
                    SelectListItem item = new SelectListItem { Value = curso.AbrevCurso, Text = string.Format("{0} ({1})", curso.NomeCurso, curso.AbrevCurso) };
                    lista.Add(item);
                }
            }

            return lista.ToArray();
        }

        //propriedade para obter a lista de estados
        public SelectListItem[] Estados { get => estados; }
    }
}