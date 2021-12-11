using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Curriculum.Models
{
    public class Usuario
    {
        //variáveis
        private string nomeUsuario, senha, codAluno, salt;
        private bool admin;

        //propriedades
        public string NomeUsuario { get => nomeUsuario; set => nomeUsuario = value; }
        public string Senha { get => senha; set => senha = value; }
        public string Salt { get => salt; set => salt = value; }
        public string CodAluno { get => codAluno; set => codAluno = value; }
        public bool Admin { get => admin; set => admin = value; }

        //construtores
        public Usuario(string nomeUsuario, string senha, string codAluno, bool admin)
        {
            //atribuição de valores através das propriedades
            NomeUsuario = nomeUsuario;
            Senha = senha;
            CodAluno = codAluno;
            Admin = admin;
        }
        public Usuario(string nomeUsuario, string senha, string salt, string codAluno, bool admin)
        {
            //atribuição de valores através das propriedades
            NomeUsuario = nomeUsuario;
            Senha = senha;
            Salt = salt;
            CodAluno = codAluno;
            Admin = admin;
        }

        //método para cadastrar usuário
        public bool inserirUsuario()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //é gerado um salt para a senha
            string salt = Crypto.GenerateSalt();
            //a senha recebe o salt no início e é hasheada
            senha = salt + senha;
            string senhaHash = Crypto.SHA256(senha);

            try
            {
                //conexão aberta
                con.Open();

                //inserção de dados na tabela de usuário
                MySqlCommand query = new MySqlCommand("INSERT INTO usuario VALUES(@nomeUsuario, @senha, @salt, @codAluno, @admin)", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                query.Parameters.AddWithValue("@senha", senhaHash);
                query.Parameters.AddWithValue("@salt", salt);
                query.Parameters.AddWithValue("@admin", Admin);

                //se houver código de aluno,
                if (codAluno != null)
                {
                    //atribuído o código como parâmetro
                    query.Parameters.AddWithValue("@codAluno", CodAluno);
                }
                else
                {
                    //caso contrário, o código é nulo
                    query.Parameters.AddWithValue("@codAluno", null);
                }

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

        //método que liga o perfil de aluno ao usuário
        public bool ligarAluno(string codAluno)
        {
            //criação da copnexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //o código do aluno é atualizado para o informado
                MySqlCommand query = new MySqlCommand("UPDATE usuario set cod_aluno = @codAluno WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                //execução do código
                query.ExecuteNonQuery();

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
        
        //método para autenticar o usuário
        public static Usuario Autenticar(string nomeUsuario, string senha)
        {
            //cria um objeto 'usuario' nulo e a conexão
            Usuario u = null;
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();
                //seleciona todos os dados do usuário cujo nome for igual ao fornecido ao método
                MySqlCommand query = new MySqlCommand("SELECT * FROM usuario WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@nomeUsuario", nomeUsuario);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //se o leitor tiver dados,
                if (leitor.HasRows)
                {
                    //enquanto o leitor puder ler,
                    while (leitor.Read())
                    {
                        //criam-se variáveis locais para o usuário, senha e salt do banco
                        string usuarioBanco = leitor["nome_usuario"].ToString();
                        string senhaBanco = leitor["senha"].ToString();
                        string salt = leitor["salt"].ToString();

                        //a senha informada pelo usuário recebe um salt no início e é hasheada
                        senha = salt + senha;
                        string senhaHash = Crypto.SHA256(senha);

                        //se o nome de usuário e senha informada for igual aos dados do banco,
                        if (nomeUsuario.Equals(usuarioBanco) && senhaHash == senhaBanco)
                        {
                            //variável para o código do aluno
                            string codAluno = null;

                            //se o usuario não for admin (ou seja, é aluno),
                            if ((bool)leitor["admin"] == false)
                            {
                                //seu código de aluno é obtido
                                codAluno = leitor["cod_aluno"].ToString();
                            }

                            //o usuário é criado com os dados do banco
                            u = new Usuario(
                                leitor["nome_usuario"].ToString(),
                                leitor["senha"].ToString(),
                                leitor["salt"].ToString(),
                                codAluno,
                                (bool)leitor["admin"]
                                );
                        }
                        else
                        {
                            //caso contrário, o usuário é nulo
                            u = null;
                        }
                    }
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //caso dê erro, o usuário é nulo
                u = null;
            }
            //retorna o usuário
            return u;
        }

        //método para atualizar a senha do usuário
        public bool atualizarSenha(string senhaAntiga, string senhaNova, string nomeUsuario)
        {
            //resultado original
            bool resultado = false;

            //a senha nova recebe o salt no início e é hasheada
            senhaNova = Salt + senhaNova;
            string senhaHash = Crypto.SHA256(senhaNova);

            //se as senhas nova e antiga forem diferentes,
            if (senhaAntiga != senhaHash)
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza a senha do usuário de acordo com o nome de usuário
                    MySqlCommand query = new MySqlCommand("UPDATE usuario SET senha = @senha WHERE nome_usuario = @nomeUsuario", con);
                    query.Parameters.AddWithValue("@senha", senhaHash);
                    query.Parameters.AddWithValue("@nomeUsuario", nomeUsuario);
                    //execução do código
                    query.ExecuteNonQuery();
                    //conexão fechada
                    con.Close();

                    //o resultado passa a ser verdadeiro
                    resultado = true;
                }
                catch
                {
                    //em caso de erro, o resultado passa a ser falso
                    resultado = false;
                }
            }
            //retorna o resultado
            return resultado;
        }

        //método para checar se há um usuário com certo nome já cadastrado
        public bool checarNomeUsuario()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //checa se há algum usuário com o nome informado
                MySqlCommand query = new MySqlCommand("SELECT nome_usuario FROM usuario WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //se o leitor possuir dados,
                if (leitor.HasRows)
                {
                    //retorna falso
                    return false;
                }
                else
                {
                    //caso contrário, retorna verdadeiro
                    return true;
                }
            }
            catch
            {
                //em caso de erro, retorna falso
                return false;
            }
        }

        //método para apagar o usuário
        public bool apagarUsuario()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //apaga o usuário de acordo com o nome de usuário
                MySqlCommand query = new MySqlCommand("DELETE FROM usuario WHERE nome_usuario = @nomeUsuario AND senha = @senha", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                query.Parameters.AddWithValue("@senha", Senha);
                //execução do código
                query.ExecuteNonQuery();

                //conexão fechada
                con.Close();

                //caso o usuário possua código de aluno,
                if (CodAluno != null)
                {
                    //são excluídos o currículo, curso feito, vagas candidatadas e os dados do aluno
                    Curriculo.excluirCurriculo(codAluno);
                    Curso.apagarCursoFeito(codAluno);
                    Vaga.removerTodasCandidaturas(codAluno);
                    Aluno.excluirAluno(codAluno);
                }

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //caso haja erro, retorna falso
                return false;
            }
        }
    }
}