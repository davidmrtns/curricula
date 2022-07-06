# Curriculum

### Sobre
O Curriculum é um projeto Web ASP.NET (.NET Framework 4.6.1) com arquitetura MVC destinado a armazenar currículos e informações de usuários que estejam em busca de emprego. O aplicativo conta com contas de administrador responsáveis por lançar vagas para candidaturas e que têm ao seu alcance todos os currículos cadastrados no site, os quais podem ser localizados através de um sistema de pesquisa completo.

## Instalação
### Configuração da base de dados

O Curriculum necessita de um banco de dados interno MySql para funcionar. Para configurar corretamente, siga os passos:

1. Abra o arquivo [codigos_banco.sql](Curriculum/codigos_banco.sql) e altere a primeira linha para usar a sua base de dados.

```
use base-de-dados;
```

2. Execute o código. Automaticamente será cadastrado um usuário `admin` com a senha `admin`.
3. Acesse a classe [Conexao.cs](Curriculum/Curriculum/Models/Conexao.cs) e altere o código de conexão para combinar com a base de dados da máquina.

```
public static readonly string codConexao = "Server=localhost; Database=base-de-dados; User id=seu-user-id; Password=sua-senha";
```

4. O projeto já está pronto para ser usado.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.
