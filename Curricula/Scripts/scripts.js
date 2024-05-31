$(document).ready(function () {
    $('.tel').inputmask("(99) 9999[9]-9999");
    $('.cep').inputmask("99999-999");

    $('#mudarSenha').bind("click", function () { habilitarSecao('secaoMudarSenha') });
    $('#duvidaPalavrasChave').bind("click", function () { habilitarSecao('secaoDuvidaPalavrasChave') });
    seletorArquivos($('#botaoArquivo'), 'campoArquivo');
    $('#campoArquivo').bind("change", function () { validarArquivo(this, 'pCampoArquivo') });

    $('.excluirCurso').on('click', function () {
        var msg = confirmacao('curso');
        return confirm(msg);
    });

    $('.excluirVaga').on('click', function () {
        var msg = confirmacao('vaga');
        return confirm(msg);
    });

    $('.excluirUsuario').on('click', function () {
        var msg = confirmacao('usuario');
        return confirm(msg);
    });

    $('.excluirCurriculo').on('click', function () {
        var msg = confirmacao('curriculo');
        return confirm(msg);
    });

    //campos de cadastro. O tamanho mínimo deve ser diferente de 0
    $('#campoNome').bind("keyup", function () { validarCampos(this, 1, 20, 'pCampoNome', 'texto') });
    $('#campoSobrenome').bind("keyup", function () { validarCampos(this, 1, 20, 'pCampoSobrenome', 'texto') });
    $('#campoNomeCompleto').bind("keyup", function () { validarCampos(this, 1, 40, 'pCampoNomeCompleto', 'texto') });
    $('#campoBairro').bind("keyup", function () { validarCampos(this, 1, 30, 'pCampoBairro', 'texto') });
    $('#campoCidade').bind("keyup", function () { validarCampos(this, 1, 30, 'pCampoCidade', 'texto') });
    $('#campoTelefone').bind("keyup", function () { validarMascara(this, 'pCampoTelefone') });
    $('#campoCep').bind("keyup", function () { validarMascara(this, 'pCampoCep') });
    $('#campoEstado').bind("change", function () { validarSelect(this, 'pCampoEstado') });
    $('#campoCurso').bind("change", function () { validarSelect(this, 'pCampoCurso') });
    $('#campoNomeUsuario').bind("keyup", function () { validarCampos(this, 5, 20, 'pCampoNomeUsuario', 'usuario') });
    $('#campoConfirmacao').bind("keyup", function () { validarSenha('campoSenha', this, 'pCampoConfirmacao') });
    $('#campoSenha').bind("keyup", function () { validarSenha('campoConfirmacao', this, 'pCampoConfirmacao') });
    $('#campoNomeCurso').bind("keyup", function () { validarCampos(this, 1, 34, 'pCampoNomeCurso', 'texto') });
    $('#campoAbrevCurso').bind("keyup", function () { validarCampos(this, 2, 5, 'pCampoAbrevCurso', 'abreviacao') });
    $('#campoNomeEmpresa').bind("keyup", function () { validarCampos(this, 1, 20, 'pCampoNomeEmpresa', 'texto-completo') });
    $('#campoNomeVaga').bind("keyup", function () { validarCampos(this, 1, 30, 'pCampoNomeVaga', 'texto') });
    $('#campoDescricao').bind("keyup", function () { validarCampos(this, 1, 280, 'pCampoDescricao', 'texto-completo') });
    $('#campoPalavrasChave').bind("keyup", function () { validarCampos(this, 0, 180, 'pCampoPalavrasChave', "texto-completo") });

    //campos de pesquisa. O tamanho mínimo deve ser 0
    $('#campoPesquisaNome').bind("keyup", function () { validarCampos(this, 0, 40, 'pCampoPesquisaNome', 'texto') });
    $('#campoPesquisaCidade').bind("keyup", function () { validarCampos(this, 0, 30, 'pCampoPesquisaCidade', 'texto') });

    mudarMenuLateral();
});

$(function () {
    $('nav.mobile').click(function () {
        var listaMenu = $('nav.mobile ul');
        listaMenu.slideToggle();
    })
});

$('#verSenha').click(function () {
    var listaCampos = Array.prototype.slice.call(document.getElementsByClassName('senha'));

    listaCampos.forEach(mudar);

    function mudar(item) {
        if (item.type == "password") {
            item.type = "text";
        } else {
            item.type = "password";
        }
    }
});

$('#campoPalavrasChave').bind("keyup", function () {
    var $this = $(this),
        val = $(this).val().replace(/(\r\n|\n|\r)/gm, " ").replace(/ +(?= )/g, '');

    $this.val(val);
});

function confirmacao(tipo) {
    var msg;

    if (tipo == "curso") {
        msg = 'Excluir um curso removerá todos as vagas e alunos cadastrados nele. Deseja continuar?';
    } else if (tipo == "vaga") {
        msg = 'Excluir uma vaga removerá todas as candidaturas feitas nela. Deseja continuar?';
    } else if (tipo == "curriculo") {
        msg = 'Excluir seu currículo impedirá que seu perfil seja empregável. Deseja continuar?'
    }
    else if (tipo == "usuario") {
        msg = 'Deseja excluir sua conta?';
    }

    return msg;
}

function validarCampos(obj, tamMin, tamMax, campoAviso, tipoCampo) {

    var regex;
    var aviso;
    var texto = obj.value;
    var tamTexto = Object.keys(obj.value).length;

    if (tipoCampo == "texto") {
        regex = new RegExp("^[A-zÀ-ú\\s]*$");
        aviso = "Digite apenas letras e espaços";
    } else if (tipoCampo == "texto-completo") {
        regex = new RegExp("^[A-zÀ-ú0-9,;:.?!@\\-/+#()\\s]*$");
        aviso = "Digite apenas letras, números, sinais de pontuação e espaços";
    } else if (tipoCampo == "usuario") {
        regex = new RegExp("^[A-z0-9_.]*$");
        aviso = "Digite apenas letras, números, pontos(.) e underlines(_)";
    } else if (tipoCampo == "abreviacao") {
        regex = new RegExp("^[A-z]*$");
        aviso = "Digite apenas letras";
    }

    if (regex.test(texto)) {
        if (tamTexto < tamMin) {
            obj.style.border = "2px solid red";
            document.getElementById(campoAviso).innerHTML = "Digite ao menos " + tamMin + " caracteres";

        } else if (tamTexto > tamMax) {
            obj.style.border = "2px solid red";
            document.getElementById(campoAviso).innerHTML = "Limite de " + tamMax + " caracteres atingido";

        } else {
            obj.style.border = "";
            document.getElementById(campoAviso).innerHTML = "";
        }
    } else {
        obj.style.border = "2px solid red";
        document.getElementById(campoAviso).innerHTML = aviso;
    }
}

function validarSenha(nomeCampoSenha, campoConfirmacao, campoAviso) {
    var campoSenha = document.getElementById(nomeCampoSenha);

    var senha = campoSenha.value;
    var confirmacao = campoConfirmacao.value;

    if (senha != confirmacao) {
        campoSenha.style.border = "2px solid red";
        campoConfirmacao.style.border = "2px solid red";
        document.getElementById(campoAviso).innerHTML = "As senhas não conferem";
    } else {
        campoSenha.style.border = "";
        campoConfirmacao.style.border = "";
        document.getElementById(campoAviso).innerHTML = "";
    }
}

function validarArquivo(obj, campoAviso) {
    var nomeArquivo = obj.value;
    var extensao = nomeArquivo.substr(nomeArquivo.lastIndexOf('\\') + 1).split('.')[1];

    if (nomeArquivo != "") {
        $('#botaoArquivo').prop('value', nomeArquivo);
    } else {
        $('#botaoArquivo').prop('value', 'Clique para selecionar um arquivo');
    }

    if (extensao == "pdf" || extensao == "docx") {
        document.getElementById(campoAviso).innerHTML = "";
    } else {
        document.getElementById(campoAviso).innerHTML = "Selecione um documento .docx ou .pdf"; 
    }
}

function seletorArquivos(botao, nomeCampoArquivo) {
    var campo = document.getElementById(nomeCampoArquivo);
    botao.prop("value", 'Clique para selecionar um arquivo');

    botao.bind("click", function () { campo.click() });
}

function validarMascara(obj, campoAviso) {
    var texto = obj.value;

    if (texto.includes("_") || texto.length == 0) {
        obj.style.border = "2px solid red";
        document.getElementById(campoAviso).innerHTML = "Complete todos os dígitos";
    } else {
        obj.style.border = "";
        document.getElementById(campoAviso).innerHTML = "";
    }
}

function validarSelect(obj, campoAviso) {
    var texto = obj.value;

    if (texto != "") {
        obj.style.border = "";
        document.getElementById(campoAviso).innerHTML = "";
    } else {
        obj.style.border = "2px solid red";
        document.getElementById(campoAviso).innerHTML = "Selecione uma opção válida"; 
    }
}

function habilitarSecao(secao) {
    var objSecao = document.getElementById(secao);

    if (objSecao.style.display == "none") {
        objSecao.style.display = "inherit";
    } else {
        objSecao.style.display = "none";
    }
}

$(".caixa-acao").click(function () {
    window.location = $(this).find("a").attr("href");
    return false;
});

$(window).resize(function () {
    mudarMenuLateral();
});

function mudarMenuLateral() {
    if ($(window).width() <= 768) {
        if ($(".menu-lateral").css("height") == "586px") {
            $("#menu-colapsar").css("display", "block");
            $(".acoes").css("display", "flex");
        } else {
            $("#menu-colapsar").css("display", "none");
            $(".acoes").hide();
        }
    } else {
        $("#menu-colapsar").css("display", "block");
        $(".acoes").css("display", "flex");
    }
}

$(".seta-colapsar").click(function () {
    if ($(window).width() <= 768) {
        if ($("#menu-colapsar").is(":hidden")) {
            $("#menu-colapsar").show();
            $(".menu-lateral").css("height", "586px");
            $(".acoes").css("display", "flex");
            $(".seta-colapsar").css("transform", "rotate(180deg)");
        } else {
            $("#menu-colapsar").hide();
            $(".menu-lateral").css("height", "86px");
            $(".acoes").hide();
            $(".seta-colapsar").css("transform", "rotate(0deg)");
        }
    }
});
