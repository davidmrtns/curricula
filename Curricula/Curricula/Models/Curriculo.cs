using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Web;

namespace Curricula.Models
{
    public class Curriculo
    {
        private string codCurriculo, codAluno, nomeArquivo;
        private HttpPostedFileBase arquivo;
        private byte[] conteudo;

        //propriedades
        public string CodCurriculo { get => codCurriculo; set => codCurriculo = value; }
        public string CodAluno { get => codAluno; set => codAluno = value; }
        public string NomeArquivo { get => nomeArquivo; set => nomeArquivo = value; }
        public HttpPostedFileBase Arquivo { get => arquivo; set => arquivo = value; }
        public byte[] Conteudo { get => conteudo; set => conteudo = value; }

        //construtores
        public Curriculo() { }
        public Curriculo(string codAluno, HttpPostedFileBase arquivo)
        {
            CodAluno = codAluno;
            Arquivo = arquivo;
        }
        public Curriculo(string codCurriculo, string codAluno, string nomeArquivo, byte[] conteudo)
        {
            CodCurriculo = codCurriculo;
            CodAluno = codAluno;
            NomeArquivo = nomeArquivo;
            Conteudo = conteudo;
        }

        //método para cadastrar currículos no banco
        public bool inserirCurriculo()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //o conteúdo do arquivo enviado é transformado em um array de bytes
            byte[] conteudo = null;
            using (var binaryReader = new BinaryReader(Arquivo.InputStream))
            {
                conteudo = binaryReader.ReadBytes(Arquivo.ContentLength);
            }

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("INSERT INTO curriculo VALUES (@codCurriculo, @codAluno, @nomeArquivo, @extensao, @conteudo)", con);
                query.Parameters.AddWithValue("@codCurriculo", CodCurriculo);
                query.Parameters.AddWithValue("@codAluno", CodAluno);
                query.Parameters.AddWithValue("@nomeArquivo", Arquivo.FileName);
                query.Parameters.AddWithValue("@extensao", Arquivo.ContentType);
                query.Parameters.AddWithValue("@conteudo", conteudo);

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

        //método para atualizar o currículo
        public bool atualizarCurriculo(Curriculo curriculoAntigo, Curriculo curriculoNovo)
        {
            bool resultado = false;

            //o conteúdo do arquivo novo é transformado em um array de bytes
            byte[] conteudo = null;
            using (var binaryReader = new BinaryReader(curriculoNovo.Arquivo.InputStream))
            {
                conteudo = binaryReader.ReadBytes(curriculoNovo.Arquivo.ContentLength);
            }

            //criação de objetos binários com o array de conteúdos
            Binary b1 = new Binary(conteudo);
            Binary b2 = new Binary(curriculoAntigo.Conteudo);

            //atualiza apenas se os conteúdos forem diferentes
            if (!b1.Equals(b2))
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    con.Open();

                    MySqlCommand query = new MySqlCommand("UPDATE curriculo SET nome_arquivo = @nomeArquivo, " +
                    "extensao_arquivo = @extensao, conteudo_arquivo = @conteudo WHERE cod_aluno = @codAluno", con);
                    query.Parameters.AddWithValue("@codAluno", curriculoAntigo.CodAluno);
                    query.Parameters.AddWithValue("@nomeArquivo", curriculoNovo.Arquivo.FileName);
                    query.Parameters.AddWithValue("@extensao", curriculoNovo.Arquivo.ContentType);
                    query.Parameters.AddWithValue("@conteudo", conteudo);

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

        //método para excluir um currículo cadastrado (usado quando um aluno é apagado)
        public static bool excluirCurriculo(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                con.Open();

                MySqlCommand query = new MySqlCommand("DELETE FROM curriculo WHERE cod_aluno = @codAluno", con);
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

        //método para buscar o currículo cadastrado
        public Curriculo buscarCurriculo(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            Curriculo curriculo = null;
            
            try
            {
                //conexão aberta
                con.Open();

                MySqlCommand query = new MySqlCommand("SELECT * FROM curriculo WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("codAluno", codAluno);

                //execução da query
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read())
                {
                    CodAluno = codAluno;
                    CodCurriculo = leitor["cod_curriculo"].ToString();
                    Conteudo = (byte[]) leitor["conteudo_arquivo"];
                    NomeArquivo = leitor["nome_arquivo"].ToString();

                    //criação do objeto curriculo com os dados do banco
                    curriculo = new Curriculo(CodCurriculo, CodAluno, NomeArquivo, Conteudo);
                }

                con.Close();
            }
            catch
            {
                curriculo = null;
            }

            return curriculo;
        }
    }
}