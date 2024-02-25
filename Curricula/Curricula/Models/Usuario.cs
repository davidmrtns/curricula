using MySql.Data.MySqlClient;
using System;
using System.Web.Helpers;
using Curricula.Classes;

namespace Curricula.Models
{
    public class Usuario
    {
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
            NomeUsuario = nomeUsuario;
            Senha = senha;
            CodAluno = codAluno;
            Admin = admin;
        }
        public Usuario(string nomeUsuario, string senha, string salt, string codAluno, bool admin)
        {
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

            //geração de salt para a senha
            string salt = Crypto.GenerateSalt();
            //a senha recebe o salt no início e é hasheada
            senha = salt + senha;
            string senhaHash = Crypto.SHA256(senha);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO usuario VALUES(@nomeUsuario, @senha, @salt, @codAluno, @admin)", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                query.Parameters.AddWithValue("@senha", senhaHash);
                query.Parameters.AddWithValue("@salt", salt);
                query.Parameters.AddWithValue("@admin", Admin);

                if (codAluno != null)
                {
                    //se houver código de aluno, é atribuído o código como parâmetro
                    query.Parameters.AddWithValue("@codAluno", CodAluno);
                }
                else
                {
                    query.Parameters.AddWithValue("@codAluno", null);
                }

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //método para ligar o aluno ao usuário
        public bool ligarAluno(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("UPDATE usuario set cod_aluno = @codAluno WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);

                query.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        //método para autenticar o usuário
        public static Usuario Autenticar(string nomeUsuario, string senha)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            Usuario u = null;

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM usuario WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@nomeUsuario", nomeUsuario);

                MySqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows)
                {
                    while (leitor.Read())
                    {
                        string usuarioBanco = leitor["nome_usuario"].ToString();
                        string senhaBanco = leitor["senha"].ToString();
                        string salt = leitor["salt"].ToString();

                        //a senha informada pelo usuário recebe um salt no início e é hasheada
                        senha = salt + senha;
                        string senhaHash = Crypto.SHA256(senha);

                        //verifica se o nome de usuário e senha informada for igual aos dados do banco
                        if (nomeUsuario.Equals(usuarioBanco) && senhaHash == senhaBanco)
                        {
                            string codAluno = null;

                            if ((bool)leitor["admin"] == false)
                            {
                                //se o usuario não for admin (ou seja, é aluno), seu código de aluno é obtido
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
                            u = null;
                        }
                    }
                }
                con.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                u = null;
            }

            return u;
        }

        //método para atualizar a senha do usuário
        public bool atualizarSenha(string senhaAntiga, string senhaNova, string nomeUsuario)
        {
            bool resultado = false;

            //a senha nova recebe o mesmo salt no início e é hasheada
            senhaNova = Salt + senhaNova;
            string senhaHash = Crypto.SHA256(senhaNova);

            if (senhaAntiga != senhaHash)
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    con.Open();

                    MySqlCommand query = new MySqlCommand("UPDATE usuario SET senha = @senha WHERE nome_usuario = @nomeUsuario", con);
                    query.Parameters.AddWithValue("@senha", senhaHash);
                    query.Parameters.AddWithValue("@nomeUsuario", nomeUsuario);

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

        //método para checar se há um usuário com certo nome de usuário já cadastrado
        public bool checarNomeUsuario()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT nome_usuario FROM usuario WHERE nome_usuario = @nomeUsuario", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);

                MySqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
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
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM usuario WHERE nome_usuario = @nomeUsuario AND senha = @senha", con);
                query.Parameters.AddWithValue("@nomeUsuario", NomeUsuario);
                query.Parameters.AddWithValue("@senha", Senha);

                query.ExecuteNonQuery();
                con.Close();
                
                if (CodAluno != null)
                {
                    //caso o usuário possua código de aluno, são excluídos o currículo, curso feito, vagas candidatadas e os dados do aluno
                    Curriculo.excluirCurriculo(codAluno);
                    Curso.apagarCursoFeito(codAluno);
                    Vaga.removerTodasCandidaturas(codAluno);
                    Aluno.excluirAluno(codAluno);
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