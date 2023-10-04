using Curricula.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Curricula.Controllers
{
    public class HomeController : Controller
    {
        //retorna a view 'Index'
        public ActionResult Index()
        {
            return View();
        }

        //retorna a view 'CadastrarAluno'
        public ActionResult CadastrarAluno()
        {
            //lista os cursos disponíveis para cadastro
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }

        //retorna a view 'Conta'
        public ActionResult Conta()
        {
            //busca o usuário ativo na seção
            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario != null && usuario.CodAluno != null)
            {
                //obtém dados do aluno
                Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                Curso curso = Curso.buscarCursoFeito(usuario.CodAluno);
                Curriculo curriculo = new Curriculo().buscarCurriculo(usuario.CodAluno);
                List<Vaga> lista = Vaga.listarVagasCandidatadas(usuario.CodAluno);

                TempData["vagasCandidatadas"] = lista;
                TempData["aluno"] = aluno;
                TempData["cursoFeito"] = curso;
                TempData["curriculo"] = curriculo;

                if (aluno.Empregavel)
                {
                    //caso o aluno seja empregável, obtém uma lista de todas as vagas relacionadas ao curso feito pelo aluno e as vagas às quais ele se candidatou
                    List<Vaga> listaVagas = Vaga.buscarVagasCurso(curso.AbrevCurso);
                    List<Vaga> vagasCandidatadas = Vaga.listarVagasCandidatadas(usuario.CodAluno);

                    TempData["listaVagas"] = listaVagas;
                    TempData["vagasCandidatadas"] = vagasCandidatadas;
                }
            }
            return View();
        }

        //retorna a view 'EditarDados'
        public ActionResult EditarDados()
        {
            //busca o usuário da seção
            Usuario u = (Usuario)Session["Usuario"];

            if (u != null && u.CodAluno != null)
            {
                //obtém dados do aluno
                Aluno aluno = new Aluno().buscarDados(u.CodAluno);
                Curso curso = Curso.buscarCursoFeito(u.CodAluno);
                Curriculo curriculo = new Curriculo().buscarCurriculo(u.CodAluno);
                List<Curso> lista = Curso.listarCursos();

                TempData["aluno"] = aluno;
                TempData["cursoFeito"] = curso;
                TempData["curriculo"] = curriculo;
                TempData["cursos"] = lista;
            }
            return View();
        }

        //retorna a view 'CandidatarVagas'
        public ActionResult CandidatarVagas()
        {
            //obtém o usuário ativo na seção
            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario != null && usuario.CodAluno != null)
            {
                //obtém dados do aluno e curso feito
                Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                Curso curso = Curso.buscarCursoFeito(usuario.CodAluno);
                TempData["aluno"] = aluno;

                if (aluno.Empregavel)
                {
                    //se for empregável, obtém uma lista de todas as vagas relacionadas ao curso feito pelo aluno e as vagas às quais ele se candidatou
                    List<Vaga> lista = Vaga.buscarVagasCurso(curso.AbrevCurso);
                    List<Vaga> vagasCandidatadas = Vaga.listarVagasCandidatadas(usuario.CodAluno);

                    TempData["listaVagas"] = lista;
                    TempData["vagasCandidatadas"] = vagasCandidatadas;
                }
            } 
            return View();
        }

        //retorna a view 'PesquisarAlunos'
        public ActionResult PesquisarAlunos()
        {
            //puxa uma lista com todos os cursos cadastrados
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }

        //retorna a view 'CadastrarCurso'
        public ActionResult CadastrarCurso()
        {
            return View();
        }

        //retorna a view 'DadosAluno'
        public ActionResult DadosAluno()
        {
            return View();
        }

        //retorna a view 'CadastrarVaga'
        public ActionResult CadastrarVaga()
        {
            //obtém uma lista com todos os cursos disponíveis para cadastro da vaga
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }

        //retorna a view 'DadosVaga'
        public ActionResult DadosVaga()
        {
            return View();
        }

        //retorna a view 'EditarVaga'
        public ActionResult EditarVaga(string id)
        {
            //obtém lista de cursos cadastrados
            List<Curso> lista = Curso.listarCursos();
            //busca a vaga que será editada
            Vaga vaga = new Vaga().buscarDados(id);

            TempData["vaga"] = vaga;
            TempData["cursos"] = lista;
            return View();
        }

        //método HttpPost para cadastro de aluno
        [HttpPost]
        public ActionResult CadastrarAluno(string nome, string sobrenome, string bairro, string cidade,
            string cursoRealizado, string dataNascimento, string telefone, string cep, string estado, HttpPostedFileBase arquivo,
            string palavrasChave, string nomeUsuario, string senha, string confirmacao)
        {
            string msg = null;

            //validação de todos os dados recebidos
            nome = Verificacao.validarTexto(nome, 1, 20);
            sobrenome = Verificacao.validarTexto(sobrenome, 1, 20);
            telefone = Verificacao.validarTelefone(telefone);
            bairro = Verificacao.validarTexto(bairro, 1, 30);
            cidade = Verificacao.validarTexto(cidade, 1, 30);
            estado = Verificacao.validarAbreviacao(estado, 2, 2);
            cep = Verificacao.validarCep(cep);
            palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);
            cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);
            nomeUsuario = Verificacao.validarNomeUsuario(nomeUsuario);
            senha = Verificacao.validarSenha(senha, confirmacao);

            //verifica se os dados são válidos e se o arquivo do currículo é um formato válido
            if (nome != null && sobrenome != null && telefone != null && bairro != null && cidade != null
                && estado != null && cep != null && cursoRealizado != null && arquivo != null && arquivo.ContentLength > 0
                && (arquivo.FileName.EndsWith(".pdf") || arquivo.FileName.EndsWith(".docx") && nomeUsuario != null && senha != null))
            {
                string nomeCompleto = nome + " " + sobrenome;

                Aluno aluno = new Aluno(nomeCompleto, bairro, cidade, estado, cep, DateTime.Parse(dataNascimento), telefone, palavrasChave, true);
                Usuario usuario = new Usuario(nomeUsuario, senha, null, false);

                //checa se o nome de usuário está disponível
                if (usuario.checarNomeUsuario())
                {
                    //faz o cadastro do usuário
                    if (usuario.inserirUsuario())
                    {
                        //faz o cadastro do aluno
                        if (aluno.inserirAluno())
                        {
                            string codAluno = aluno.codAlunoTemp.ToString();
                            Curriculo curriculo = new Curriculo(codAluno, arquivo);

                            //faz o cadstro do curso feito e do currículo
                            if (Curso.inserirCursoFeito(codAluno, cursoRealizado) && curriculo.inserirCurriculo())
                            {
                                //conecta o aluno ao usuário criado
                                if (usuario.ligarAluno(codAluno))
                                {
                                    msg = "Cadastrado com sucesso";
                                }
                                else
                                {
                                    msg = "Um erro ocorreu ao cadastrar. Tente novamente";
                                }
                            }
                            else
                            {
                                //em caso de erro, o usuário é excluído
                                usuario.apagarUsuario();
                            }
                        }
                        else
                        {
                            //em caso de erro, o usuário é excluído
                            usuario.apagarUsuario();
                        }
                    }
                    else
                    {
                        msg = "Um erro ocorreu ao cadastrar. Tente novamente";
                    }
                }
                else
                {
                    //informa se o nome de usuário já foi usado
                    msg = string.Format("O nome de usuário inserido ({0}) já está sendo utilizado", nomeUsuario);
                }
            }
            else
            {
                msg = "Preencha os campos corretamente";
            }

            TempData["msg"] = msg;
            return RedirectToAction("CadastrarAluno");
        }

        //método HttpPost para editar dados do usuário
        [HttpPost]
        public ActionResult EditarDados(string nomeCompleto, string bairro, string cidade,
            string cursoRealizado, string dataNascimento, string telefone, string cep, string estado, HttpPostedFileBase arquivo,
            string palavrasChave, string nomeUsuario, string senha, string confirmacao)
        {
            //mensagem padrão para casos de erro
            string msg = "Nenhum dado foi atualizado. Tente novamente";
            //obtém o usuário da sessão
            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario != null)
            {
                //valida a senha nova informada para alteração
                senha = Verificacao.validarSenha(senha, confirmacao);

                //se for aluno,
                if (usuario.CodAluno != null)
                {
                    //obtém dados
                    Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                    Curso cursoFeito = Curso.buscarCursoFeito(usuario.CodAluno);
                    Curriculo curriculo = new Curriculo().buscarCurriculo(usuario.CodAluno);

                    //validação dos campos para atualização
                    nomeCompleto = Verificacao.validarTexto(nomeCompleto, 1, 40);
                    telefone = Verificacao.validarTelefone(telefone);
                    bairro = Verificacao.validarTexto(bairro, 1, 30);
                    cidade = Verificacao.validarTexto(cidade, 1, 30);
                    estado = Verificacao.validarAbreviacao(estado, 2, 2);
                    cep = Verificacao.validarCep(cep);
                    cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);
                    palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);

                    if (nomeCompleto != null && telefone != null && bairro != null && cidade != null
                        && estado != null && cep != null)
                    {
                        //cria um aluno com os dados novos
                        Aluno alunoNovo = new Aluno(aluno.CodAluno, nomeCompleto, bairro, cidade, estado, cep, DateTime.Parse(dataNascimento), telefone, palavrasChave, aluno.Empregavel);

                        //atualização dos dados
                        if(alunoNovo.atualizarAluno(aluno, alunoNovo))
                        {
                            msg = "Dados atualizados";
                        }
                    }

                    //atualização do curso feito
                    if (cursoRealizado != null)
                    {
                        if (cursoFeito == null)
                        {
                            //inserção do curso feito, caso não tenha um anterior
                            if (Curso.inserirCursoFeito(aluno.CodAluno, cursoRealizado))
                            {
                                msg = "Dados atualizados";
                            }
                        }
                        else
                        {
                            //atualização do curso feito já existente
                            if (Curso.atualizarCursoFeito(cursoFeito.AbrevCurso, cursoRealizado, aluno.CodAluno))
                            {
                                msg = "Dados atualizados";
                            }
                        }
                    }

                    //checa se o currículo é válido
                    if (arquivo != null && arquivo.ContentLength > 0 && (arquivo.FileName.EndsWith(".pdf") || arquivo.FileName.EndsWith(".docx")))
                    {
                        Curriculo curriculoNovo = new Curriculo(aluno.CodAluno, arquivo);

                        if (curriculo == null)
                        {
                            //inserção do currículo, caso não tenha um anterior
                            if (curriculoNovo.inserirCurriculo())
                            {
                                //altera o estado do aluno para empregável
                                Aluno.mudarEmpregavel(aluno.CodAluno, true);
                                msg = "Dados atualizados";
                            }
                            else
                            {
                                //em caso de erro, o estado do aluno é não empregável
                                Aluno.mudarEmpregavel(aluno.CodAluno, false);
                            }
                        }
                        else
                        {
                            //atualização do currículo anterior
                            curriculoNovo.atualizarCurriculo(curriculo, curriculoNovo);
                            msg = "Dados atualizados";
                        }
                    }
                }

                if (senha != null)
                {
                    if (usuario.atualizarSenha(usuario.Senha, senha, usuario.NomeUsuario))
                    {
                        //desconecta o usuário após atualizar a senha
                        return Desconectar();
                    } 
                }
            }

            TempData["msg"] = msg;
            return RedirectToAction("Conta");
        }     

        //método HttpPost de autenticação do usuário
        [HttpPost]
        public ActionResult Conta(string nomeUsuario, string senha)
        {
            //autentição do usuário
            Usuario u = Usuario.Autenticar(nomeUsuario, senha);
            string msg = null;

            if (u == null)
            {
                msg = "Usuário ou senha incorretos";
            }
            //caso a autenticação funcione,
            else if (u != null)
            {
                //adiciona o usuário a uma sessão
                Session.Add("Usuario", u);
                msg = "Bem-vindo(a), " + u.NomeUsuario;
            }

            TempData["msg"] = msg;
            return RedirectToAction("Conta");
        }

        //método HttpPost para cadastrar curso
        [HttpPost]
        public ActionResult CadastrarCurso(string nomeCurso, string abrevCurso)
        {
            string msg = null;

            //validação do nome e abreviação do curso
            nomeCurso = Verificacao.validarTexto(nomeCurso, 1, 34);
            abrevCurso = Verificacao.validarAbreviacao(abrevCurso, 2, 5);

            if (nomeCurso != null && abrevCurso != null)
            {
                Curso curso = new Curso(nomeCurso, abrevCurso);

                //inserção do curso
                if (curso.inserirCurso())
                {
                    msg = "Curso cadastrado com sucesso";
                }
                else
                {
                    msg = "Não foi possível cadastrar o curso. Tente novamente";
                }
            }

            TempData["msg"] = msg;
            return RedirectToAction("CadastrarCurso");
        }

        //método HttpPost para cadastrar vaga
        [HttpPost]
        public ActionResult CadastrarVaga(string nomeEmpresa, string nomeVaga, string cidade, string estado, string descricao, string cursoRelacionado)
        {
            string msg = null;

            //validação dos dados da vaga
            nomeEmpresa = Verificacao.validarTextoCompleto(nomeEmpresa, 1, 20);
            nomeVaga = Verificacao.validarTexto(nomeVaga, 1, 30);
            cidade = Verificacao.validarTexto(cidade, 1, 30);
            estado = Verificacao.validarTexto(estado, 2, 2);
            descricao = Verificacao.validarTextoCompleto(descricao, 1, 280);
            cursoRelacionado = Verificacao.validarAbreviacao(cursoRelacionado, 2, 5);

            //se os dados forem válidos,
            if (nomeEmpresa != null && nomeVaga != null && cidade != null && estado != null && descricao != null && cursoRelacionado != null)
            {
                Vaga vaga = new Vaga(nomeEmpresa, nomeVaga, cidade, estado, descricao, cursoRelacionado);

                //cadastro da vaga
                if (vaga.inserirVaga())
                {
                    msg = "Vaga cadastrada com sucesso";
                }
                else
                {
                    msg = "Erro ao cadastrar a vaga. Tente novamente";
                }
            }

            TempData["msg"] = msg;
            return RedirectToAction("CadastrarVaga");
        }

        //método HttpPost para editar vaga
        [HttpPost]
        public ActionResult EditarVaga(string codigoVaga, string nomeEmpresa, string nomeVaga, string cidade, string estado,
            string descricao, string cursoRelacionado)
        {
            //mensagem padrão para casos de erro
            string msg = "Nenhum dado foi atualizado. Tente novamente";
            //busca a vaga para edição
            Vaga vaga = new Vaga().buscarDados(codigoVaga);

            if (vaga != null)
            {
                //validação dos dados novos
                nomeEmpresa = Verificacao.validarTextoCompleto(nomeEmpresa, 1, 20);
                nomeVaga = Verificacao.validarTexto(nomeVaga, 1, 30);
                cidade = Verificacao.validarTexto(cidade, 1, 30);
                estado = Verificacao.validarTexto(estado, 2, 2);
                descricao = Verificacao.validarTextoCompleto(descricao, 1, 280);
                cursoRelacionado = Verificacao.validarAbreviacao(cursoRelacionado, 2, 5);

                //se os dados não forem nulos,
                if (nomeEmpresa != null && nomeVaga != null && cidade != null && estado != null && descricao != null && cursoRelacionado != null)
                {
                    Vaga vagaNova = new Vaga(vaga.CodVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, cursoRelacionado);

                    //atualização da vaga
                    if (vagaNova.atualizarVaga(vaga, vagaNova))
                    {
                        msg = "Vaga atualizada";
                    }
                }
            }

            TempData["msg"] = msg;
            return VerVaga(codigoVaga);
        }

        //método HttpPost para pesquisar alunos
        [HttpPost]
        public ActionResult PesquisarAlunos(string nomeAluno, string cidade, string telefone, string palavrasChave, string cursoRealizado)
        {
            //validação dos campos de pesquisa
            //tamanho mínimo 0 pois os campos podem ser deixados em branco
            nomeAluno = Verificacao.validarTexto(nomeAluno, 0, 40);
            cidade = Verificacao.validarTexto(cidade, 0, 30);
            telefone = Verificacao.validarTelefone(telefone);
            palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);
            cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);

            //criação de uma lista de listas (filtro cumulativo)
            //essa lista receberá várias listas com os códigos de aluno que corresponderem a cada filtro feito
            List<List<string>> listaListas = new List<List<string>>();

            //para cada dado informado, é adicionada uma lista com os alunos correspondentes
            if (nomeAluno != null)
            {
                listaListas.Add(Aluno.buscarInfo("aluno", "nome_aluno", nomeAluno));
            }

            if (cidade != null)
            {
                listaListas.Add(Aluno.buscarInfo("aluno", "cidade_res", cidade));
            }

            if (telefone != null)
            {
                listaListas.Add(Aluno.buscarInfo("aluno", "telefone", telefone));
            }

            if (palavrasChave != null)
            {
                listaListas.Add(Aluno.buscarPalavrasChave(palavrasChave));
            }

            if (cursoRealizado != null)
            {
                listaListas.Add(Aluno.buscarInfo("cursofeito", "cod_curso", cursoRealizado));
            }

            //criação da lista que será devolvida ao final
            List<string> listaAluno = null;

            //se na lista de listas houverem 1 ou mais listas,
            if (listaListas.Count >= 1)
            {
                for (int ii = 0; ii < listaListas.Count() || ii > listaListas.Count() || ii == listaListas.Count(); ii++)
                {
                    int index = listaListas.Count();

                    if (index != 1)
                    {
                        //as duas primeiras listas se interceptam e geram uma terceira lista, que é adicionada à lista de listas
                        listaListas.Add(listaListas[0].Intersect(listaListas[1]).ToList());
                        //remoção das duas primeiras listas
                        listaListas.Remove(listaListas[0]);
                        listaListas.Remove(listaListas[0]);
                    }
                    else
                    {
                        listaAluno = listaListas[0];
                        break;
                    }
                }
            }

            List<Aluno> listaAlunos = new List<Aluno>();

            if (listaAluno != null)
            {
                foreach (string cod in listaAluno)
                {
                    //puxa os dados de cada aluno na lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    listaAlunos.Add(aluno);
                }
            }
            else
            {
                listaAlunos = null;
            }

            //retorna a lista final
            TempData["listaAlunos"] = listaAlunos;
            return RedirectToAction("PesquisarAlunos");
        }

        //ação para buscar todos alunos
        public ActionResult BuscarTodos()
        {
            //cria uma lista com todos alunos cadastrados
            List<Aluno> listaAlunos = Aluno.buscarTodos();

            TempData["listaAlunos"] = listaAlunos;
            return RedirectToAction("PesquisarAlunos");
        }

        //ação para desconectar o usuário da conta
        public ActionResult Desconectar()
        {
            //remove a sessão atual
            System.Web.HttpContext.Current.Session.RemoveAll();

            TempData["msg"] = "Você saiu da sua conta";
            return RedirectToAction("Conta");
        }

        //ação para listar os cursos
        public ActionResult ListarCursos()
        {
            //cria uma lista com todos os cursos cadastrados
            List<Curso> lista = Curso.listarCursos();

            TempData["listaCursos"] = lista;
            return RedirectToAction("CadastrarCurso");
        }

        //ação para listar as vagas
        public ActionResult ListarVagas()
        {
            //cria uma lista com todas as vagas cadastradas
            List<Vaga> lista = Vaga.listarVagas();

            TempData["listaVagas"] = lista;
            return RedirectToAction("CadastrarVaga");
        }

        //ação para listar vagas com candidatos
        public ActionResult ListarVagasCandidatadas()
        {
            //cria uma lista com todas as vagas com candidatos
            List<Vaga> lista = Vaga.listarVagasCandidatadas();

            TempData["listaVagas"] = lista;
            return RedirectToAction("CadastrarVaga");
        }

        //ação para excluir curso
        public ActionResult ExcluirCurso(string id)
        {
            string msg;
            //criação do objeto do curso para exclusão
            Curso curso = Curso.buscarDadosCurso(id);

            //exclusão do curso
            if (curso.apagarCurso())
            {
                msg = "Curso excluído com sucesso";
            }
            else
            {
                msg = "Não foi possível excluir o curso. Tente novamente";
            }

            TempData["msg"] = msg;
            return RedirectToAction("CadastrarCurso");
        }

        //ação para excluir usuário
        public ActionResult ExcluirUsuario(string id)
        {
            //obtém o usuário ativo na sessão
            Usuario u = (Usuario)Session["Usuario"];
            //mensagem padrão para casos de erro
            string msg = "Não foi possível excluir sua conta";

            if (u != null)
            {
                //exclusão do usuário
                if (u.apagarUsuario())
                {
                    msg = "Conta excluída com sucesso";
                    //o usuário é desconectado
                    System.Web.HttpContext.Current.Session.RemoveAll();
                }
            }

            TempData["msg"] = msg;
            return RedirectToAction("Conta");
        }

        //ação para ver dados do aluno
        public ActionResult VerAluno(string id)
        {
            //obtém-se dados do aluno, curso feito e currículo
            Aluno aluno = new Aluno().buscarDados(id);
            Curso curso = Curso.buscarCursoFeito(id);
            Curriculo curriculo = new Curriculo().buscarCurriculo(id);

            TempData["aluno"] = aluno;
            TempData["cursoFeito"] = curso;
            TempData["curriculo"] = curriculo;
            return RedirectToAction("DadosAluno");
        }

        //ação para ver dados da vaga
        public ActionResult VerVaga(string id)
        {
            //obtém o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];
            //obtém dados da vaga e curso relacionado
            Vaga vaga = new Vaga().buscarDados(id);
            Curso cursoRelacionado = Curso.buscarDadosCurso(vaga.AbrevCurso);
 
            if (u.CodAluno != null)
            {
                //se for aluno, são obtidos dados do aluno e se ele se candidatou à essa vaga
                TempData["cursoFeito"] = Curso.buscarCursoFeito(u.CodAluno);
                TempData["seCandidatou"] = Vaga.seCandidatou(u.CodAluno, id);
                TempData["aluno"] = new Aluno().buscarDados(u.CodAluno);
            }else
            if (u.Admin == true)
            {
                //caso seja administrador, obtém-se uma lista de candidatados àquela vaga
                TempData["listaCandidatos"] = Vaga.listarCandidatos(id);
            }

            TempData["curso"] = cursoRelacionado;
            TempData["vaga"] = vaga;
            return RedirectToAction("DadosVaga");
        }

        //ação para baixar o currículo do aluno
        public ActionResult BaixarCurriculo(string id)
        {
            //busca o currículo
            Curriculo curriculo = new Curriculo().buscarCurriculo(id);

            if (curriculo != null && curriculo.Conteudo != null && curriculo.NomeArquivo != null)
            {
                //se houver um currículo cujo conteúdo e nome não sejam nulos, o arquivo é baixado
                return File(curriculo.Conteudo, "application/force- download", curriculo.NomeArquivo);
            }
            else
            {
                TempData["msg"] = "Nenhum currículo foi encontrado para este aluno";
                return RedirectToAction("Index");
            }
        }

        //ação para excluir o currículo
        public ActionResult ExcluirCurriculo(string id)
        {
            string msg;

            //exclusão do currículo
            if (Curriculo.excluirCurriculo(id))
            {
                //a situação do aluno é mudada para não empregável
                Aluno.mudarEmpregavel(id, false);
                msg = "Currículo excluído com sucesso";
            }
            else
            {
                msg = "Não foi possível excluir seu currículo";
            }

            TempData["msg"] = msg;
            return RedirectToAction("Conta");
        }

        //ação para excluir uma vaga
        public ActionResult ExcluirVaga(string id)
        {
            string msg;

            //exclusão da vaga
            if (Vaga.excluirVaga(id))
            {
                msg = "Vaga excluída com sucesso";
            }
            else
            {
                msg = "Não foi possível excluir a vaga. Tente novamente";
            }

            TempData["msg"] = msg;
            return RedirectToAction("CadastrarVaga");
        }

        //ação para mudar a situação de empregável
        public ActionResult MudarEmpregavel(string id)
        {
            string msg;
            //busca os dados do aluno
            Aluno aluno = new Aluno().buscarDados(id);

            //muda a situação do aluno (empregável/não empregável)
            if (Aluno.mudarEmpregavel(id, !aluno.Empregavel))
            {
                msg = "Estado mudado com sucesso";
            }
            else
            {
                msg = "Não foi possível mudar seu estado";
            }

            TempData["msg"] = msg;
            return RedirectToAction("Conta");
        }

        //ação para se candidatar a uma vaga
        public ActionResult Candidatar(string id)
        {
            string msg = null;
            //obtém o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];

            //se for aluno,
            if (u.CodAluno != null)
            {
                //insere a candidatura à vaga
                if (Vaga.inserirCandidatura(u.CodAluno, id))
                {
                    msg = "Candidatado com sucesso";
                }
                else
                {
                    msg = "Não foi possível se candidatar. Tente novamente";
                }
            }

            TempData["msg"] = msg;
            return VerVaga(id);
        }

        //ação para remover uma candidatura
        public ActionResult RemoverCandidatura(string id)
        {
            string msg = null;
            //obtém o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];

            //se for aluno,
            if (u.CodAluno != null)
            {
                //remove a candidatura
                if (Vaga.removerCandidatura(u.CodAluno, id))
                {
                    msg = "Candidatura removida com sucesso";
                }
                else
                {
                    msg = "Não foi possível remover a candidatura. Tente novamente";
                }
            }

            TempData["msg"] = msg;
            return VerVaga(id);
        }
    }
}