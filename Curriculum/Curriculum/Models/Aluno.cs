using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Curriculum.Models
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
                //conexão aberta
                con.Open();
                //inserindo dados através de parâmetros
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

                //executando comando
                query.ExecuteNonQuery();
                //recupera o código do aluno recém-cadastrado
                codAlunoTemp = query.LastInsertedId;
                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //caso dê erro, retorna falso
                return false;
            }
        }

        //método para buscar dados do aluno no banco de dados passando o código do aluno como parâmetro
        public Aluno buscarDados(string codAluno)
        {
            //criação do objeto aluno como nulo
            Aluno aluno = null;
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();
                //seleciona tudo da tabela 'aluno' onde o a chave primária for igual à informada
                MySqlCommand query = new MySqlCommand("SELECT * FROM aluno WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);

                //criação do leitor de dados
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //atribuição de valores às prorpiedades de Aluno
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

                    //o objeto aluno deixa de ser nulo e passa a ter dados através do construtor
                    aluno = new Aluno(CodAluno, Nome, Bairro, CidadeResidencia, Estado, Cep, DataNascimento, Telefone, PalavrasChave, Empregavel);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //caso dê erro, o objeto aluno é nulo
                aluno = null;
            }
            //retorna o objeto aluno
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
                //conexão aberta
                con.Open();
                MySqlCommand query;

                //caso a tabela seja a de cursos feitos,
                if (tabela == "cursofeito")
                {
                    //seleciona o código do aluno de acordo com o curso e estado da coluna empregavel
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
                    //enquanto o leitor puder ler,
                    while (leitor.Read())
                    {
                        //adiciona à lista o código do aluno
                        listaCodigos.Add(leitor["cod_aluno"].ToString());
                    }
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, a lista é nula
                listaCodigos = null;
            }
            //a lista com os alunos é retornada
            return listaCodigos;
        }

        //método para buscar alunos com palavras-chave correspondentes
        public static List<string> buscarPalavrasChave(string palavrasChave)
        {
            //criação da lista com os alunos correspondentes e lista de listas
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

                //para cada palavra-chave,
                foreach (string palavraChave in arrayPalavras)
                {
                    //se a palavra-chave não for vazia,
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
                            //enquanto o leitor puder ler,
                            while (leitor.Read())
                            {
                                //adicionado à lista temporária o código do aluno
                                listaTemp.Add(leitor["cod_aluno"].ToString());
                            }
                        }
                        //por fim, adiciona a lista temporária à lista de listas
                        listaListas.Add(listaTemp);
                    }
                }
                //conexão fechada
                con.Close();

                //se houverem listas na lista de listas,
                if (listaListas.Count >= 1)
                {
                    for (int ii = 0; ii < listaListas.Count() || ii > listaListas.Count() || ii == listaListas.Count(); ii++)
                    {
                        //o index é a quantidade de listas na lista
                        int index = listaListas.Count();
                        //se o index for diferente de 1,
                        if (index != 1)
                        {
                            //as duas primeiras listas se interceptam e geram uma terceira lista onde os dados
                            //se igualam, a qual é adicionada à lista de listas
                            listaListas.Add(listaListas[0].Intersect(listaListas[1]).ToList());
                            //remoção das duas primeiras listas
                            listaListas.Remove(listaListas[0]);
                            listaListas.Remove(listaListas[0]);
                        }
                        else
                        {
                            //caso contrário, a lista de códigos é igual à primeira lista da lista de listas
                            listaCodigos = listaListas[0];
                            break;
                        }
                    }
                }
            }
            catch
            {
                //em caso de erro, a lista de códigos é nula
                listaCodigos = null;
            }
            //retorna a lista de códigos
            return listaCodigos;
        }

        //método para buscar todos alunos no banco de dados
        public static List<Aluno> buscarTodos()
        {
            //criação da lista com os alunos correspondentes
            List<Aluno> listaAluno = new List<Aluno>();
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();
                //seleciona o código de todos alunos do banco de dados
                MySqlCommand query = new MySqlCommand("SELECT cod_aluno FROM aluno", con);

                //lista temporária para guardar os códigos dos alunos correspondentes
                List<string> lista = new List<string>();

                //execução do leitor
                using (MySqlDataReader leitor = query.ExecuteReader())
                {
                    //enquanto o leitor puder ler,
                    while (leitor.Read())
                    {
                        //adiciona à lista o código do aluno
                        lista.Add(leitor["cod_aluno"].ToString());
                    }
                }
                //conexão fechada
                con.Close();

                //para cada código na lista,
                foreach (string cod in lista)
                {
                    //é criado um objeto aluno e passado para a lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    listaAluno.Add(aluno);
                }
            }
            catch
            {
                //em caso de erro, a lista é nula
                listaAluno = null;
            }
            //a lista com os alunos é retornada
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

                //apaga da tabela 'aluno' onde a chave primária se igualar ao código informado
                MySqlCommand query = new MySqlCommand("DELETE FROM aluno WHERE cod_aluno = @codAluno", con);
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

        //método para atualizar dados do aluno
        public bool atualizarAluno(Aluno alunoAntigo, Aluno alunoNovo)
        {
            //resultado original
            bool resultado = false;

            //criação de lista com tuplas para armazenar dado e coluna a ser alterada e conexão
            List<Tuple<string, string>> lista = new List<Tuple<string, string>>();
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //se os nomes novo e antigo forem diferentes,
            if (alunoAntigo.Nome != alunoNovo.Nome)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.Nome, "nome_aluno"));
            }

            //se os bairros novo e antigo forem diferentes,
            if (alunoAntigo.Bairro != alunoNovo.Bairro)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.Bairro, "bairro"));
            }

            //se as cidades nova e antiga forem diferentes,
            if (alunoAntigo.CidadeResidencia != alunoNovo.CidadeResidencia)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.CidadeResidencia, "cidade_res"));
            }

            //se os estados novo e antigo forem diferentes,
            if (alunoAntigo.Estado != alunoNovo.Estado)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.Estado, "estado"));
            }

            //se os ceps novo e antigo forem diferentes,
            if (alunoAntigo.Cep != alunoNovo.Cep)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.Cep, "cep"));
            }

            //se as datas de nascimento nova e antiga forem diferentes,
            if (alunoAntigo.DataNascimento != alunoNovo.DataNascimento)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.DataNascimento.ToString("yyyy-MM-dd"), "nascimento"));
            }

            //se os telefones novo e antigo forem diferentes,
            if (alunoAntigo.Telefone != alunoNovo.Telefone)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.Telefone, "telefone"));
            }

            //se as palavras-chave antigas e novas forem diferentes,
            if (alunoAntigo.PalavrasChave != alunoNovo.PalavrasChave)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
                lista.Add(new Tuple<string, string>(alunoNovo.PalavrasChave, "palavras_chave"));
            }

            //se os estados de empregavel novo e antigo forem diferentes,
            if (alunoAntigo.Empregavel != alunoNovo.Empregavel)
            {
                //adicionado o dado novo e o nome da coluna a se alterar
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
                    //os parâmetros são obtidos das tuplas da lista
                    query.Parameters.AddWithValue("@dado", item.Item1);
                    query.Parameters.AddWithValue("@codAluno", alunoAntigo.CodAluno);
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

        //método para mudar o estado de empregamento do aluno
        public static bool mudarEmpregavel(string codAluno, bool empregavel)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //atualiza a tabela, definindo a coluna 'empregavel' de acordo com o parâmetro informado
                MySqlCommand query = new MySqlCommand("UPDATE aluno SET empregavel = @empregavel WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@empregavel", empregavel);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                //execução do código
                query.ExecuteNonQuery();
                //conexão fechada
                con.Close();

                //caso 'empregavel' tenha sido mudado para falso,
                if (empregavel == false)
                {
                    //todas as vagas em que aquele aluno se candidatou são removidas
                    Vaga.removerTodasCandidaturas(codAluno);
                }

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //caso dê erro, retorna falso
                return false;
            }
        }
    }
}