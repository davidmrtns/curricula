using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Curriculum.Models
{
    public static class Verificacao
    {
        //método para validar e formatar uma cadeia de caracteres
        public static string validarTexto(string dado, int tamMin, int tamMax)
        {
            //se o dado informado possuir apenas letras/espaços,
            if (Regex.IsMatch(dado, @"^[A-zÀ-ú\s]*$"))
            {
                //se o dado começar ou terminar com espaço,
                if (dado.StartsWith(" ") || dado.EndsWith(" "))
                {
                    //são removidos os espaços do início e fim da palavra
                    dado = dado.Trim();
                }

                //se o dado for igual a nada,
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    //o dado é nulo
                    dado = null;
                }
            }
            //caso contrário,
            else
            {
                //o dado é nulo
                dado = null;
            }
            //o dado é retornado
            return dado;
        }

        //método para validar e formatar um array de string
        public static string[] validarPalavrasChave(string[] palavrasChave)
        {
            //enquanto houverem palavras dentro do array,
            for (int i = 0; i < palavrasChave.Length; i++)
            {
                //se a palavra começar com um espaço,
                if (palavrasChave[i].StartsWith(" "))
                {
                    //o espaço é removido
                    palavrasChave[i] = Regex.Replace(palavrasChave[i], @"\s", "");
                }
                //se a palavra for igual a nada (aspas vazias),
                if (palavrasChave[i].Equals(""))
                {
                    //um novo array é criado sem a palavra vazia
                    palavrasChave = palavrasChave.Where(o => o != "").ToArray();
                    //i passa a ser 0 para que a verificação seja feita novamente
                    i = 0;
                }
            }
            //retorna o array com as palavras-chave
            return palavrasChave;
        }

        //método para validar campos com máscara
        public static string validarMascara(string dado, int tamanho)
        {
            //nos campos com máscara, os espaços em brancos possuem underline. Portanto, se houver algum underline,
            if (dado.Contains("_"))
            {
                //ele é removido
                dado = dado.Replace("_", "");
            }

            //se o tamanho do dado não igualar ao tamanho informado,
            if (dado.Length != tamanho)
            {
                //o dado é nulo
                dado = null;
            }
            //retorna o dado
            return dado;
        }

        //método para validar o nome de usuário
        public static string validarNomeUsuario(string dado)
        {
            //se o dado informado não possuir apenas letras, ponto e/ou underline e estiver entre 5 e 20 caracteres,
            if (!Regex.IsMatch(dado, @"^[A-z0-9_.]*$") || dado.Length < 5 || dado.Length > 20)
            {
                //o dado é nulo
                dado = null;
            }
            //o dado é retornado
            return dado;
        }

        //método para validar senhas
        public static string validarSenha(string senha, string confirmacao)
        {
            //se a senha não for vazia ou se não for igual à confirmação de senha,
            if (senha == "" || !senha.Equals(confirmacao))
            {
                //a senha é nula
                senha = null;
            }
            //retorna a senha
            return senha;
        }

        //método para validar abreviações
        public static string validarAbreviacao(string dado, int tamMin, int tamMax)
        {
            //deixa o dado com letras maiúsculas
            dado = dado.ToUpper();

            //se o dado informado possuir apenas letras maiúsculas,
            if (Regex.IsMatch(dado, @"^[A-Z]*$"))
            {
                //se o dado possuir espaços,
                if (dado.Contains(" "))
                {
                    //os espaços são removidos
                    dado = dado.Replace(" ", "");
                }

                //se o dado for igual a nada,
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    //o dado é nulo
                    dado = null;
                }
            }
            //caso contrário,
            else
            {
                //o dado é nulo
                dado = null;
            }
            //o dado é retornado
            return dado;
        }

        //método para validar um texto com pontuações
        public static string validarTextoCompleto(string dado, int tamMin, int tamMax)
        {
            //se o dado informado possuir apenas letras, espaços, números e pontuações,
            if (Regex.IsMatch(dado, @"^[A-zÀ-ú0-9,;:.?!@+#\-/()\s]*$"))
            {
                //se o dado começar ou terminar com espaço,
                if (dado.StartsWith(" ") || dado.EndsWith(" "))
                {
                    //são removidos os espaços do início e fim da palavra
                    dado = dado.Trim();
                }

                //se o dado for igual a nada,
                if (dado.Equals("") || dado.Length < tamMin || dado.Length > tamMax)
                {
                    //o dado é nulo
                    dado = null;
                }
            }
            //caso contrário,
            else
            {
                //o dado é nulo
                dado = null;
            }
            //o dado é retornado
            return dado;
        }

        //método para validar palavras-chave e formatá-las em uma única cadeia de caracteres
        public static string validarPalavrasChave(string dado, int tamMin, int tamMax)
        {
            //dado que será retornado no final
            string dadoFinal = "";
            //criado um array separando as palavras-chave com ponto e vírgula
            string[] palavrasChave = dado.Split(';');

            //se houverem palavras-chave,
            if (palavrasChave.Count() > 0)
            {
                //para cada palavra no array de palavras-chave,
                foreach (string palavra in palavrasChave)
                {
                    //se o dado informado possuir apenas letras, espaços, números e pontuações e não for vazio
                    if (Regex.IsMatch(palavra, @"^[A-zÀ-ú0-9,;:.?!@+#\-/()\s]*$") && palavra != "")
                    {
                        //a palavra é acrescentada no dado final
                        dadoFinal = dadoFinal + palavra.Trim();

                        //se essa palavra não for a última no array,
                        if (!palavrasChave.Last().Equals(palavra))
                        {
                            //adicionado um ponto e vírgula com espaço no final
                            dadoFinal = dadoFinal + "; ";
                        }
                        //caso a palavra seja a última e não termine com ponto e vírgula,
                        else if (!palavra.EndsWith(";"))
                        {
                            //adiciona um ponto e vírgula no final
                            dadoFinal = dadoFinal + ";";
                        }
                    }
                }

                //se o dado for igual a nada ou se for menor ou maior que os tamanhos mínimo e máximo,
                if (dadoFinal.Equals("") || dadoFinal.Length < tamMin || dadoFinal.Length > tamMax)
                {
                    //o dado final é nulo
                    dadoFinal = null;
                }
                else
                {
                    //caso contrário, os espaços do início e fim são removidos
                    dadoFinal = dadoFinal.Trim();
                }
            }
            else
            {
                //caso contrário, o dado final é nulo
                dadoFinal = null;
            }
            //retorna o dado final
            return dadoFinal;
        }
    }
}