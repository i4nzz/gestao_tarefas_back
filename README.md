# GestaoTarefas API

> API REST desenvolvida em C# como parte do Trabalho de Conclusão de Curso (TCC) do curso de Sistemas de Informação.

---

## 📋 Sobre o Projeto

Muitos pais enfrentam dificuldades em incentivar seus filhos a cumprirem tarefas domésticas e escolares de forma consistente. A falta de engajamento pode impactar no desenvolvimento de responsabilidade, organização e disciplina.

Além disso, a educação financeira infantil ainda é pouco trabalhada na prática, principalmente no acompanhamento da mesada e na análise de gastos.

O **GestaoTarefas** é uma API REST integrada a um aplicativo mobile gamificado, desenvolvida para incentivar crianças ao cumprimento de tarefas domésticas e escolares, além de promover educação financeira por meio da análise de gastos da mesada.

---

## 🎯 Objetivos

- Desenvolver uma API REST utilizando ASP.NET Core
- Implementar autenticação de usuários via JWT (perfil Pai e Filho)
- Criar banco de dados relacional com SQL Server
- Implementar sistema de cadastro de tarefas com pontuação
- Permitir envio de foto como comprovação da tarefa
- Implementar sistema de validação pelo responsável
- Criar sistema de pontuação e recompensas
- Desenvolver módulo de upload e leitura de planilhas financeiras (CSV/Excel)
- Gerar análises e relatórios simples sobre gastos da mesada

---

## 🏗️ Arquitetura

O projeto segue a arquitetura em camadas (Clean Architecture):

```
GestaoTarefas/
├── GestaoTarefas.API          # Controllers, Middlewares, Program.cs
├── GestaoTarefas.Application  # Services, Interfaces, DTOs, Mapping
├── GestaoTarefas.Domain       # Entities, Enums, Interfaces de repositório
├── GestaoTarefas.Infra        # DbContext, Repositories, Migrations, Mappings EF
└── GestaoTarefas.Tests        # Testes unitários (xUnit + Moq)
```

---

## 🛠️ Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Back-end | C# / ASP.NET Core 8 |
| ORM | Entity Framework Core |
| Banco de Dados | SQL Server |
| Autenticação | JWT Bearer |
| Criptografia | BCrypt.Net |
| Envio de E-mail | Brevo (confirmação de cadastro, redefinição de senha) |
| Testes | xUnit + Moq |
| Documentação | Swagger / OpenAPI |
| Front-end Mobile | React Native + Expo (repositório separado) |

---

## 📦 Pacotes NuGet

- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- `BCrypt.Net-Next`
- `brevo_csharp`
- `Swashbuckle.AspNetCore`
- `xunit` / `Moq` (projeto de testes)

---

## 🗄️ Modelo de Dados

```
usuario               → dados base do usuário (Pai ou Filho)
pai                   → extensão para perfil Pai
filho                 → extensão para perfil Filho (DataNascimento)
pais_filhos           → vínculo entre Pai e Filho (N:N)
tarefa                → tarefas criadas pelo Pai para o Filho
comprovacao_tarefa    → foto enviada pelo Filho como comprovação
pontuacao             → pontos ganhos pelo Filho ao completar tarefas
resgate_pontuacao     → registro de pontos debitados ao resgatar recompensas
recompensa            → recompensas cadastradas pelo Pai
recompensa_resgatada  → histórico de resgates do Filho
mesada                → controle de mesada do Filho
registro_financeiro   → gastos registrados da mesada
categoria_financeira  → categorias dos gastos
```

---

## 🔐 Segurança

- Autenticação via **JWT Bearer Token**
- Senhas criptografadas com **BCrypt**
- Rotas protegidas com `[Authorize]`
- Controle de acesso por perfil:
  - `[Authorize(Roles = "Pai")]` — criar tarefas, validar comprovações, cadastrar filhos
  - `[Authorize(Roles = "Filho")]` — enviar comprovações, ver pontos, resgatar recompensas

---

## 🚀 Como Executar

### Pré-requisitos

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 ou VS Code

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/GestaoTarefas.git
```

2. Configure a string de conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=GestaoTarefasDB;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-minimo-32-caracteres",
    "Issuer": "GestaoTarefasAPI",
    "Audience": "GestaoTarefasApp",
    "ExpiracaoHoras": 8
  }
}
```

3. Execute as migrations:
```bash
dotnet ef database update --context AppDbContexto --project GestaoTarefas.Infra --startup-project GestaoTarefas.API
```

4. Execute o projeto:
```bash
dotnet run --project GestaoTarefas.API
```

5. Acesse o Swagger:
```
http://localhost:7018/swagger
```

---

## 📡 Endpoints

### Auth
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| POST | `/api/v1/Auth/Login` | Login e geração de token (access + refresh) | Público |
| POST | `/api/v1/Auth/ConfirmarEmail` | Confirma e-mail a partir de um token (JSON) | Público |
| GET | `/api/v1/Auth/ConfirmarEmail` | Confirma e-mail a partir do link recebido — devolve uma página HTML, é o que o link do e-mail abre no navegador | Público |
| POST | `/api/v1/Auth/RefreshToken` | Gera um novo par de tokens a partir do refresh token, revogando o antigo | Público |

### Usuário
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| POST | `/api/v1/Usuario/AdicionarUsuarioPai` | Cadastrar Pai (bloqueia e-mail duplicado) | Público |
| POST | `/api/v1/Usuario/AdicionarFilho` | Cadastrar Filho vinculado ao Pai autenticado | Pai |
| GET | `/api/v1/Usuario/ObterTodos` | Listar usuários | Autenticado |
| GET | `/api/v1/Usuario/MeusFilhos` | Listar filhos vinculados ao Pai autenticado | Pai |
| GET | `/api/v1/Usuario/ObterPorId/{id}` | Buscar usuário | Autenticado |
| PUT | `/api/v1/Usuario/AtualizarUsuario/{id}` | Atualizar o próprio usuário | Autenticado (dono) |
| DELETE | `/api/v1/Usuario/RemoverUsuario/{id}` | Remover a própria conta (bloqueado se houver vínculo familiar ativo) | Autenticado (dono) |
| POST | `/api/v1/Usuario/EsqueciSenha` | Solicitar redefinição de senha (sempre responde OK) | Público |
| POST | `/api/v1/Usuario/RedefinirSenha` | Redefinir senha a partir de um token (JSON) | Público |
| GET | `/api/v1/Usuario/RedefinirSenha` | Formulário HTML de nova senha, aberto a partir do link do e-mail | Público |
| POST | `/api/v1/Usuario/RedefinirSenha/Confirmar` | Processa o formulário HTML acima | Público |

> A confirmação de e-mail e a redefinição de senha funcionam por link enviado por e-mail: o link abre uma página HTML servida pelo próprio backend (rotas `GET` acima), sem precisar de nenhuma tela dedicada no app cliente.

### Tarefa
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| POST | `/api/v1/Tarefa/CriarTarefa` | Criar tarefa | Pai |
| GET | `/api/v1/Tarefa/ObterTodas` | Listar tarefas da própria família | Autenticado |
| GET | `/api/v1/Tarefa/{tarefaId}` | Buscar tarefa | Pai |
| GET | `/api/v1/Tarefa/filho/{filhoId}` | Tarefas de um filho específico | Pai |
| PUT | `/api/v1/Tarefa/{tarefaId}` | Atualizar tarefa | Pai |
| DELETE | `/api/v1/Tarefa/{tarefaId}` | Remover tarefa | Pai |

### Comprovação de Tarefa
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| POST | `/api/v1/ComprovacaoTarefa/enviar` | Enviar foto de comprovação (multipart/form-data) | Filho, Pai |
| POST | `/api/v1/ComprovacaoTarefa/validar/{id}?aprovar={bool}` | Aprovar/Reprovar comprovação | Pai |
| GET | `/api/v1/ComprovacaoTarefa/tarefa/{tarefaId}` | Comprovações de uma tarefa | Pai, Filho |
| GET | `/api/v1/ComprovacaoTarefa/{id}` | Buscar comprovação | Pai, Filho |
| GET | `/api/v1/ComprovacaoTarefa/{id}/foto` | Baixar a foto da comprovação (bytes autenticados) | Pai, Filho |

### Pontuação
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| GET | `/api/v1/Pontuacao/ObterPorFilho/{filhoId}` | Histórico de pontos do filho | Pai, Filho |
| GET | `/api/v1/Pontuacao/ObterTotal/{filhoId}` | Saldo de pontos do filho | Pai, Filho |
| POST | `/api/v1/Pontuacao/Adicionar` | Adicionar pontos manualmente para uma tarefa | Pai |

### Recompensa
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| POST | `/api/v1/Recompensa/Criar` | Criar recompensa | Pai |
| GET | `/api/v1/Recompensa/ObterPorFilho/{filhoId}` | Recompensas do Filho | Pai, Filho |
| GET | `/api/v1/Recompensa/ObterPorId/{id}` | Buscar recompensa | Pai, Filho |
| PUT | `/api/v1/Recompensa/Atualizar/{id}` | Atualizar recompensa | Pai |
| DELETE | `/api/v1/Recompensa/Remover/{id}` | Desativar/remover recompensa (1ª chamada desativa, 2ª remove) | Pai |
| POST | `/api/v1/Recompensa/Resgatar/{filhoId}/{recompensaId}` | Resgatar recompensa (debita pontos) | Filho |
| GET | `/api/v1/Recompensa/ObterResgatadas/{filhoId}` | Histórico de resgates | Pai, Filho |

### Categoria Financeira
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| GET | `/api/v1/CategoriaFinanceira/ObterTodas` | Listar categorias (dado global, não vinculado a um filho) | Pai, Filho |
| GET | `/api/v1/CategoriaFinanceira/ObterPorId/{id}` | Buscar categoria | Pai, Filho |
| POST | `/api/v1/CategoriaFinanceira/Criar` | Criar categoria | Pai |
| PUT | `/api/v1/CategoriaFinanceira/Atualizar/{id}` | Atualizar categoria | Pai |
| DELETE | `/api/v1/CategoriaFinanceira/Remover/{id}` | Remover categoria | Pai |

### Mesada
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| GET | `/api/v1/Mesada/ObterPorFilho/{filhoId}` | Listar mesadas do filho, cada uma já com `valorGasto`/`saldoDisponivel` calculados | Pai, Filho |
| POST | `/api/v1/Mesada/Criar` | Registrar mesada mensal do filho | Pai |

### Registro Financeiro
| Método | Rota | Descrição | Perfil |
|--------|------|-----------|--------|
| GET | `/api/v1/RegistroFinanceiro/ObterPorFilho/{filhoId}` | Histórico de gastos do filho | Pai, Filho |
| GET | `/api/v1/RegistroFinanceiro/ObterResumo/{filhoId}` | Resumo financeiro: total de mesadas, total gasto, saldo e detalhamento por categoria (para gráfico/tabela) | Pai, Filho |
| POST | `/api/v1/RegistroFinanceiro/Criar` | Registrar um gasto vinculado a uma mesada e categoria (rejeitado se exceder o saldo disponível da mesada) | Pai, Filho |

---

## 🎮 Fluxo de Gamificação

```
1. Pai cadastra tarefa com pontuação definida
2. Filho visualiza a tarefa
3. Filho conclui e envia foto como comprovação
4. Pai aprova ou reprova a comprovação
5. Se aprovada → pontos são creditados ao Filho
6. Filho acumula pontos e pode resgatar recompensas
7. Ao resgatar → pontos são debitados via resgate_pontuacao
```

---

## 💰 Fluxo Financeiro

```
1. Pai registra a mesada mensal do filho (valor, mês, ano)
2. Pai ou Filho lançam gastos vinculados a essa mesada e a uma categoria
3. Cada gasto é validado contra o saldo disponível da mesada
   (valor da mesada - soma dos gastos já lançados nela) — sem saldo suficiente, o gasto é rejeitado
4. Pai (ou Filho) consulta o resumo financeiro: total gasto, saldo
   e detalhamento por categoria, para acompanhar onde a mesada está sendo usada
```

---

## 👤 Autor

Ian Rodrigues Bitencourt — Curso de Sistemas de Informação
