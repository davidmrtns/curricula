using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Curricula.Models
{
    public class Curso
    {
        private string nomeCurso, abrevCurso;

        //propriedades
        public string NomeCurso { get => nomeCurso; set => nomeCurso = value; }
        public string AbrevCurso { get => abrevCurso; set => abrevCurso = value; }

        //construtor
        public Curso(string nomeCurso, string abrevCurso)
        {
            NomeCurso = nomeCurso;
            AbrevCurso = abrevCurso;
        }

        //método para cadastrar cursos no banco
        public bool inserirCurso()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO curso VALUES(@codigo, @nomeCurso)", con);
                query.Parameters.AddWithValue("@codigo", AbrevCurso);
                query.Parameters.AddWithValue("@nomeCurso", NomeCurso);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para cadastrar cursos realizados pelo aluno no banco
        public static bool inserirCursoFeito(string codAluno, string codCurso)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO cursofeito VALUES(@codAluno, @codCurso)", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codCurso", codCurso);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para buscar todos os cursos cadastrados
        public static List<Curso> listarCursos()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            List<Curso> listaCursos = new List<Curso>();

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM curso", con);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    string abrevCurso = leitor["cod_curso"].ToString();
                    string nomeCurso = leitor["nome_curso"].ToString();
                    Curso curso = new Curso(nomeCurso, abrevCurso);

                    //adicionado o curso à lista
                    listaCursos.Add(curso);
                }

                con.Close();
            }
            catch
            {
                listaCursos = null;
            }

            return listaCursos;
        }

        //método para buscar dados de um curso cadastrado
        public static Curso buscarDadosCurso(string codCurso)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            Curso curso = null;

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM curso WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    //o curso é adicionado à lista
                    string abrevCurso = leitor["cod_curso"].ToString();
                    string nomeCurso = leitor["nome_curso"].ToString();
                    curso = new Curso(nomeCurso, abrevCurso);
                }

                con.Close();
            }
            catch
            {
                curso = null;
            }

            return curso;
        }

        //método para apagar o curso cadastrado
        public bool apagarCurso()
        {
            //criação de listas com todos alunos e vagas registrados com aquele curso
            List<string> codAluno = buscarAlunosCurso(AbrevCurso);
            List<Vaga> listaVagas = Vaga.buscarVagasCurso(AbrevCurso);

            foreach (string cod in codAluno)
            {
                //o curso feito é removido do cadastro de todos os alunos desse curso
                apagarCursoFeito(cod);
            }

            foreach (Vaga vaga in listaVagas)
            {
                //cada vaga relacionada ao curso é excluída
                Vaga.excluirVaga(vaga.CodVaga);
            }

            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM curso WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", AbrevCurso);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para remover o curso feito pelo aluno
        public static bool apagarCursoFeito(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM cursofeito WHERE cod_aluno = @codAluno", con);
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

        //método para atualizar o curso feito
        public static bool atualizarCursoFeito(string codCursoAntigo, string codCursoNovo, string codAluno)
        {
            bool resultado = false;

            if (codCursoAntigo != codCursoNovo)
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    con.Open();

                    //atualiza o curso feito de acordo com o código do aluno
                    MySqlCommand query = new MySqlCommand("UPDATE cursofeito SET cod_curso = @codCurso WHERE cod_aluno = @codAluno", con);
                    query.Parameters.AddWithValue("@codCurso", codCursoNovo);
                    query.Parameters.AddWithValue("@codAluno", codAluno);

                    query.ExecuteNonQuery();
                    con.Close();

                    resultado = true;
                }
                catch
                {
                    resultado = false;
                }
            }

            return resultado;
        }

        //método para buscar todos os alunos cadastrados em determinado curso
        public List<string> buscarAlunosCurso(string codCurso)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            List<string> listaCod = new List<string>();

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM cursofeito WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    //o código do aluno é adicionado na lista
                    listaCod.Add(leitor["cod_aluno"].ToString());
                }

                con.Close();
            }
            catch
            {
                listaCod = null;
            }

            return listaCod;
        }

        //método para buscar o curso feito por um aluno
        public static Curso buscarCursoFeito(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            Curso curso = null;

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_curso FROM cursofeito WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    //o curso é criado com os dados do banco
                    curso = buscarDadosCurso(leitor["cod_curso"].ToString());
                }

                con.Close();
            }
            catch
            {
                curso = null;
            }

            return curso;
        }
    }
}