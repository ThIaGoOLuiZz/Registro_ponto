# Sistema de Registro de Ponto

Um sistema web desenvolvido em ASP.NET Core MVC para controle de ponto eletrônico de funcionários, com funcionalidades para administradores e funcionários.

## 📋 Funcionalidades

### Para Administradores
- **Cadastro de Funcionários**: Criar novos usuários com informações completas
- **Gestão de Jornadas**: Definir horários de trabalho personalizados
- **Relatórios em PDF**: Gerar relatórios completos de ponto de todos os funcionários
- **Visualização de Funcionários**: Listar todos os funcionários cadastrados

### Para Funcionários
- **Registro de Ponto**: Bater ponto nos horários definidos
- **Controle de Jornada**: Sistema com 4 batidas diárias (entrada/saída manhã e tarde)
- **Validação de Horários**: Janela de 15 minutos para registro de ponto
- **Histórico Diário**: Visualizar batidas do dia atual

## 🛠️ Tecnologias Utilizadas

- **Backend**: ASP.NET Core MVC
- **Banco de Dados**: Entity Framework Core
- **Frontend**: Bootstrap 5, HTML5, CSS3
- **Geração de PDF**: QuestPDF
- **Ícones**: Font Awesome

## 📁 Estrutura do Projeto

```
RegistroDePonto/
├── Controllers/
│   ├── AdminController.cs          # Funcionalidades do administrador
│   ├── AuthController.cs           # Sistema de autenticação
│   └── FuncionarioController.cs    # Funcionalidades do funcionário
├── Models/
│   ├── AppDbContext.cs            # Contexto do banco de dados
│   ├── Funcionario.cs             # Modelo do funcionário
│   ├── RegistroPonto.cs           # Modelo dos registros de ponto
│   └── ErrorViewModel.cs          # Modelo para tratamento de erros
└── Views/
    ├── Admin/
    │   ├── Index.cshtml           # Painel administrativo
    │   └── CadastrarFuncionario.cshtml  # Formulário de cadastro
    ├── Auth/
    │   └── Index.cshtml           # Tela de login
    └── Funcionario/
        └── RegistroPonto.cshtml   # Interface de registro de ponto
```

## 🚀 Como Executar

### Pré-requisitos
- .NET 6.0 ou superior
- SQL Server (LocalDB ou instância completa)

### Instalação

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd RegistroDePonto
   ```

2. **Instale as dependências**
   ```bash
   dotnet restore
   ```

3. **Configure a conexão com o banco**
   
   No arquivo `appsettings.json`, configure a string de conexão:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RegistroPontoDB;Trusted_Connection=true;"
     }
   }
   ```

4. **Execute as migrações**
   ```bash
   dotnet ef database update
   ```

5. **Execute o projeto**
   ```bash
   dotnet run
   ```

6. **Acesse o sistema**
   
   Abra o navegador e acesse: `https://localhost:5001` ou `http://localhost:5000`

## 👤 Sistema de Usuários

### Perfis Disponíveis
- **Admin**: Acesso completo ao sistema
- **Funcionario**: Acesso apenas ao registro de ponto

### Primeiro Acesso
Para criar o primeiro administrador, você pode:

1. **Via código**: Adicionar um usuário diretamente no banco de dados
2. **Via seed**: Implementar dados iniciais no contexto do Entity Framework

Exemplo de usuário administrador:
- Matrícula: `1`
- Senha: `admin123`
- Perfil: `Admin`

## ⏰ Funcionamento do Sistema de Ponto

### Jornada de Trabalho
O sistema trabalha com 4 batidas diárias:
- **Entrada Manhã**: Início do período matutino
- **Saída Manhã**: Fim do período matutino (intervalo almoço)
- **Entrada Tarde**: Início do período vespertino
- **Saída Tarde**: Fim do período vespertino

### Validações
- **Janela de Tempo**: 15 minutos antes e depois do horário definido
- **Duplicidade**: Não permite registrar a mesma batida duas vezes no mesmo dia
- **Sequência**: Controle lógico da sequência de batidas

### Exemplo de Configuração
```
Entrada Manhã: 08:00
Saída Manhã: 12:00
Entrada Tarde: 13:00
Saída Tarde: 17:00
```

## 📊 Relatórios

O sistema gera relatórios em PDF com:
- Dados completos de todos os funcionários
- Registros organizados por data
- Status de batidas (registradas ou pendentes)
- Layout otimizado para impressão

## 🔧 Configurações Importantes

### Sessões
O sistema utiliza sessões para controle de autenticação. Configure no `Program.cs`:

```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### QuestPDF
Para geração de PDFs, configure a licença community:

```csharp
QuestPDF.Settings.License = LicenseType.Community;
```
---

**Desenvolvido com ❤️ usando ASP.NET Core**
