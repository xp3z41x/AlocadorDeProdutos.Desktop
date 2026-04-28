# Alocador de Produtos

Aplicação Windows para atribuir alocações físicas de produtos no estoque do **ERP Integra** (PostgreSQL). Operadores de almoxarifado bipam um código de alocação (ex: `A12-03`), depois bipam os códigos de barras dos produtos com leitor Zebra/USB e a alocação é gravada no banco. Inclui também uma tela de **alocação em massa** para casos como migração de prateleiras ou organização inicial de estoque.

[![.NET](https://img.shields.io/badge/.NET-10.0%20LTS-blueviolet)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2B-blue)](https://www.microsoft.com/windows)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## Funcionalidades

- **Bipe contínuo** — operador trava uma alocação e bipa N produtos seguidos. Cada bipe identifica o produto pelo EAN/GTIN (validação de check-digit nativa) e gera um `UPDATE estsal SET local = …` em background.
- **Cadastro on-the-fly de EAN** — código de barras desconhecido abre uma janela para vincular ao matrícula correta sem sair do fluxo.
- **Busca rápida (F4)** — modal com pesquisa por descrição ou referência caso o operador não tenha o código de barras.
- **Alocação em massa** — cabeçalho com botão dedicado abre uma tela de pesquisa com grid + checkboxes (incluindo Shift+click para range), Ctrl+A, contador de selecionados entre buscas. Confirma substituições com lista do que será sobrescrito.
- **Fila write-ahead** — todas as gravações no banco passam por uma fila persistente em disco (`fila_pendente.txt`). Kill abrupto, queda de energia ou perda de rede não perdem operações: na próxima execução a fila é replayed (UPDATE idempotente).
- **Health-check de conexão** — probe `SELECT 1` a cada 10s; se a rede cai, beep + indicador visual + reconexão automática quando volta. Operador continua bipando offline e os UPDATEs drenam quando a conexão restabelece.
- **Multi-filial** — filtro `AND estsal.filial = @filial` em todas as queries de estoque. Cada matrícula tem 1 linha por filial em `estsal`; sem o filtro, um UPDATE bagunçaria todas as filiais.
- **Configuração protegida** — instalação per-machine em Program Files exige UAC. Senha do banco persistida em `App.config` ao lado do `.exe`. Apenas administradores conseguem alterar IP/senha/filial via engrenagem.

## Tecnologia

| Camada | Stack |
|--------|-------|
| Runtime | **.NET 10 LTS** (Windows Desktop, self-contained no instalador) |
| UI | **Windows Forms** (csproj SDK-style, source-generator `ApplicationConfiguration.Initialize`) |
| Banco | **PostgreSQL 13** via **Npgsql 9.0.x** (ADO.NET puro, sem ORM) |
| Build | `dotnet build` / `dotnet publish` (sem dependência de Visual Studio) |
| Empacotamento | **Inno Setup 6** (single .exe ~35 MB) |
| Linguagem | C# `latest` |

## Tabelas do ERP usadas

| Tabela | Colunas | Papel |
|--------|---------|-------|
| `cadpro` | `matricula` (PK), `descricao`, `referencia`, `marca`, `codigo_barra` | Cadastro global de produtos |
| `estsal` | `matricula`, `filial` (smallint), `local` (varchar) | Estoque por filial — `local` é o campo da alocação |
| `formar` | `marca`, `descricao` | Cadastro global de marcas |

## Instalação

Baixe o `AlocadorDeProdutos-Setup-11.0.0.0.exe` mais recente em [Releases](../../releases) e execute como administrador.

- Instala em `C:\Program Files\Alocador de Produtos\`
- Cria atalhos no Menu Iniciar (Desktop opcional)
- Self-contained: **não requer instalar .NET Runtime** previamente

Após a primeira execução, a engrenagem (⚙) abre automaticamente para configuração inicial:
1. **IP do servidor** (PostgreSQL do Integra)
2. **Filial** (1-99)
3. **Senha do banco** (ficará em `Alocador de Produtos.exe.config` ao lado do .exe)

## Build a partir do código

Pré-requisitos:
- Windows 10 ou 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- (Opcional, para gerar instalador) [Inno Setup 6](https://jrsoftware.org/isinfo.php)

### Build de desenvolvimento

```bash
git clone https://github.com/xp3z41x/AlocadorDeProdutos.Desktop.git
cd AlocadorDeProdutos.Desktop
dotnet build -c Debug
# saída: bin/Debug/net10.0-windows/Alocador de Produtos.exe
```

No VSCode com a extensão C#: `F5` para debug, `Ctrl+F5` para rodar sem debugger.

### Build de produção (publish)

```bash
dotnet publish -p:PublishProfile=win-x64
# saída: bin/Release/net10.0-windows/publish/win-x64/  (self-contained, ~119 MB)
```

### Gerar o instalador

Após o publish acima:

```bash
"C:\Users\<user>\AppData\Local\Programs\Inno Setup 6\ISCC.exe" "Setup\AlocadorDeProdutos.iss"
# saída: Setup/Output/AlocadorDeProdutos-Setup-11.0.0.0.exe  (~35 MB)
```

Tasks equivalentes no VSCode: `publish-for-inno-setup`, depois compilar manualmente o `.iss`.

## Estrutura do projeto

```
AlocadorDeProdutos.csproj    # csproj SDK-style (.NET 10, WinForms)
AlocadorDeProdutos.sln
App.config                    # template de connection string PostgreSQL (Password vazio)
LICENSE                       # MIT
app.manifest                  # DPI awareness PerMonitorV2 + admin (asInvoker)

Program.cs                    # ApplicationConfiguration.Initialize + Run(FormMain)
DbConfig.cs                   # helpers para App.config + filial.txt

FormMain.{cs,Designer.cs,resx}    # tela principal (bipe + fila + health-check)
FormConfig.{cs,Designer.cs}       # engrenagem (IP/filial/senha)
FormBuscaProduto.{cs,Designer.cs} # busca por descrição (F4)
FormCadastroBarras.{cs,Designer.cs} # cadastro de EAN desconhecido
FormAlocacaoMassa.{cs,Designer.cs}  # alocação em massa

Properties/PublishProfiles/win-x64.pubxml  # publish self-contained
Resources/                    # ícone + logo
Setup/AlocadorDeProdutos.iss  # script Inno Setup
.vscode/                      # tasks de build/publish/clean
.editorconfig                 # padrões de formatação
.gitattributes                # line endings consistentes (CRLF)
CLAUDE.md                     # documentação técnica detalhada (arquitetura, SQL, threading)
```

## Documentação técnica

Para detalhes de arquitetura, SQL completo, padrão write-ahead da fila, race conditions tratadas, fluxo de UI, política de privilégios e prontidão para Inno Setup, consulte **[CLAUDE.md](CLAUDE.md)**.

## Licença

[MIT](LICENSE) — Copyright (c) 2024-2026 Bernardo Graunke.
