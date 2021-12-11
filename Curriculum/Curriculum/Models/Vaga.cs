using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Curriculum.Models
{
    public class Vaga
    {
        //variáveis
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
            //atribuição de valores através das propriedades
            NomeEmpresa = nomeEmpresa;
            NomeVaga = nomeVaga;
            Cidade = cidade;
            Estado = estado;
            Descricao = descricao;
            AbrevCurso = abrevCurso;
        }
        public Vaga(string codVaga, string nomeEmpresa, string nomeVaga, string cidade, string estado, string descricao, string abrevCurso)
        {
            //atribuição de valores através das propriedades
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
                //conexão aberta
                con.Open();

                //cadastra a vaga de acordo com os dados informados
                MySqlCommand query = new MySqlCommand("INSERT INTO vaga VALUES (@codVaga, @nomeEmpresa, @nomeVaga, @cidade, @estado, @descricao, @cursoRelacionado)", con);
                query.Parameters.AddWithValue("@codVaga", CodVaga);
                query.Parameters.AddWithValue("@nomeEmpresa", NomeEmpresa);
                query.Parameters.AddWithValue("@nomeVaga", NomeVaga);
                query.Parameters.AddWithValue("@cidade", Cidade);
                query.Parameters.AddWithValue("@estado", Estado);
                query.Parameters.AddWithValue("@descricao", Descricao);
                query.Parameters.AddWithValue("@cursoRelacionado", AbrevCurso);
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

        //método para excluir vagas
        public static bool excluirVaga(string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();
                
                //é excluído todas as candidaturas feitas naquela vaga e depois é excluída a vaga em si
                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_vaga = @codVaga;" +
                    "DELETE FROM vaga WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("codVaga", codVaga);
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

        //método para atualizar a vaga
        public bool atualizarVaga(Vaga vagaAntiga, Vaga vagaNova)
        {
            //resultado original
            bool resultado = false;
            //resultado que informa se devem ser removidas as candidaturas dessa vaga
            bool removerCandidaturas = false;

            //criação de lista com tuplas para armazenar dado e coluna a ser alterada e conexão
            List<Tuple<string, string>> lista = new List<Tuple<string, string>>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //se os nomes da empresa antigo e novo forem diferentes,
            if (vagaAntiga.NomeEmpresa != vagaNova.NomeEmpresa)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.NomeEmpresa, "nome_empresa"));
            }

            //se os nomes da vaga antigo e novo forem diferentes,
            if (vagaAntiga.NomeVaga != vagaNova.NomeVaga)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.NomeVaga, "nome_vaga"));
            }

            //se as cidades antiga e nova forem diferentes,
            if (vagaAntiga.Cidade != vagaNova.Cidade)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.Cidade, "cidade"));
            }

            //se os estados antigo e novo forem diferentes,
            if (vagaAntiga.Estado != vagaNova.Estado)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.Estado, "estado"));
            }

            //se as descrições antiga e nova forem diferentes,
            if (vagaAntiga.Descricao != vagaNova.Descricao)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.Descricao, "descricao"));
            }

            //se os cursos relacionados antigo e novo forem diferentes,
            if (vagaAntiga.AbrevCurso != vagaNova.AbrevCurso)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(vagaNova.AbrevCurso, "cod_curso"));
                //as candidaturas dessa vaga devem ser removidas
                removerCandidaturas = true;
            }

            //para cada dado a ser alterado,
            foreach (var item in lista)
            {
                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza o campo de acordo com o dado informado
                    MySqlCommand query = new MySqlCommand(string.Format("UPDATE vaga SET {0} = @dado WHERE cod_vaga = @codVaga", item.Item2), con);
                    //os parâmetros são obtidos das tuplas da lista
                    query.Parameters.AddWithValue("@dado", item.Item1);
                    query.Parameters.AddWithValue("@codVaga", vagaAntiga.CodVaga);
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

            //se as candidaturas precisarem ser removidas,
            if (removerCandidaturas)
            {
                try
                {
                    //conexão aberta
                    con.Open();
                    //remove todas as candidaturas de acordo com o dado informado
                    MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_vaga = @codVaga", con);
                    query.Parameters.AddWithValue("@codVaga", vagaAntiga.CodVaga);
                    //execução do código
                    query.ExecuteNonQuery();
                    //conexão fechada
                    con.Close();

                    //resultado passa a ser verdadeiro
                    resultado = true;
                }
                catch
                {
                    //em caso de erro, o resultado não é alterado
                }
            }
            //retorna o resultado
            return resultado;
        }

        //método para buscar dados da vaga
        public Vaga buscarDados(string codVaga)
        {
            //criação do objeto 'vaga' e conexão
            Vaga vaga = null;
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();
                //seleciona tudo da tabela 'vaga' onde o a chave primária for igual à informada
                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codVaga", codVaga);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //atribuição de valores às prorpiedades da vaga
                    CodVaga = leitor["cod_vaga"].ToString();
                    NomeEmpresa = leitor["nome_empresa"].ToString();
                    NomeVaga = leitor["nome_vaga"].ToString();
                    Cidade = leitor["cidade"].ToString();
                    Estado = leitor["estado"].ToString();
                    Descricao = leitor["descricao"].ToString();
                    AbrevCurso = leitor["cod_curso"].ToString();

                    //os dados são atribuídos ao objeto
                    vaga = new Vaga(CodVaga, NomeEmpresa, NomeVaga, Cidade, Estado, Descricao, AbrevCurso);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //caso dê erro, a vaga é nula
                vaga = null;
            }
            //retorna o objeto 'vaga'
            return vaga;
        }

        //método para listar todas as vagas
        public static List<Vaga> listarVagas()
        {
            //criação da lista de cursos e conexão
            List<Vaga> listaVagas = new List<Vaga>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //seleciona tudo da tabela 'vaga'
                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga", con);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //os dados são atribuídos à variáveis
                    string codVaga = leitor["cod_vaga"].ToString();
                    string nomeEmpresa = leitor["nome_empresa"].ToString();
                    string nomeVaga = leitor["nome_vaga"].ToString();
                    string cidade = leitor["cidade"].ToString();
                    string estado = leitor["estado"].ToString();
                    string descricao = leitor["descricao"].ToString();
                    string codCurso = leitor["cod_curso"].ToString();

                    //criado um objeto para a vaga
                    Vaga vaga = new Vaga(codVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, codCurso);
                    //vaga adicionada à lista
                    listaVagas.Add(vaga);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, a lista de vagas é nula
                listaVagas = null;
            }
            //por fim, retorna a lista de vagas
            return listaVagas;
        }

        public static List<Vaga> listarVagasCandidatadas()
        {
            //criação da lista de cursos e conexão
            List<Vaga> listaVagas = new List<Vaga>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //seleciona tudo da tabela 'vaga'
                MySqlCommand query = new MySqlCommand("SELECT cod_vaga FROM candidatura", con);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();
                //lista de códigos de vagas temporária
                List<string> listaCodigos = new List<string>();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //adiciona o código da vaga à lista temporária
                    listaCodigos.Add(leitor["cod_vaga"].ToString());
                }
                //conexão fechada
                con.Close();

                //remove os dados duplicados da lista
                listaCodigos = listaCodigos.Distinct().ToList();

                //para cada código na lista de códigos,
                foreach (string codigoVaga in listaCodigos)
                {
                    //cria uma vaga com o código e adiciona à lista de vagas
                    Vaga vaga = new Vaga().buscarDados(codigoVaga);
                    listaVagas.Add(vaga);
                }
            }
            catch
            {
                //em caso de erro, a lista de vagas é nula
                listaVagas = null;
            }
            //por fim, retorna a lista de vagas
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
                //conexão aberta
                con.Open();

                //seleciona tudo de acordo com o curso informado
                MySqlCommand query = new MySqlCommand("SELECT * FROM vaga WHERE cod_curso = @codCurso", con);
                query.Parameters.AddWithValue("@codCurso", codCurso);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //os dados são atribuídos à variáveis
                    string codVaga = leitor["cod_vaga"].ToString();
                    string nomeEmpresa = leitor["nome_empresa"].ToString();
                    string nomeVaga = leitor["nome_vaga"].ToString();
                    string cidade = leitor["cidade"].ToString();
                    string estado = leitor["estado"].ToString();
                    string descricao = leitor["descricao"].ToString();
                    codCurso = leitor["cod_curso"].ToString();

                    //criado um objeto para a vaga
                    Vaga vaga = new Vaga(codVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, codCurso);
                    //vaga adicionada à lista
                    listaVagas.Add(vaga);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, a lista de vagas é nula
                listaVagas = null;
            }
            //retorna a lista de vagas
            return listaVagas;
        }

        //método para registrar uma candidatura
        public static bool inserirCandidatura(string codAluno, string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //cadastra a candidatura de acordo com os dados informados
                MySqlCommand query = new MySqlCommand("INSERT INTO candidatura VALUES(@codAluno, @codVaga)", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);
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

        //método para remover candidatura
        public static bool removerCandidatura(string codAluno, string codVaga)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //remove a candidatura de acordo com os códigos de vaga e aluno
                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_aluno = @codAluno AND cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);
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

        //método que checa se o aluno se candidatou em determinada vaga
        public static string seCandidatou(string codAluno, string codVaga)
        {
            //criação do resultado e conexão
            string resultado = null;
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
  
            try
            {
                //conexão aberta
                con.Open();

                //seleciona tudo da candidatura de acordo com os códigos de vaga e aluno
                MySqlCommand query = new MySqlCommand("SELECT * FROM candidatura WHERE cod_aluno = @codAluno AND cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@codVaga", codVaga);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //se o leitor encontrar dados,
                if (leitor.HasRows)
                {
                    //o resultado é verdadeiro
                    resultado = "verdadeiro";
                }
                else
                {
                    //caso contrário o resultado é falso
                    resultado = "falso";
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, o resultado é falso
                resultado = "falso";
            }
            //retorna o resultado
            return resultado;
        }

        //método para listar todos candidatos em determinada vaga
        public static List<Aluno> listarCandidatos(string codVaga)
        {
            //criação de lista com os alunos candidatados e conexão
            List<Aluno> alunosCandidatos = new List<Aluno>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //lista temporária com os códigos do aluno
                List<string> codigoAlunos = new List<string>();
                //conexão aberta
                con.Open();

                //seleciona todas as candidaturas de acordo com o código da vaga
                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM candidatura WHERE cod_vaga = @codVaga", con);
                query.Parameters.AddWithValue("@codVaga", codVaga);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //adicionado o código do aluno à lista temporária
                    string codAluno = leitor["cod_aluno"].ToString();
                    codigoAlunos.Add(codAluno);
                }
                //conexão fechada
                con.Close();

                //para cada código na lista temporária de códigos,
                foreach (string cod in codigoAlunos)
                {
                    //é criado um objeto 'aluno' e passado para a lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    alunosCandidatos.Add(aluno);
                }
            }
            catch
            {
                //em caso de erro, a lista de alunos candidatados é nula
                alunosCandidatos = null;
            }
            //retorna a lista de alunos candidatados
            return alunosCandidatos;
        }

        //método para remover todas as candidaturas de um aluno
        public static bool removerTodasCandidaturas(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //exclui todas as candidaturas registradas com o mesmo código de aluno
                MySqlCommand query = new MySqlCommand("DELETE FROM candidatura WHERE cod_aluno = @codAluno", con);
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

        //método para listar todas as vagas nas quais um aluno se candidatou
        public static List<Vaga> listarVagasCandidatadas(string codAluno)
        {
            //criação da lista de vagas e conexão
            List<Vaga> listaVagas = new List<Vaga>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //seleciona as candidaturas registradas com o código do aluno informado
                MySqlCommand query = new MySqlCommand("SELECT cod_vaga FROM candidatura WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //lista temporária de códigos da vaga
                List<string> codigosVaga = new List<string>();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //adicionado o código da vaga à lista temporária
                    string codigo = leitor["cod_vaga"].ToString();
                    codigosVaga.Add(codigo);
                }
                //conexão fechada
                con.Close();

                //para cada código na lista temporária,
                foreach (string cod in codigosVaga)
                {
                    //criado um objeto 'vaga' e adicionado à lista
                    Vaga vaga = new Vaga().buscarDados(cod);
                    listaVagas.Add(vaga);
                }
            }
            catch
            {
                //em caso de erro, a lista é nula
                listaVagas = null;
            }
            //retorna a lista
            return listaVagas;
        }
    }
}