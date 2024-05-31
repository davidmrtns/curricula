using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Curricula.Classes;

namespace Curricula.Models
{
    public class Aluno
    {
        //variáveis
        private string codAluno, nome, bairro, cidadeResidencia, estado, cep, telefone, palavrasChave;
        private DateTime dataNascimento;
        private bool empregavel;
        public long codAlunoTemp;

        //propriedades
        public string CodAluno { get => codAluno; set => codAluno = value; }
        public string Nome { get => nome; set => nome = value; }
        public string Bairro { get => bairro; set => bairro = value; }
        public string CidadeResidencia { get => cidadeResidencia; set => cidadeResidencia = value; }
        public string Estado { get => estado; set => estado = value; }
        public string Cep { get => cep; set => cep = value; }
        public DateTime DataNascimento { get => dataNascimento.Date; set => dataNascimento = value; }
        public string Telefone { get => telefone; set => telefone = value; }
        public string PalavrasChave { get => palavrasChave; set => palavrasChave = value; }
        public bool Empregavel { get => empregavel; set => empregavel = value; }

        //construtores
        public Aluno() { }
        public Aluno(string nome, string bairro, string cidadeResidencia, string estado, string cep, DateTime dataNascimento, string telefone, string palavrasChave, bool empregavel)
        {
            //atribuição de valores através das propriedades
            Nome = nome;
            Bairro = bairro;
            CidadeResidencia = cidadeResidencia;
            Estado = estado;
            Cep = cep;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            PalavrasChave = palavrasChave;
            Empregavel = empregavel;
        }
        public Aluno(string codAluno, string nome, string bairro, string cidadeResidencia, string estado, string cep, DateTime dataNascimento, string telefone, string palavrasChave, bool empregavel)
        {
            //atribuição de valores através das propriedades
            CodAluno = codAluno;
            Nome = nome;
            Bairro = bairro;
            CidadeResidencia = cidadeResidencia;
            Estado = estado;
            Cep = cep;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            PalavrasChave = palavrasChave;
            Empregavel = empregavel;
        }

        //método para cadastrar alunos no banco
        public bool inserirAluno()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO aluno VALUES(@codigo, @nome, @telefone, @bairro, @cidadeResidencia, @estado, @cep, @dataNascimento, @palavrasChave, @empregavel)", con);
                query.Parameters.AddWithValue("@codigo", CodAluno);
                query.Parameters.AddWithValue("@nome", Nome);
                query.Parameters.AddWithValue("@telefone", Telefone);
                query.Parameters.AddWithValue("@bairro", Bairro);
                query.Parameters.AddWithValue("@cidadeResidencia", CidadeResidencia);
                query.Parameters.AddWithValue("@estado", Estado);
                query.Parameters.AddWithValue("@cep", Cep);
                query.Parameters.AddWithValue("@dataNascimento", DataNascimento);
                query.Parameters.AddWithValue("@palavrasChave", PalavrasChave);
                query.Parameters.AddWithValue("@empregavel", Empregavel);

                //execução da query
                query.ExecuteNonQuery();

                //recupera o código do aluno recém-cadastrado
                codAlunoTemp = query.LastInsertedId;

                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para buscar dados do aluno no banco de dados passando o código do aluno como parâmetro
        public Aluno buscarDados(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            Aluno aluno = null;

            try
            {
                //conexão aberta
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM aluno WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                MySqlDataReader leitor = query.ExecuteReader();

                //execução do leitor
                while (leitor.Read())
                {
                    CodAluno = leitor["cod_aluno"].ToString();
                    Nome = leitor["nome_aluno"].ToString();
                    Bairro = leitor["bairro"].ToString();
                    CidadeResidencia = leitor["cidade_res"].ToString();
                    Estado = leitor["estado"].ToString();
                    Cep = leitor["cep"].ToString();
                    DataNascimento = (DateTime)leitor["nascimento"];
                    Telefone = leitor["telefone"].ToString();
                    PalavrasChave = leitor["palavras_chave"].ToString();
                    Empregavel = (bool) leitor["empregavel"];

                    if (Telefone.Length == 10)
                    {
                        Telefone = Telefone.Insert(6, "-");
                    }

                    //criado o objeto aluno com dados do banco
                    aluno = new Aluno(CodAluno, Nome, Bairro, CidadeResidencia, Estado, Cep, DataNascimento, Telefone, PalavrasChave, Empregavel);
                }

                con.Close();
            }
            catch
            {

                aluno = null;
            }

            return aluno;
        }

        //método para buscar informações no banco de dados em tabelas diferentes
        public static List<string> buscarInfo(string tabela, string campo, string dado)
        {
            //criação da lista com os alunos correspondentes
            List<string> listaCodigos = new List<string>();
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();
                MySqlCommand query;

                if (tabela == "cursofeito")
                {
                    //busca o código do aluno de acordo com o curso e estado da coluna empregavel
                    query = new MySqlCommand("SELECT cursofeito.cod_aluno FROM cursofeito INNER JOIN aluno ON cursofeito.cod_aluno = aluno.cod_aluno AND aluno.empregavel = true AND cursofeito.cod_curso = @dado", con);
                }
                else
                {
                    //seleciona o código do aluno de acordo com a tabela e dado informados
                    query = new MySqlCommand(string.Format("SELECT cod_aluno FROM {0} WHERE LOWER({1}) LIKE @dado AND empregavel = true", tabela, campo), con);//ver uma maneira mais profissional de formatar o comando
                    //os dados correspondentes serão todos que iniciarem com o dado informado
                    dado = dado + "%";
                }

                //atribuição de parâmetros ao código
                query.Parameters.AddWithValue("@dado", dado);

                //execução do leitor
                using (MySqlDataReader leitor = query.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        //adiciona à lista o código do aluno
                        listaCodigos.Add(leitor["cod_aluno"].ToString());
                    }
                }

                con.Close();
            }
            catch
            {
                listaCodigos = null;
            }

            return listaCodigos;
        }

        //método para buscar alunos com palavras-chave correspondentes
        public static List<string> buscarPalavrasChave(string palavrasChave)
        {
            //criação da lista com os alunos correspondentes e lista de listas (filtro cumulativo)
            List<string> listaCodigos = new List<string>();
            List<List<string>> listaListas = new List<List<string>>();

            //as palavras-chave são divididas e passadas para um array
            string[] arrayPalavras = palavrasChave.Split(';');

            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //para cada palavra chave não vazia,
                foreach (string palavraChave in arrayPalavras)
                {
                    if (palavraChave != "")
                    {
                        //seleciona o código do aluno onde houver correspondência da palavra-chave
                        MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM aluno WHERE INSTR(palavras_chave, @dado) AND empregavel = true", con);
                        query.Parameters.AddWithValue("@dado", palavraChave);

                        //lista de códigos temporária
                        List<string> listaTemp = new List<string>();

                        //execução do leitor
                        using (MySqlDataReader leitor = query.ExecuteReader())
                        {
                            while (leitor.Read())
                            {
                                //adicionado à lista temporária o código do aluno correspondente
                                listaTemp.Add(leitor["cod_aluno"].ToString());
                            }
                        }

                        //adiciona a lista temporária à lista de listas
                        listaListas.Add(listaTemp);
                    }
                }

                con.Close();

                if (listaListas.Count >= 1)
                {
                    for (int ii = 0; ii < listaListas.Count() || ii > listaListas.Count() || ii == listaListas.Count(); ii++)
                    {
                        int index = listaListas.Count();

                        if (index != 1)
                        {
                            //as duas primeiras listas se interceptam e geram uma nova lista
                            listaListas.Add(listaListas[0].Intersect(listaListas[1]).ToList());
                            //remoção das duas primeiras listas
                            listaListas.Remove(listaListas[0]);
                            listaListas.Remove(listaListas[0]);
                        }
                        else
                        {
                            listaCodigos = listaListas[0];
                            break;
                        }
                    }
                }
            }
            catch
            {
                listaCodigos = null;
            }

            //retorna a lista de códigos final
            return listaCodigos;
        }

        //método para buscar todos alunos no banco de dados
        public static List<Aluno> buscarTodos()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            List<Aluno> listaAluno = new List<Aluno>();

            try
            {
                //conexão aberta
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM aluno", con);

                //lista temporária para guardar os códigos dos alunos correspondentes
                List<string> lista = new List<string>();

                //execução do leitor
                using (MySqlDataReader leitor = query.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        //adiciona à lista o código do aluno
                        lista.Add(leitor["cod_aluno"].ToString());
                    }
                }

                con.Close();

                foreach (string cod in lista)
                {
                    //criado um objeto aluno para cada código na lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    listaAluno.Add(aluno);
                }
            }
            catch
            {
                listaAluno = null;
            }

            return listaAluno;
        }

        //método para excluir aluno
        public static bool excluirAluno(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM aluno WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

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

        //método para atualizar dados do aluno
        public bool atualizarAluno(Aluno alunoAntigo, Aluno alunoNovo)
        {
            bool resultado = false;

            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //criação de lista com tuplas para armazenar dado e coluna a ser alterada
            List<Tuple<string, string>> lista = new List<Tuple<string, string>>();

            //se os dados novo e antigo forem diferentes, adiciona o dado novo e o nome da coluna a ser alterada na lista de tuplas
            if (alunoAntigo.Nome != alunoNovo.Nome)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Nome, "nome_aluno"));
            }

            if (alunoAntigo.Bairro != alunoNovo.Bairro)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Bairro, "bairro"));
            }

            if (alunoAntigo.CidadeResidencia != alunoNovo.CidadeResidencia)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.CidadeResidencia, "cidade_res"));
            }

            if (alunoAntigo.Estado != alunoNovo.Estado)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Estado, "estado"));
            }

            if (alunoAntigo.Cep != alunoNovo.Cep)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Cep, "cep"));
            }

            if (alunoAntigo.DataNascimento != alunoNovo.DataNascimento)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.DataNascimento.ToString("yyyy-MM-dd"), "nascimento"));
            }

            if (alunoAntigo.Telefone != alunoNovo.Telefone)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Telefone, "telefone"));
            }

            if (alunoAntigo.PalavrasChave != alunoNovo.PalavrasChave)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.PalavrasChave, "palavras_chave"));
            }

            if (alunoAntigo.Empregavel != alunoNovo.Empregavel)
            {
                lista.Add(new Tuple<string, string>(alunoNovo.Empregavel.ToString(), "empregavel"));
            }

            //para cada dado a ser alterado,
            foreach (var item in lista)
            {
                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza o campo de acordo com o dado informado
                    MySqlCommand query = new MySqlCommand(string.Format("UPDATE aluno SET {0} = @dado WHERE cod_aluno = @codAluno", item.Item2), con);
                    query.Parameters.AddWithValue("@dado", item.Item1);
                    query.Parameters.AddWithValue("@codAluno", alunoAntigo.CodAluno);

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

            return resultado;
        }

        //método para mudar o estado de empregamento do aluno
        public static bool mudarEmpregavel(string codAluno, bool empregavel)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                MySqlCommand query = new MySqlCommand("UPDATE aluno SET empregavel = @empregavel WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@empregavel", empregavel);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                //execução da query
                query.ExecuteNonQuery();
                con.Close();

                if (empregavel == false)
                {
                    //caso 'empregavel' tenha sido mudado para falso, todas as vagas em que aquele aluno se candidatou são removidas
                    Vaga.removerTodasCandidaturas(codAluno);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}