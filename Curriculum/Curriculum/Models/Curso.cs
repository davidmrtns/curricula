using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Curriculum.Models
{
    public class Curso
    {
        //variáveis
        private string nomeCurso, abrevCurso;

        //propriedades
        public string NomeCurso { get => nomeCurso; set => nomeCurso = value; }
        public string AbrevCurso { get => abrevCurso; set => abrevCurso = value; }

        //construtor
        public Curso(string nomeCurso, string abrevCurso)
        {
            //atribuição de valores através das propriedades
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
                //conexão aberta
                con.Open();
                //inserindo dados através de parâmetros
                MySqlCommand query = new MySqlCommand("INSERT INTO curso VALUES(@codigo, @nomeCurso)", con);
                query.Parameters.AddWithValue("@codigo", AbrevCurso);
                query.Parameters.AddWithValue("@nomeCurso", NomeCurso);

                //executando comando
                query.ExecuteNonQuery();
                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //retorna falso em caso de erro
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
                //conexão aberta
                con.Open();
                //inserindo dados através de parâmetros
                MySqlCommand query = new MySqlCommand("INSERT INTO cursofeito VALUES(@codAluno, @codCurso)", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codCurso", codCurso);
                //executando comando
                query.ExecuteNonQuery();

                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //em caso de erro, retorna falso
                return false;
            }
        }

        //método para buscar todos os cursos cadastrados
        public static List<Curso> listarCursos()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            //criação da lista de cursos
            List<Curso> listaCursos = new List<Curso>();

            try
            {
                //conexão aberta
                con.Open();

                //seleciona tudo da tabela 'curso'
                MySqlCommand query = new MySqlCommand("SELECT * FROM curso", con);
                //criação do leitor de dados
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //criado objeto 'curso' com os dados do banco
                    string abrevCurso = leitor["cod_curso"].ToString();
                    string nomeCurso = leitor["nome_curso"].ToString();
                    Curso curso = new Curso(nomeCurso, abrevCurso);

                    //adicionado o objeto à lista
                    listaCursos.Add(curso);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, a lista é nula
                listaCursos = null;
            }
            //por fim, retorna a lista
            return listaCursos;
        }

        //método para buscar dados de um curso cadastrado
        public static Curso buscarDadosCurso(string codCurso)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            //criação da lista de cursos
            Curso curso = null;

            try
            {
                //conexão aberta
                con.Open();

                //seleciona tudo da tabela 'curso'
                MySqlCommand query = new MySqlCommand("SELECT * FROM curso WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);
                //criação do leitor de dados
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //criado objeto 'curso' com os dados do banco
                    string abrevCurso = leitor["cod_curso"].ToString();
                    string nomeCurso = leitor["nome_curso"].ToString();
                    curso = new Curso(nomeCurso, abrevCurso);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, a lista é nula
                curso = null;
            }
            //por fim, retorna a lista
            return curso;
        }

        //método para apagar o curso cadastrado
        public bool apagarCurso()
        {
            //criação de listas com todos alunos e vagas registrados com aquele curso
            List<string> codAluno = buscarAlunosCurso(AbrevCurso);
            List<Vaga> listaVagas = Vaga.buscarVagasCurso(AbrevCurso);

            //para cada código na lista de códigos de aluno,
            foreach (string cod in codAluno)
            {
                //o curso feito pelo aluno é removido
                apagarCursoFeito(cod);
            }
            //para cada vaga na lista de vagas,
            foreach (Vaga vaga in listaVagas)
            {
                //a vaga é excluída
                Vaga.excluirVaga(vaga.CodVaga);
            }

            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //apaga o curso de acordo com o código do curso informado
                MySqlCommand query = new MySqlCommand("DELETE FROM curso WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", AbrevCurso);
                //execução do código
                query.ExecuteNonQuery();

                //conexão fechada
                con.Close();

                //retorna uma mensagem de sucesso
                return true;
            }
            catch
            {
                //em caso de erro, retorna falso
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
                //conexão aberta
                con.Open();

                //apaga o curso feito de acordo com o código do aluno
                MySqlCommand query = new MySqlCommand("DELETE FROM cursofeito WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                //execução do código
                query.ExecuteNonQuery();

                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //em caso de erro, retorna falso
                return false;
            }
        }

        //método para atualizar o curso feito
        public static bool atualizarCursoFeito(string codCursoAntigo, string codCursoNovo, string codAluno)
        {
            //resultado original
            bool resultado = false;

            //se os cursos antigo e novo forem diferentes,
            if (codCursoAntigo != codCursoNovo)
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza o curso feito de acordo com o código do aluno
                    MySqlCommand query = new MySqlCommand("UPDATE cursofeito SET cod_curso = @codCurso WHERE cod_aluno = @codAluno", con);
                    query.Parameters.AddWithValue("@codCurso", codCursoNovo);
                    query.Parameters.AddWithValue("@codAluno", codAluno);
                    //execução do código
                    query.ExecuteNonQuery();
                    //conexão fechada
                    con.Close();

                    //resultado passa a ser verdadeiro
                    resultado = true;
                }
                catch
                {
                    //em caso de erro, o resultado continua a ser falso
                    resultado = false;
                }
            }
            //retorna o resultado
            return resultado;
        }

        //método para buscar todos os alunos cadastrados em determinado curso
        public List<string> buscarAlunosCurso(string codCurso)
        {
            //criação da lista de códigos de alunos e conexão
            List<string> listaCod = new List<string>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //seleciona todos os alunos cadastrados no curso informado
                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM cursofeito WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);
                //execução do código
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor ler,
                while (leitor.Read())
                {
                    //o código do aluno é adicionado na lista
                    listaCod.Add(leitor["cod_aluno"].ToString());
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //caso dê errado, a lista é nula
                listaCod = null;
            }
            //retorna a lista de códigos
            return listaCod;
        }

        //método para buscar o curso feito por um aluno
        public static Curso buscarCursoFeito(string codAluno)
        {
            //criação do objeto 'curso' e conexão
            Curso curso = null;
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //selecionado o curso feito de acordo com o código informado
                MySqlCommand query = new MySqlCommand("SELECT cod_curso FROM cursofeito WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //são buscados os dados do curso e atribuídos ao objeto
                    curso = buscarDadosCurso(leitor["cod_curso"].ToString());
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, o objeto 'curso' é nulo
                curso = null;
            }
            //retorna o curso
            return curso;
        }
    }
}