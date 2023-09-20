using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Curricula.Models
{
    public static class Conexao
    {
        //código de conexão ao banco de dados
        public static readonly string codConexao = "Server=az-mysql-curriculum.mysql.database.azure.com; Port=3306; Database=bdcurriculum; User id=curriculumdev; Password=#Azuredb!3371";
    }
}