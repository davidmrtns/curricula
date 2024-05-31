using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Curricula.Classes;

namespace Curricula.Models
{
    public class Vaga
    {
        private string codVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, abrevCurso;

        //propriedades
        public string CodVaga { get => codVaga; set => codVaga = value; }
        public string NomeEmpresa { get => nomeEmpresa; set => nomeEmpresa = value; }
        public string NomeVaga { get => nomeVaga; set => nomeVaga = value; }
        public string Cidade { get => cidade; set => cidade = value; }
        public string Estado { get => estado; set => estado = value; }
        public string Descricao { get => descricao; set => descricao = value; }
        public string AbrevCurso { get => abrevCurso; set => abrevCurso = value; }

        //construtores
        public Vaga() { }
        public Vaga(string nomeEmpresa, string nomeVaga, string cidade, string estado, string descricao, string abrevCurso)
        {
            NomeEmpresa = nomeEmpresa;
            NomeVaga = nomeVaga;
            Cidade = cidade;
            Estado = estado;
            Descricao = descricao;
            AbrevCurso = abrevCurso;
        }
        public Vaga(string codVaga, string nomeEmpresa, string nomeVaga, string cidade, string estado, string descricao, string abrevCurso)
        {
            CodVaga = codVaga;
            NomeEmpresa = nomeEmpresa;
            NomeVaga = nomeVaga;
            Cidade = cidade;
            Estado = estado;
            Descricao = descricao;
            AbrevCurso = abrevCurso;
        }

        //método para cadastrar vagas
        public bool inserirVaga()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO vaga VALUES (@codVaga, @nomeEmpresa, @nomeVaga, @cidade, @estado, @descricao, @cursoRelacionado)", con);
                query.Parameters.AddWithValue("@codVaga", CodVaga);
                query.Parameters.AddWithValue("@nomeEmpresa", NomeEmpresa);
                query.Parameters.AddWithValue("@nomeVaga", NomeVaga);
                query.Parameters.AddWithValue("@cidade", Cidade);
                query.Parameters.AddWithValue("@estado", Estado);
                query.Parameters.AddWithValue("@descricao", Descricao);
                query.Parameters.AddWithValue("@cursoRelacionado", AbrevCurso);

                //execução da query
                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para excluir vagas
        public static bool excluirVaga(string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();
                
                //é excluído todas as candidaturas feitas naquela vaga e depois a vaga
                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_vaga = @codVaga;" +
                    "DELETE FROM vaga WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("codVaga", codVaga);
                
                //execução da query
                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para atualizar a vaga
        public bool atualizarVaga(Vaga vagaAntiga, Vaga vagaNova)
        {
            bool resultado = false;
            //variável que servirá para dizer se serão removidas as candidaturas na vaga (caso mude de curso relacionado)
            bool removerCandidaturas = false;

            //criação de lista com tuplas para armazenar dado e coluna a ser alterada
            List<Tuple<string, string>> lista = new List<Tuple<string, string>>();

            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //se os nomes antigo e diferente forem diferentes, é adicionado à tupla o dado novo e o nome da coluna para ser alterada
            if (vagaAntiga.NomeEmpresa != vagaNova.NomeEmpresa)
            {
                lista.Add(new Tuple<string, string>(vagaNova.NomeEmpresa, "nome_empresa"));
            }

            if (vagaAntiga.NomeVaga != vagaNova.NomeVaga)
            {
                lista.Add(new Tuple<string, string>(vagaNova.NomeVaga, "nome_vaga"));
            }

            if (vagaAntiga.Cidade != vagaNova.Cidade)
            {
                lista.Add(new Tuple<string, string>(vagaNova.Cidade, "cidade"));
            }

            if (vagaAntiga.Estado != vagaNova.Estado)
            {
                lista.Add(new Tuple<string, string>(vagaNova.Estado, "estado"));
            }

            if (vagaAntiga.Descricao != vagaNova.Descricao)
            {
                lista.Add(new Tuple<string, string>(vagaNova.Descricao, "descricao"));
            }

            if (vagaAntiga.AbrevCurso != vagaNova.AbrevCurso)
            {
                lista.Add(new Tuple<string, string>(vagaNova.AbrevCurso, "cod_curso"));
                //as candidaturas dessa vaga devem ser removidas
                removerCandidaturas = true;
            }

            //faz a alteração para cada dado novo
            foreach (var item in lista)
            {
                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza o campo de acordo com o dado e coluna informados
                    MySqlCommand query = new MySqlCommand(string.Format("UPDATE vaga SET {0} = @dado WHERE cod_vaga = @codVaga", item.Item2), con);
                    query.Parameters.AddWithValue("@dado", item.Item1);
                    query.Parameters.AddWithValue("@codVaga", vagaAntiga.CodVaga);
                    
                    //execução da query
                    query.ExecuteNonQuery();
                    con.Close();

                    resultado = true;
                }
                catch
                {
                    resultado = false;
                }
            }

            //remoção das candidaturas (em caso de novo curso relacioando)
            if (removerCandidaturas)
            {
                try
                {
                    //conexão aberta
                    con.Open();

                    MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_vaga = @codVaga", con);
                    query.Parameters.AddWithValue("@codVaga", vagaAntiga.CodVaga);
                    
                    query.ExecuteNonQuery();
                    con.Close();

                    resultado = true;
                }
                catch
                {
                    //em caso de erro, o resultado não é alterado
                }
            }

            return resultado;
        }

        //método para buscar dados da vaga
        public Vaga buscarDados(string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            Vaga vaga = null;
            
            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codVaga", codVaga);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    CodVaga = leitor["cod_vaga"].ToString();
                    NomeEmpresa = leitor["nome_empresa"].ToString();
                    NomeVaga = leitor["nome_vaga"].ToString();
                    Cidade = leitor["cidade"].ToString();
                    Estado = leitor["estado"].ToString();
                    Descricao = leitor["descricao"].ToString();
                    AbrevCurso = leitor["cod_curso"].ToString();

                    //vaga criada com os dados do banco
                    vaga = new Vaga(CodVaga, NomeEmpresa, NomeVaga, Cidade, Estado, Descricao, AbrevCurso);
                }

                con.Close();
            }
            catch
            {
                vaga = null;
            }

            return vaga;
        }

        //método para listar todas as vagas
        public static List<Vaga> listarVagas()
        {
            //criação da conexão
            List<Vaga> listaVagas = new List<Vaga>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga", con);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    string codVaga = leitor["cod_vaga"].ToString();
                    string nomeEmpresa = leitor["nome_empresa"].ToString();
                    string nomeVaga = leitor["nome_vaga"].ToString();
                    string cidade = leitor["cidade"].ToString();
                    string estado = leitor["estado"].ToString();
                    string descricao = leitor["descricao"].ToString();
                    string codCurso = leitor["cod_curso"].ToString();

                    //a vaga é icionada à lista
                    Vaga vaga = new Vaga(codVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, codCurso);
                    listaVagas.Add(vaga);
                }

                con.Close();
            }
            catch
            {
                listaVagas = null;
            }

            return listaVagas;
        }

        //método para listar vagas com candidatos
        public static List<Vaga> listarVagasCandidatadas()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            List<Vaga> listaVagas = new List<Vaga>();

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_vaga FROM candidatura", con);

                MySqlDataReader leitor = query.ExecuteReader();

                //lista de códigos de vagas temporária
                List<string> listaCodigos = new List<string>();

                while (leitor.Read())
                {
                    //adiciona o código da vaga à lista temporária
                    listaCodigos.Add(leitor["cod_vaga"].ToString());
                }

                con.Close();

                //remove os dados duplicados da lista
                listaCodigos = listaCodigos.Distinct().ToList();

                foreach (string codigoVaga in listaCodigos)
                {
                    //cria uma vaga com o código e adiciona à lista de vagas
                    Vaga vaga = new Vaga().buscarDados(codigoVaga);
                    listaVagas.Add(vaga);
                }
            }
            catch
            {
                listaVagas = null;
            }

            return listaVagas;
        }

        //método que busca todas as vagas de acordo com um curso
        public static List<Vaga> buscarVagasCurso(string codCurso)
        {
            //criação da lista de cursos e conexão
            List<Vaga> listaVagas = new List<Vaga>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    string codVaga = leitor["cod_vaga"].ToString();
                    string nomeEmpresa = leitor["nome_empresa"].ToString();
                    string nomeVaga = leitor["nome_vaga"].ToString();
                    string cidade = leitor["cidade"].ToString();
                    string estado = leitor["estado"].ToString();
                    string descricao = leitor["descricao"].ToString();
                    codCurso = leitor["cod_curso"].ToString();

                    //vaga adicionada à lista
                    Vaga vaga = new Vaga(codVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, codCurso);
                    listaVagas.Add(vaga);
                }

                con.Close();
            }
            catch
            {
                listaVagas = null;
            }

            return listaVagas;
        }

        //método para registrar uma candidatura
        public static bool inserirCandidatura(string codAluno, string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO candidatura VALUES(@codAluno, @codVaga)", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para remover candidatura
        public static bool removerCandidatura(string codAluno, string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_aluno = @codAluno AND cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método que checa se o aluno se candidatou em determinada vaga
        public static bool seCandidatou(string codAluno, string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            bool resultado;
  
            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM candidatura WHERE cod_aluno = @codAluno AND cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);

                MySqlDataReader leitor = query.ExecuteReader();

                //se houverem dados, é verdadeiro
                if (leitor.HasRows)
                {
                    resultado = true;
                }
                else
                {
                    resultado = false;
                }

                con.Close();
            }
            catch
            {

                resultado = false;
            }

            return resultado;
        }

        //método para listar todos candidatos em determinada vaga
        public static List<Aluno> listarCandidatos(string codVaga)
        {
            //criação da conexão
            List<Aluno> alunosCandidatos = new List<Aluno>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //lista temporária com os códigos do aluno
                List<string> codigoAlunos = new List<string>();

                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM candidatura WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codVaga", codVaga);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    //adicionado o código do aluno à lista temporária
                    string codAluno = leitor["cod_aluno"].ToString();
                    codigoAlunos.Add(codAluno);
                }

                con.Close();

                foreach (string cod in codigoAlunos)
                {
                    //para cada código, é criado um objeto aluno e passado para a lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    alunosCandidatos.Add(aluno);
                }
            }
            catch
            {
                alunosCandidatos = null;
            }

            return alunosCandidatos;
        }

        //método para remover todas as candidaturas de um aluno
        public static bool removerTodasCandidaturas(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                query.ExecuteNonQuery();

                con.Close();

                return true;
            }
            catch
            {

                return false;
            }
        }

        //método para listar todas as vagas nas quais um aluno se candidatou
        public static List<Vaga> listarVagasCandidatadas(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            List<Vaga> listaVagas = new List<Vaga>();

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_vaga FROM candidatura WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                MySqlDataReader leitor = query.ExecuteReader();

                //lista temporária de códigos da vaga
                List<string> codigosVaga = new List<string>();

                while (leitor.Read())
                {
                    //adicionado o código da vaga à lista temporária
                    string codigo = leitor["cod_vaga"].ToString();
                    codigosVaga.Add(codigo);
                }

                con.Close();

                foreach (string cod in codigosVaga)
                {
                    //para cada código na lista temporária, é criado um objeto 'vaga' e adicionado à lista
                    Vaga vaga = new Vaga().buscarDados(cod);
                    listaVagas.Add(vaga);
                }
            }
            catch
            {
                listaVagas = null;
            }

            return listaVagas;
        }
    }
}