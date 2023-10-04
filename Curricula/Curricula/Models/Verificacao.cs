using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Curricula.Models
{
    public static class Verificacao
    {
        //método para validar e formatar uma cadeia de caracteres
        public static string validarTexto(string dado, int tamMin, int tamMax)
        {
            //verifica se o dado possui apenas letras/espaços
            if (Regex.IsMatch(dado, @"^[A-zÀ-ú\s]*$"))
            {
                if (dado.StartsWith(" ") || dado.EndsWith(" "))
                {
                    //remove espaços em excesso
                    dado = dado.Trim();
                }

                //verifica se o dado é vazio
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    dado = null;
                }
            }
            else
            {
                dado = null;
            }

            return dado;
        }

        //método para validar e formatar palavras-chave em um array
        public static string[] validarPalavrasChave(string[] palavrasChave)
        {
            for (int i = 0; i < palavrasChave.Length; i++)
            {
                if (palavrasChave[i].StartsWith(" "))
                {
                    //se a palavra começar com um espaço, o espaço é removido
                    palavrasChave[i] = Regex.Replace(palavrasChave[i], @"\s", "");
                }
                if (palavrasChave[i].Equals(""))
                {
                    //se a palavra for vazia, um novo array é criado sem ela
                    palavrasChave = palavrasChave.Where(o => o != "").ToArray();
                    //i passa a ser 0 para que a verificação seja feita novamente
                    i = 0;
                }
            }

            //retorna o array formatado
            return palavrasChave;
        }

        //método para validar telefone
        public static string validarTelefone(string dado)
        {
            if (dado.Contains("_"))
            {
                //nos campos com máscara, os espaços em brancos possuem underline. Portanto, se houver algum underline, ele é removido
                dado = dado.Replace("_", "");
            }

            //caracteres especiais removidos
            dado = dado.Replace("(", "");
            dado = dado.Replace(")", "");
            dado = dado.Replace("-", "");
            dado = dado.Replace(" ", "");

            if (dado.Length != 11 && dado.Length != 10)
            {
                //se o tamanho do dado não igualar ao tamanho informado, o dado é nulo
                dado = null;
            }

            return dado;
        }

        //método para validar CEP
        public static string validarCep(string dado)
        {
            if (dado.Contains("_"))
            {
                //nos campos com máscara, os espaços em brancos possuem underline. Portanto, se houver algum underline, ele é removido
                dado = dado.Replace("_", "");
            }

            dado = dado.Replace("-", "");

            if (dado.Length != 8)
            {
                //se o tamanho do dado não igualar ao tamanho informado, o dado é nulo
                dado = null;
            }

            return dado;
        }

        //método para validar o nome de usuário
        public static string validarNomeUsuario(string dado)
        {
            if (!Regex.IsMatch(dado, @"^[A-z0-9_.]*$") || dado.Length < 5 || dado.Length > 20)
            {
                //se o dado informado não possuir apenas letras, ponto e/ou underline e estiver entre 5 e 20 caracteres, o dado é nulo
                dado = null;
            }

            return dado;
        }

        //método para validar senhas
        public static string validarSenha(string senha, string confirmacao)
        {
            if (senha == "" || !senha.Equals(confirmacao))
            {
                //se a senha não for vazia ou se não for igual à confirmação de senha, a senha é nula
                senha = null;
            }

            return senha;
        }

        //método para validar abreviações
        public static string validarAbreviacao(string dado, int tamMin, int tamMax)
        {
            dado = dado.ToUpper();

            //verifica se o dado possui apenas letras maiúsculas
            if (Regex.IsMatch(dado, @"^[A-Z]*$"))
            {
                if (dado.Contains(" "))
                {
                    //se o dado possuir espaços, os espaços são removidos
                    dado = dado.Replace(" ", "");
                }

                //verifica se o dado é nulo
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    dado = null;
                }
            }
            else
            {
                dado = null;
            }

            return dado;
        }

        //método para validar um texto com pontuações
        public static string validarTextoCompleto(string dado, int tamMin, int tamMax)
        {
            //verifica se o dado informado possui apenas letras, espaços, números e pontuações
            if (Regex.IsMatch(dado, @"^[A-zÀ-ú0-9,;:.?!@+#\-/()\s]*$"))
            {
                if (dado.StartsWith(" ") || dado.EndsWith(" "))
                {
                    //removidos os espaços do início e fim da palavra
                    dado = dado.Trim();
                }

                //verifica se o dado é nulo
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    dado = null;
                }
            }
            else
            {
                dado = null;
            }

            return dado;
        }

        //método para validar palavras-chave e formatá-las em uma única cadeia de caracteres
        public static string validarPalavrasChave(string dado, int tamMin, int tamMax)
        {
            //dado que será retornado no final
            string dadoFinal = "";
            //criado um array separando as palavras-chave com ponto e vírgula
            string[] palavrasChave = dado.Split(';');

            if (palavrasChave.Count() > 0)
            {
                foreach (string palavra in palavrasChave)
                {
                    if (Regex.IsMatch(palavra, @"^[A-zÀ-ú0-9,;:.?!@+#\-/()\s]*$") && palavra != "")
                    {
                        //se o dado informado possuir apenas letras, espaços, números e pontuações e não for vazio, a palavra é acrescentada no dado final
                        dadoFinal = dadoFinal + palavra.Trim();
                        
                        if (!palavrasChave.Last().Equals(palavra))
                        {
                            //adicionado um ponto e vírgula com espaço no final
                            dadoFinal = dadoFinal + "; ";
                        }           
                        else if (!palavra.EndsWith(";"))
                        {
                            //caso a palavra seja a última e não termine com ponto e vírgula, adiciona um ponto e vírgula no final
                            dadoFinal = dadoFinal + ";";
                        }
                    }
                }

                //verifica se o dado é nulo
                if (dadoFinal.Equals("") || dadoFinal.Length < tamMin || dadoFinal.Length > tamMax)
                {
                    dadoFinal = null;
                }
                else
                {
                    //remoção de espaços no início e fim da palavra
                    dadoFinal = dadoFinal.Trim();
                }
            }
            else
            {
                dadoFinal = null;
            }

            return dadoFinal;
        }
    }
}