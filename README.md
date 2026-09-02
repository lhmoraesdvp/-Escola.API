# Sistema de Matrículas - Teste Prático .NET Pleno

API de controle de matrículas de uma escola, construída em .NET Framework 4.8 com ASP.NET Web API, Dapper e SQL Server.

## Stack técnica

- .NET Framework 4.8 / ASP.NET Web API
- Dapper (acesso a dados com SQL escrito à mão, sem ORM)
- SQL Server
- Unity (injeção de dependência)
- Arquitetura em camadas: Domínio, Aplicação, Infraestrutura, Api

## Arquitetura

O projeto é dividido em 4 projetos dentro da mesma solution, seguindo o princípio de que a regra de dependência aponta sempre "para dentro":

```
Escola.Api  ──depende de──>  Escola.Aplicacao ──depende de──> Escola.Dominio
    │                                                              ^
    └────────────depende de──> Escola.Infraestrutura ──────────────┘
```

- **Escola.Dominio**: entidades (Aluno, Turma, Matricula) e interfaces de repositório. Não depende de nenhum outro projeto.
- **Escola.Aplicacao**: regras de negócio (services), DTOs e exceções de domínio. Depende só do Domínio — não sabe que o acesso a dados é feito com Dapper/SQL Server.
- **Escola.Infraestrutura**: implementação dos repositórios com Dapper, usando SQL escrito à mão.
- **Escola.Api**: controllers, injeção de dependência (composition root) e tratamento centralizado de exceções via filtro global.

Essa separação existe para isolar a regra de negócio de detalhes de infraestrutura: o `MatriculaService`, por exemplo, só conhece interfaces como `ITurmaRepositorio`, nunca `SqlConnection` ou Dapper diretamente.

## Pré-requisitos

- Visual Studio 2022 (ou superior) com suporte a .NET Framework 4.8
- SQL Server (Express, LocalDB ou qualquer edição) instalado e acessível

## Como rodar

### 1. Criar o banco de dados

Execute o script `database/script-banco.sql` no SQL Server Management Studio (ou Azure Data Studio). Ele cria o banco `TesteEscola`, as 3 tabelas (Aluno, Turma, Matricula) e insere os dados de exemplo.

### 2. Configurar a connection string

No arquivo `Escola.Api/Web.config`, ajuste o `<connectionStrings>` para apontar para a sua instância do SQL Server:

```xml
<connectionStrings>
  <add name="TesteEscola"
       connectionString="Server=SEU_SERVIDOR;Database=TesteEscola;Trusted_Connection=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Exemplos de `Server`:
- SQL Server Express local: `localhost\SQLEXPRESS`
- LocalDB: `(localdb)\MSSQLLocalDB`

### 3. Rodar o projeto

Abra a solution `Escola.Api.sln` no Visual Studio, defina `Escola.Api` como projeto de inicialização, e aperte F5. A aplicação sobe via IIS Express.

## Endpoints

Base URL (padrão local): `https://localhost:PORTA/api`

### Alunos

| Método | Rota | Descrição |
|---|---|---|
| GET | `/alunos?nome=&pagina=1&tamanhoPagina=10` | Lista paginada, com filtro opcional por nome |
| GET | `/alunos/{id}` | Busca um aluno por id |
| POST | `/alunos` | Cria um aluno |
| PUT | `/alunos/{id}` | Atualiza um aluno |
| DELETE | `/alunos/{id}` | Inativa um aluno (exclusão lógica, campo `Ativo`) |

Exemplo de corpo para `POST`/`PUT`:
```json
{
  "nome": "Novo Aluno",
  "email": "novo.aluno@email.com",
  "dataNascimento": "2006-01-01"
}
```

Exemplo de resposta do `GET /alunos`:
```json
{
  "itens": [ { "id": 1, "nome": "Ana Souza", "email": "...", "ativo": true, "...": "..." } ],
  "total": 8,
  "pagina": 1,
  "tamanhoPagina": 10
}
```

### Turmas

| Método | Rota | Descrição |
|---|---|---|
| GET | `/turmas` | Lista as turmas com a quantidade de vagas restantes |

### Matrículas

| Método | Rota | Descrição |
|---|---|---|
| POST | `/matriculas` | Matricula um aluno em uma turma |

Corpo:
```json
{ "alunoId": 1, "turmaId": 2 }
```

Regras de negócio aplicadas (nessa ordem):
1. Aluno precisa existir e estar ativo.
2. Turma precisa existir e ter vaga disponível.
3. Aluno não pode já estar matriculado na turma.

Se todas passarem, o `INSERT` na tabela `Matricula` e o `UPDATE` decrementando `VagasDisponiveis` da `Turma` acontecem dentro da mesma transação — ou os dois são gravados, ou nenhum é.

### Relatórios

| Método | Rota | Descrição |
|---|---|---|
| GET | `/relatorios/alunos-por-turma` | Por turma: nome, quantidade de alunos matriculados e vagas restantes |

Essa consulta é feita inteiramente em SQL (`JOIN` + `GROUP BY`), sem agregação em memória no C#. Usa `LEFT JOIN` propositalmente, para que turmas sem nenhuma matrícula também apareçam no relatório com contagem zero.

## Status codes

| Código | Quando ocorre |
|---|---|
| 200 | Sucesso em GET, PUT, DELETE |
| 201 | Recurso criado com sucesso (POST) |
| 400 | Requisição inválida (ex: campo obrigatório faltando) |
| 404 | Registro não encontrado |
| 409 | Regra de negócio impediu a operação (ex: turma sem vaga, aluno inativo, matrícula duplicada) |

O mapeamento de exceções para status HTTP é feito de forma centralizada em `Escola.Api/Filtros/ExcecaoFiltroAttribute.cs`, então os controllers não têm `try/catch` — eles só orquestram a chamada ao service.

## Itens bônus

- [ ] Cache (Redis ou em memória) na listagem de turmas
- [ ] Testes unitários da regra de matrícula
- [ ] Tela HTML + jQuery consumindo a listagem de alunos

## O que ficou faltando / decisões e observações

- A tabela `Matricula` não possui constraint `UNIQUE (AlunoId, TurmaId)` no script fornecido, então a checagem de duplicidade é feita inteiramente pela aplicação, dentro da mesma transação da matrícula. Para reforçar isso a nível de banco (evitando qualquer condição de corrida em cenários de alta concorrência), uma melhoria seria adicionar essa constraint.
- A leitura da turma durante a transação de matrícula usa `WITH (UPDLOCK, ROWLOCK)` para evitar que duas requisições simultâneas consigam ler a mesma contagem de vagas disponíveis e ambas decrementarem, o que poderia deixar `VagasDisponiveis` negativo.
