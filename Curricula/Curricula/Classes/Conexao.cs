using System;

namespace Curricula.Classes
{
    public static class Conexao
    {
        //código de conexão ao banco de dados
        public static readonly string codConexao = Environment.GetEnvironmentVariable("CONNECTION_STRING_CURRICULA", EnvironmentVariableTarget.User);
    }
}