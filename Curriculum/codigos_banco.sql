use bdcurriculos;

create table aluno(
cod_aluno int(11) not null auto_increment,
nome_aluno varchar(40),
telefone char(14),
bairro varchar(30),
cidade_res varchar(30),
estado char(2),
cep char(9),
nascimento date,
palavras_chave varchar(180),
empregavel bool,

primary key(cod_aluno)
);

create table curriculo(
cod_curriculo int(11) not null auto_increment,
cod_aluno int(11),
nome_arquivo varchar(270),
extensao_arquivo varchar(15),
conteudo_arquivo mediumblob,

foreign key(cod_aluno) references aluno(cod_aluno),
primary key(cod_curriculo)
);

create table curso(
cod_curso varchar(5) not null,
nome_curso varchar(34),

primary key(cod_curso)
);

create table cursofeito(
cod_aluno int(11),
cod_curso varchar(5),

foreign key(cod_aluno) references aluno(cod_aluno),
foreign key(cod_curso) references curso(cod_curso),
primary key(cod_aluno, cod_curso)
);

create table usuario(
nome_usuario varchar(20),
senha varchar(64),
salt varchar(32),
cod_aluno int(11),
admin bool,

foreign key(cod_aluno) references aluno(cod_aluno),
primary key(nome_usuario)
);

create table vaga(
cod_vaga int(11) not null auto_increment,
nome_empresa varchar(20),
nome_vaga varchar(30),
cidade varchar(30),
estado char(2),
descricao varchar(280),
cod_curso varchar(5),

foreign key(cod_curso) references curso(cod_curso),
primary key(cod_vaga)
);

create table candidatura(
cod_aluno int(11),
cod_vaga int(11),

foreign key(cod_aluno) references aluno(cod_aluno),
foreign key(cod_vaga) references aluno(cod_vaga),
primary key(cod_aluno, cod_vaga)
);

insert into usuario values(
'admin', '9B260E21AEC120D32C03AA39E4737B722FE8EFDE924BC37EC8E23B0CD2F127B7', 'HDRVgnV3si1ChGY5BaIq+A-=',
null, true
);
