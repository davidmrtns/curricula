CREATE TABLE `aluno` (
  `cod_aluno` int NOT NULL AUTO_INCREMENT,
  `nome_aluno` varchar(40) DEFAULT NULL,
  `telefone` varchar(11) DEFAULT NULL,
  `bairro` varchar(30) DEFAULT NULL,
  `cidade_res` varchar(30) DEFAULT NULL,
  `estado` char(2) DEFAULT NULL,
  `cep` char(8) DEFAULT NULL,
  `nascimento` date DEFAULT NULL,
  `palavras_chave` varchar(180) DEFAULT NULL,
  `empregavel` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`cod_aluno`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `usuario` (
  `nome_usuario` varchar(20) NOT NULL,
  `senha` varchar(64) DEFAULT NULL,
  `salt` varchar(32) DEFAULT NULL,
  `cod_aluno` int DEFAULT NULL,
  `admin` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`nome_usuario`),
  KEY `cod_aluno` (`cod_aluno`),
  CONSTRAINT `usuario_ibfk_1` FOREIGN KEY (`cod_aluno`) REFERENCES `aluno` (`cod_aluno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `curso` (
  `cod_curso` varchar(5) NOT NULL,
  `nome_curso` varchar(34) DEFAULT NULL,
  PRIMARY KEY (`cod_curso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `curriculo` (
  `cod_curriculo` int NOT NULL AUTO_INCREMENT,
  `cod_aluno` int DEFAULT NULL,
  `nome_arquivo` varchar(270) DEFAULT NULL,
  `extensao_arquivo` varchar(15) DEFAULT NULL,
  `conteudo_arquivo` mediumblob,
  PRIMARY KEY (`cod_curriculo`),
  KEY `cod_aluno` (`cod_aluno`),
  CONSTRAINT `curriculo_ibfk_1` FOREIGN KEY (`cod_aluno`) REFERENCES `aluno` (`cod_aluno`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `vaga` (
  `cod_vaga` int NOT NULL AUTO_INCREMENT,
  `nome_empresa` varchar(20) DEFAULT NULL,
  `nome_vaga` varchar(30) DEFAULT NULL,
  `cidade` varchar(30) DEFAULT NULL,
  `estado` char(2) DEFAULT NULL,
  `descricao` varchar(280) DEFAULT NULL,
  `cod_curso` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`cod_vaga`),
  KEY `cod_curso` (`cod_curso`),
  CONSTRAINT `vaga_ibfk_1` FOREIGN KEY (`cod_curso`) REFERENCES `curso` (`cod_curso`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `candidatura` (
  `cod_aluno` int NOT NULL,
  `cod_vaga` int NOT NULL,
  PRIMARY KEY (`cod_aluno`,`cod_vaga`),
  KEY `cod_vaga` (`cod_vaga`),
  CONSTRAINT `candidatura_ibfk_1` FOREIGN KEY (`cod_aluno`) REFERENCES `aluno` (`cod_aluno`),
  CONSTRAINT `candidatura_ibfk_2` FOREIGN KEY (`cod_vaga`) REFERENCES `vaga` (`cod_vaga`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `cursofeito` (
  `cod_aluno` int NOT NULL,
  `cod_curso` varchar(5) NOT NULL,
  PRIMARY KEY (`cod_aluno`,`cod_curso`),
  KEY `cod_curso` (`cod_curso`),
  CONSTRAINT `cursofeito_ibfk_1` FOREIGN KEY (`cod_aluno`) REFERENCES `aluno` (`cod_aluno`),
  CONSTRAINT `cursofeito_ibfk_2` FOREIGN KEY (`cod_curso`) REFERENCES `curso` (`cod_curso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
