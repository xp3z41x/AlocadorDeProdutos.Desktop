# Diretrizes do Projeto — Alocador de Produtos (Desktop)

Aplicação **WinForms / .NET 10 (LTS)** que integra com o ERP Integra (PostgreSQL) para atribuir alocações físicas de produtos no estoque. Roda em PCs Windows. Este projeto é **independente** e não compartilha código nem build com nenhuma outra versão da aplicação.

## Identidade do produto

| Campo | Valor |
|-------|-------|
| AssemblyName | `Alocador de Produtos` (com espaços) |
| Title / FileDescription | `Alocador de Produtos` |
| Description | `Alocação de produtos no estoque do ERP Integra` |
| Version / FileVersion / AssemblyVersion | `11.0.0.0` |
| Authors / Company | `Bernardo Graunke` |
| Copyright | `Copyright (c) 2024-2026 Bernardo Graunke — MIT License` |
| Licença | **MIT** (vide `LICENSE` na raiz) |
| RootNamespace | `AlocadorDeProdutos` |
| **AppId Inno Setup** | `{F7CBB36B-C8BB-455C-ADB0-21AB353C384D}` ← **NÃO MUDAR** entre versões (identifica unicamente a aplicação para upgrades automáticos) |

## Build e Deploy

- Solution: `AlocadorDeProdutos.sln` / projeto: `AlocadorDeProdutos.csproj` (SDK-style, `Microsoft.NET.Sdk`)
- Target: `net10.0-windows`, `OutputType=WinExe`, `UseWindowsForms=true`, `AnyCPU`
- Saída de Debug: `bin/Debug/net10.0-windows/Alocador de Produtos.exe`
- Saída de Release: `bin/Release/net10.0-windows/Alocador de Produtos.exe`
- Build via CLI (recomendado): `dotnet build -c Release` ou `dotnet build -c Debug`
- Restore de pacotes: automático ao buildar; força via `dotnet restore`
- Publicação self-contained (sem precisar de runtime instalado no alvo): `dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true`
- Publicação framework-dependent (mais leve, requer .NET 10 Desktop Runtime no alvo): `dotnet publish -c Release -r win-x64 --self-contained=false`
- Pré-requisitos no host de build: **.NET 10 SDK** (`dotnet --list-sdks` deve mostrar 10.x). Não há mais necessidade de Visual Studio ou Targeting Pack do .NET Framework.
- Não gerar builds automaticamente — apenas quando o usuário solicitar

## Diretrizes de Git

- **NÃO fazer push/upload para GitHub** sem solicitação explícita do usuário em cada caso (diretriz máxima).
- Commits podem ser feitos localmente quando solicitados; push é sempre manual e explícito.
- **State local do operador NÃO vai para o repo**: `db_config.txt`, `filial.txt`, `fila_pendente.txt`, `Alocador de Produtos.exe.config` (preenchido com senha) ficam no `.gitignore`.

## Hardening aplicado (code review 2026-04)

Após auditoria de 5 agentes Explore (SQL, Threading, UI, Config/Segurança, Build), foram aplicadas **18 correções** em 4 fases. Pontos relevantes para futuras manutenções:

- **Fila write-ahead atômica**: `Enqueue` + `SalvarFilaPendente` ficam dentro do mesmo `lock(filaLock)`. Versão `SalvarFilaPendenteUnsafe` (sem lock) é chamada por código que já detém o lock; `SalvarFilaPendente` público adquire o lock antes de delegar.
- **`MostrarNaUI` deadlock-safe**: usa `BeginInvoke` (assíncrono) + check `appFechando`. Não trava em `FormClosing.Join`.
- **Worker totalmente cancelável**: todos os `Thread.Sleep` foram trocados por `sinalFila.WaitOne(timeout)` + `if (appFechando) return`. Fechamento responde em < 1s mesmo com worker em backoff.
- **`File.Move` atômico**: usa overload `overwrite: true` (NTFS atômico) — sem janela onde `fila_pendente.txt` não existe.
- **Filial diferenciada**: `FilialConfig.TryLer(out int)` distingue "arquivo não existe" (default OK) de "arquivo corrompido" (warning + abre engrenagem). `Ler()` é wrapper compat.
- **Senha re-mascara em `Leave`**: mitiga shoulder-surfing. Re-mascaramento garantido mesmo se o operador trocar de campo com `chkMostrarSenha` ativo.
- **UAC/permissão tratado especificamente** em `FormConfig.TentarSalvarSenha`: mensagens distintas para `UnauthorizedAccessException`, `ConfigurationErrorsException`, `IOException`.
- **Fresh-install / falha de conexão na primeira execução**: `FormMain_Load` abre `FormConfig` automaticamente via `BeginInvoke` quando detecta primeira execução, senha vazia, ou filial inválida.
- **Disposes corretos** em todos os Forms (FormMain, FormConfig, FormBuscaProduto, FormAlocacaoMassa): handlers são desinscritos com `-=`, fontes (`Font`) são liberadas. Modais reabertos múltiplas vezes na sessão não vazam GDI handles.
- **Debounce de 300ms** em `nMatricula_TextChanged`: cada keystroke restarta o timer; query só dispara quando o operador para. Reduz queries em ~10x.
- **Build determinístico**: `<Deterministic>true</Deterministic>` + `<DebugType>portable</DebugType>` + `.editorconfig` + `.gitattributes` (line endings consistentes CRLF).
- **Ícone**: `Resources/Icon.ico` (sem caractere acentuado, evita problemas de cross-locale build).
- **Dead code removido**: `pVolume`, `pPedido`.

## Banco de Dados

- PostgreSQL 13 (porta 5432); IP do servidor configurável em runtime via engrenagem (gravado em `db_config.txt`)
- Database: integrapgsql
- **SSL Mode=Prefer (obrigatório)**
- Filial **default = 1** (configurável 1-99 via engrenagem; persistida em `filial.txt`)
- Senha: vive em **App.config** (`<connectionStrings name="IntegraDb">`), não no código
- Driver: **Npgsql 9.0.x** via `<PackageReference>` (sem `packages.config`/`packages/` — config legacy foi totalmente removida no upgrade para .NET 10)

## Camada de Banco — Desktop (como funciona)

### Driver e arquitetura
- **Driver:** Npgsql 9.0.x (ADO.NET puro). Sem ORM, sem Dapper, sem repositório.
- SQL é **inline em cada Form**, sempre via `NpgsqlCommand` parametrizado (`AddWithValue`) — sem risco de SQL injection.
- **Sem `async/await`** em DB: `ExecuteReader()`, `ExecuteNonQuery()`, `ExecuteScalar()` síncronos. Quem isola a UI das gravações é uma thread dedicada (não é Task). Migrar para o caminho async é trivial em .NET 10 se for necessário no futuro.
- Configuração centralizada em [DbConfig.cs](X:\AlocadorDeProdutos.Desktop\DbConfig.cs) — duas classes estáticas: `DbConfig` (template, build, persistência da senha) e `FilialConfig` (leitura/escrita do `filial.txt`). Usa `System.Configuration.ConfigurationManager` que vem nativo no shared framework de `net10.0-windows` (não é mais NuGet separado).

### Origem do host (IP do servidor)
- Lido de `db_config.txt` ao lado do `.exe` (`AppDomain.CurrentDomain.BaseDirectory`) — método `LerHost()` em [FormMain.cs](X:\AlocadorDeProdutos.Desktop\FormMain.cs).
- Se o arquivo não existir, é criado com default `127.0.0.1`.
- Editável em runtime via `FormConfig` (engrenagem) → grava de volta em `db_config.txt`.

### Origem da filial
- Lida de `filial.txt` ao lado do `.exe` via `FilialConfig.Ler()`. Default `1` se ausente ou inválido.
- Editável em runtime via `FormConfig` (NumericUpDown 1-99) → `FilialConfig.Salvar()`.
- Validação: 1-99 inclusive (clamp em ambos os lados).

### Origem da connection string (template)
- **App.config** (`<connectionStrings name="IntegraDb">`):
  ```xml
  <add name="IntegraDb"
       connectionString="Username=postgres;Password=;Database=integrapgsql;Port=5432;
                         Timeout=5;Command Timeout=5;Keepalive=10;TCP Keepalive=true;
                         SSL Mode=Prefer;Trust Server Certificate=true;"
       providerName="Npgsql" />
  ```
- `Host=` é **injetado em runtime** por `DbConfig.BuildConnString(host)` — não fica no template.
- `Password=` deliberadamente vazio na distribuição. O administrador preenche **uma de duas formas**:
  1. Editando `Alocador de Produtos.exe.config` ao lado do .exe.
  2. **Pela engrenagem do app**: campo "SENHA DO BANCO" no `FormConfig` → `DbConfig.SalvarSenha()` faz o write atômico no `.exe.config` via `ConfigurationManager.OpenExeConfiguration()`.
- Quando a senha está vazia, o app **abre normalmente** mas mostra "SENHA NAO CONFIGURADA — Abra a engrenagem" no `lblStatus`/`pDescricao`. O operador vai na engrenagem, preenche, salva.
- Se for necessário rotacionar a senha, basta abrir a engrenagem e cadastrar a nova — não precisa redeploy.

### Duas conexões persistentes em paralelo (em `FormMain`)
| Campo | Uso | Thread |
|-------|-----|--------|
| `conn`     | Todos os SELECTs da UI (lookup de produto, marca, busca, cadastro de barras) | UI thread |
| `connFila` | Apenas o `UPDATE estsal` da fila | Thread `FilaAlocacao` (background) |
- São abertas separadamente em `Conectar()`/`ConectarFila()` e fechadas em `FormClosing`. A separação evita contenção entre leitura interativa e gravação em segundo plano.
- `FormBuscaProduto` e `FormCadastroBarras` **reusam a `conn` da UI** que é passada no construtor — eles não abrem conexão própria.
- `FormConfig` abre **conexão temporária** (curta) só para teste, usando o mesmo `DbConfig.BuildConnString(host)`.

### Tabelas usadas (Integra ERP)
| Tabela  | Colunas referenciadas | PK / unique | Papel |
|---------|------------------------|-------------|-------|
| `cadpro` | `matricula` (int), `descricao`, `referencia`, `marca` (FK), `codigo_barra` | `matricula` | Cadastro global de produtos |
| `estsal` | `matricula`, **`filial` (smallint)**, `local` (varchar) | **(`matricula`, `filial`)** | Estoque por filial — `local` é a alocação |
| `formar` | `marca`, `descricao` | `marca` | Cadastro global de marcas |

> **Importante**: cada `matricula` em `estsal` tem **uma linha por filial** (no banco de dev existem 9 filiais ativas: 1, 3, 4, 5, 6, 7, 8, 9, 20). Por isso o filtro por filial em todos os SELECTs e no UPDATE é **obrigatório** — sem ele, o UPDATE da alocação afetaria todas as 9 linhas da matrícula.

### Inventário completo de operações SQL

#### Leituras (SELECT)
1. **Probe de conexão** — `SELECT 1` (timeout 3s) para `ConexaoAtiva()`. Roda no health-check de 10s e antes de cada operação via `GarantirConexao()`.
2. **Lookup por matrícula digitada** — disparado em `nMatricula_TextChanged`:
   ```sql
   SELECT cadpro.descricao, cadpro.referencia, cadpro.marca, estsal.local
   FROM cadpro
   LEFT JOIN estsal ON cadpro.matricula = estsal.matricula
                    AND estsal.filial = @filial
   WHERE cadpro.matricula = @matricula;
   ```
   `estsal.filial` está no **ON** (não no WHERE) para preservar o LEFT — produto sem estoque na filial atual ainda aparece, com `local = NULL`.
3. **Lookup de marca** (sem filial — `formar` é global):
   ```sql
   SELECT descricao FROM formar WHERE marca = @marca
   ```
4. **Lookup por código de barras (bipe)**:
   ```sql
   SELECT cadpro.matricula, cadpro.descricao, cadpro.referencia, cadpro.marca, estsal.local
   FROM cadpro
   JOIN estsal ON cadpro.matricula = estsal.matricula
              AND estsal.filial = @filial
   WHERE cadpro.codigo_barra = @codigo_barra
   ```
   `JOIN` (não LEFT) — produto sem `estsal` na filial atual cai no fluxo de cadastro.
5. **Re-busca após cadastro de EAN novo** — mesma query da (4) mas filtrando por `cadpro.matricula = @matricula`.
6. **Busca de produto (F4 / FormBuscaProduto)** — sem filial, busca apenas em `cadpro` + `formar`:
   ```sql
   SELECT cadpro.descricao, cadpro.referencia, cadpro.matricula, formar.descricao AS marca
   FROM cadpro
   LEFT JOIN formar ON cadpro.marca = formar.marca
   WHERE 1=1
     [AND cadpro.descricao ILIKE @descricao]   -- prefixo + %
     [AND cadpro.referencia ILIKE @referencia]
   ORDER BY cadpro.descricao
   LIMIT 200
   ```
   `ILIKE prefixo%`. Live search em `TextChanged`. LIMIT 200.
7. **Verificação de matrícula no cadastro de EAN** (sem filial — `cadpro` é global):
   ```sql
   SELECT cadpro.matricula, cadpro.descricao FROM cadpro WHERE cadpro.matricula = @matricula
   ```
8. **Busca para alocação em massa** ([FormAlocacaoMassa.cs](X:\AlocadorDeProdutos.Desktop\FormAlocacaoMassa.cs)) — duas barras combináveis (prefixo + contém):
   ```sql
   SELECT cadpro.matricula, cadpro.descricao, cadpro.referencia, estsal.local
   FROM cadpro
   JOIN estsal ON cadpro.matricula = estsal.matricula
               AND estsal.filial = @filial
   WHERE 1=1
     [AND cadpro.descricao ILIKE @prefixo]   -- 'termo%'
     [AND cadpro.descricao ILIKE @contem]    -- '%termo%'
   ORDER BY cadpro.descricao
   LIMIT 500
   ```
   INNER JOIN com filial (mesma do bipe). LIMIT 500. Live search nas duas barras (TextChanged).

#### Escritas (UPDATE)
Apenas **dois** UPDATEs no app inteiro — não há nenhum INSERT nem DELETE.

1. **Gravação da alocação (via fila, assíncrono)**:
   ```sql
   UPDATE estsal SET local = @local
   WHERE matricula = @matricula AND filial = @filial
   ```
   - Roda na thread `FilaAlocacao`, nunca na UI.
   - `CommandTimeout = 10s`.
   - **Idempotente** — chave para o write-ahead replay (ver Tarefa 4 abaixo).
   - Se `rows == 0` (matrícula não existe na filial) → marca como sucesso para remover da fila e mostra erro vermelho ("ERRO FILA: Mat. X sem estoque na filial Y").
   - Se exceção → backoff `Thread.Sleep(2000 * tentativas)`, até **5 tentativas**.
   - Se as 5 falharem → item **permanece na fila** e no arquivo, mensagem "FILA: falha de gravacao", `Thread.Sleep(5000)` e re-entra no loop.

2. **Cadastro de novo EAN num produto (síncrono, UI)** — `cadpro` é global, sem filial:
   ```sql
   UPDATE cadpro SET codigo_barra = @codigo_barra WHERE matricula = @matricula
   ```
   Síncrono **de propósito**: o pipeline do bipe precisa do retorno antes de enfileirar a alocação. Não passa pela fila de retry.

### Padrão de fila write-ahead (resiliência) — o destaque do design

- **Estrutura:** `ConcurrentQueue<AlocacaoPendente>` `filaOperacoes` (produtor: UI; consumidor: thread `FilaAlocacao`).
- **Sinalização:** `AutoResetEvent` `sinalFila` acorda a thread imediatamente; senão ela acorda a cada 2s (`WaitOne(2000)`).
- **Struct:**
  ```csharp
  private struct AlocacaoPendente {
      public int Matricula;
      public string Alocacao;
      public string Descricao;
      public int Filial;     // filial em que a alocação deve ser gravada
  }
  ```
- **Lock:** `object filaLock` protege toda escrita/leitura de `fila_pendente.txt`.

#### Padrão write-ahead (peek-then-dequeue)

1. **Enqueue (UI)** — `EnfileirarAlocacao`:
   - `filaOperacoes.Enqueue(...)`
   - `SalvarFilaPendente()` síncrono (sob `filaLock`, escrita atômica `tmp + rename`)
   - `sinalFila.Set()` para acordar o worker
2. **Worker** — `ProcessarFila`:
   - `TryPeek` (não remove)
   - Tenta `UPDATE estsal SET local=... WHERE matricula=... AND filial=...` (até 5 tentativas com backoff)
   - **Apenas se confirmou pelo banco**: `TryDequeue` + `SalvarFilaPendente()` (regrava o arquivo sem este item)
   - Se 5 falhas: item permanece na fila e no arquivo; sleep 5s; próxima rodada
3. **Startup** — `CarregarFilaPendente`:
   - Lê `fila_pendente.txt`, re-injeta itens na fila em memória
   - **NÃO deleta o arquivo** (o worker mantém sincronizado a cada UPDATE)
   - Se o arquivo veio em formato legado (3 partes, sem filial), re-grava em formato novo

#### Formato do arquivo `fila_pendente.txt`
- Cada linha: `matricula|alocacao|filial|descricao` (4 campos separados por `|`)
- Caracteres `|` na descrição são substituídos por `/` ao salvar (sanitização do separador)
- Compat retroativo: linhas com 3 partes (formato legado, sem filial) são lidas com filial = atual configurada

#### Garantias após o fix de kill abrupto
| Cenário | Comportamento |
|---------|---------------|
| Crash entre UI Enqueue e o write síncrono em `EnfileirarAlocacao` | Janela de microsegundos; ainda existe risco mínimo. Mitigado pela proximidade física das duas instruções. |
| **Crash durante o UPDATE no banco** | Item permanece em `fila_pendente.txt` (peek, não dequeue) → no próximo startup é re-injetado e o UPDATE roda de novo. **Idempotente** (`UPDATE estsal SET local=X WHERE matricula=Y AND filial=Z` — testado: 3 execuções consecutivas produzem o mesmo resultado). |
| Crash entre UPDATE bem-sucedido e o `SalvarFilaPendente` final | Replay re-executa o UPDATE (idempotente, sem dano) e regrava o arquivo. |
| Crash durante o write atômico do arquivo | `tmp + rename`: ou o arquivo antigo está intacto, ou o novo está completo. |
| Fechamento normal pelo X | `FormClosing` espera `threadFila.Join(15000)` para drenar; chama `SalvarFilaPendente()` final por defesa. |

### Health-check e reconexão
- `System.Windows.Forms.Timer` de **10s**.
- Cada tick: `ConexaoAtiva()` (probe `SELECT 1`); se caiu, beep + tenta `Conectar()`. Se voltou, mostra "Conexao restabelecida!" e dá `sinalFila.Set()` para drenar a fila imediatamente.
- **Pausado** (`PausarHealthCheck()`) durante diálogos modais (`FormConfig`, `FormBuscaProduto`, `FormCadastroBarras`, `FormAlocacaoMassa`) para não disparar reconexões durante input do usuário.

### Tratamento de erros de conexão
- `TratarErroConexao(Exception)` classifica `NpgsqlException`, `SocketException`, `TimeoutException` (incluindo `InnerException`) como falha de rede → tenta reconectar uma vez e atualiza o `lblStatus`.
- Se a reconexão falhar e a senha estiver vazia, mostra "SENHA NAO CONFIGURADA! Abra a engrenagem." em vez de "SEM CONEXAO".
- Outros erros mostram a mensagem crua em vermelho.

### Garantias por operação
| Operação | Síncrona/Assíncrona | Retry | Persistência | Conexão |
|----------|---------------------|-------|--------------|---------|
| SELECTs da UI | Síncrona | Não (mostra erro) | Não | `conn` |
| Probe `SELECT 1` | Síncrona | Não | Não | qualquer |
| `UPDATE estsal` (alocação) | **Assíncrona via fila** | **5x com backoff + replay no startup** | **`fila_pendente.txt` write-ahead** | `connFila` |
| `UPDATE cadpro SET codigo_barra` | Síncrona | Não | Não | `conn` (passada do FormMain) |

### Onde tocar quando…
- **Adicionar nova consulta no bipe:** `txtBipe_KeyDown` em [FormMain.cs](X:\AlocadorDeProdutos.Desktop\FormMain.cs).
- **Adicionar nova gravação resiliente:** criar nova struct, novo método tipo `EnfileirarX`, e ramificar dentro do loop de `ProcessarFila`. A persistência (`SalvarFilaPendente`/`CarregarFilaPendente`) precisa ser estendida se for outro tipo de op.
- **Mudar timeouts globais:** campo `comandoTimeout` em [FormMain.cs](X:\AlocadorDeProdutos.Desktop\FormMain.cs) (afeta `CriarComando`).
- **Mudar SSL/credencial/connection string:** **só** em [App.config](X:\AlocadorDeProdutos.Desktop\App.config) (`IntegraDb`). O código não tem nada hardcoded — `DbConfig.BuildConnString` injeta apenas o Host.
- **Mudar filial fora da UI:** editar `filial.txt` ao lado do `.exe` (uma linha, número 1-99).
- **Adicionar nova tabela com filial:** SQLs precisam de `AND tabela.filial = @filial` (no `ON` se for LEFT JOIN para preservar; no `WHERE` ou `ON` em INNER JOIN). O parâmetro é `(short)filial`.

## Tela de Alocação em Massa (`FormAlocacaoMassa`)

Janela modal aberta pelo botão **"📦 ALOCAÇÃO EM MASSA"** no novo `panelHeader` (Dock.Top, h=50) do `FormMain`. Permite atribuir uma mesma alocação a dezenas/centenas de produtos selecionados via grid.

### Componentes
- `txtAlocacao` — alocação alvo, **independente** do `nAlocacao` da tela principal (max 10, letra+dígito+hífen, uppercase)
- `lblFilial` — readonly, mostra a filial atual configurada (não pode ser mudada daqui)
- `txtBuscaPrefixo` (`ILIKE 'termo%'`) e `txtBuscaContem` (`ILIKE '%termo%'`) — combináveis (AND)
- `dgvProdutos` — DataGridView com 5 colunas: checkbox, matrícula, descrição, referência, **alocação atual** (com cor condicional: branco=NEW, verde=OK no-op, laranja=SUBSTITUI)
- `btnMarcarTodos` / `btnDesmarcarTodos` — afetam apenas linhas **visíveis** (resultado da busca corrente)
- `lblContador` — "Selecionados: N" (N atravessa todas as buscas)
- `btnAlocar` — texto dinâmico "ALOCAR (N)", desabilitado quando inválido ou N=0

### Estado interno
- `Dictionary<int, ProdutoSelecionado> selecionados` — preserva seleção entre buscas: marcar 3 em "PARAFUSO", trocar para "PORCA", marcar 2 → contador=5, ALOCAR atualiza os 5.
- `int? lastCheckedRowIndex` — anchor para Shift+click range selection.

### Atalhos
- `Ctrl+A` — Marcar Todos (visíveis)
- `Esc` — Cancelar
- `Shift+click` em checkbox — seleciona/deseleciona faixa entre o último clique simples e a posição atual (estado-alvo = estado da célula clicada com Shift)

### Fluxo do botão ALOCAR
1. Valida `txtAlocacao` (regras iguais às do `nAlocacao` no FormMain).
2. Particiona seleção em **novos** (sem `local`), **mantém** (`local == nova`), **substitui** (`local != '' && != nova`).
3. Se houver substituições → `MessageBox.Warning` listando até 10 (Mat + alocação atual) + Yes/No. Default = No (`MessageBoxDefaultButton.Button2`).
4. Confirmado → invoca callback `FormMain.EnfileirarLote(IEnumerable<Tuple<int,string,string>>)` que faz `Enqueue` em loop + **uma única** `SalvarFilaPendente()` atômica + `sinalFila.Set()` + `AtualizarContadorFila()`.
5. Worker drena 1-a-1 (mesmo padrão write-ahead, idempotente, resiliente a kill abrupto).

### Caminho de código
```
FormMain.btnAlocacaoMassa_Click
  ↓ PausarHealthCheck + ShowDialog
FormAlocacaoMassa (modal)
  ↓ usuário busca, marca, clica ALOCAR
FormMain.EnfileirarLote(itens)
  ↓ Enqueue x N + SalvarFilaPendente() x 1
ProcessarFila (thread FilaAlocacao)
  ↓ TryPeek → UPDATE estsal SET local=... WHERE matricula=... AND filial=...
  ↓ TryDequeue + SalvarFilaPendente após cada sucesso
```

### Performance / limites
- Grid limita a 500 linhas (`LIMIT 500` no SQL); aviso laranja se atingir o teto.
- Worker processa ~10-30 UPDATEs/s em LAN; lote de 200 itens ≈ 10s para drenar.
- Para lotes maiores que algumas centenas, considerar futura otimização: agrupamento em transação `WHERE filial = @f AND matricula IN (...)` no worker.

---

## Prontidão para Inno Setup

O projeto está estruturado para empacotamento via Inno Setup quando solicitado. Resumo do que já está pronto:

### Constantes para o `.iss`

| Diretiva Inno | Valor |
|---------------|-------|
| `AppId` | `{F7CBB36B-C8BB-455C-ADB0-21AB353C384D}` (vide tabela "Identidade do produto" no topo deste documento) |
| `AppName` | `Alocador de Produtos` |
| `AppVersion` | `11.0.0.0` |
| `AppPublisher` | `Bernardo Graunke` |
| `AppCopyright` | `Copyright (c) 2024-2026 Bernardo Graunke — MIT License` |
| `AppComments` | `Alocação de produtos no estoque do ERP Integra` |
| `LicenseFile` | `LICENSE` (raiz do projeto) |
| `DefaultDirName` | `{autopf}\Alocador de Produtos` (sugestão) |
| `DefaultGroupName` | `Alocador de Produtos` (sugestão) |
| `OutputBaseFilename` | `AlocadorDeProdutos-Setup-11.0.0` (sugestão) |
| `SetupIconFile` | `Resources\Icon.ico` |
| `Compression` | `lzma2/ultra64` (recomendado) |
| `SolidCompression` | `yes` |
| `WizardStyle` | `modern` |
| `ArchitecturesAllowed` | `x64compatible` |
| `ArchitecturesInstallIn64BitMode` | `x64compatible` |

### Build de produção para o instalador

Comando (ou task `publish-for-inno-setup` no VSCode):
```bash
dotnet publish -p:PublishProfile=win-x64
```

Saída em `bin/Release/net10.0-windows/publish/win-x64/`:
- `Alocador de Produtos.exe` (apphost)
- `Alocador de Produtos.dll` + dependências
- `Alocador de Produtos.dll.config` (template de connection string com `Password=` vazia)
- Runtime do .NET 10 (self-contained — operador alvo NÃO precisa instalar runtime)
- Total ~70 MB

O `.iss` empacota a pasta inteira via `[Files]`:
```iss
[Files]
Source: "bin\Release\net10.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
```

### Arquivos que o operador cria/edita em runtime (NÃO empacotar no instalador)

| Arquivo | Propósito | Local |
|---------|-----------|-------|
| `db_config.txt` | IP do servidor PostgreSQL | ao lado do .exe |
| `filial.txt` | Filial atual (1-99) | ao lado do .exe |
| `fila_pendente.txt` | Fila write-ahead | ao lado do .exe |
| `Alocador de Produtos.exe.config` | Senha do banco persistida via engrenagem | ao lado do .exe |

Esses arquivos são todos cobertos pelo `.gitignore` e devem ser ignorados pelo `.iss` também (ou listados em `[InstallDelete]` apenas no caso de upgrade pra um caminho de install diferente).

### Permissões

`PrivilegesRequired=lowest` (não precisa admin para a aplicação rodar). Mas se `DefaultDirName={autopf}` (Program Files), instalação requer admin uma única vez. Recomendação:
- **Single user**: `DefaultDirName={userpf}\Alocador de Produtos` + `PrivilegesRequired=lowest` — instalador sem UAC, senha gravável pelo operador sem privilégio elevado.
- **Multi user / corporate**: `DefaultDirName={autopf}\Alocador de Produtos` + `PrivilegesRequired=admin` — operador NÃO consegue gravar senha em runtime via engrenagem (UAC bloqueia). Nesse caso, configurar a senha **manualmente** em `Alocador de Produtos.exe.config` durante a instalação ou após (com Notepad em Admin).

### Atalhos sugeridos para o `.iss`

```iss
[Icons]
Name: "{group}\Alocador de Produtos"; Filename: "{app}\Alocador de Produtos.exe"
Name: "{commondesktop}\Alocador de Produtos"; Filename: "{app}\Alocador de Produtos.exe"; Tasks: desktopicon
```

### Como compilar o instalador (`.iss` já existe)

O script Inno Setup está em [`Setup/AlocadorDeProdutos.iss`](Setup/AlocadorDeProdutos.iss). Já vem configurado com:
- AppId estável `{F7CBB36B-C8BB-455C-ADB0-21AB353C384D}` (não mudar entre versões — controla upgrade in-place)
- Instalação em `{autopf}\Alocador de Produtos` (Program Files x64)
- `PrivilegesRequired=admin` (instalação E mudanças de config exigem UAC — intencional para isolar credencial do banco)
- Idiomas: Português Brasileiro + Inglês
- Compactação `lzma2/ultra64` (reduz ~119 MB de publish para ~35 MB de instalador)
- `[UninstallDelete]` limpa também os arquivos de state do operador (`db_config.txt`, `filial.txt`, `fila_pendente.txt*`)

**Passos para gerar o instalador:**

```bash
# 1. Publish self-contained (saída em bin/Release/net10.0-windows/publish/win-x64/)
dotnet publish AlocadorDeProdutos.csproj -p:PublishProfile=win-x64
# OU via task no VSCode: "publish-for-inno-setup"

# 2. Compilar o .iss
"C:\Users\<user>\AppData\Local\Programs\Inno Setup 6\ISCC.exe" "Setup\AlocadorDeProdutos.iss"
```

**Saída:** `Setup/Output/AlocadorDeProdutos-Setup-11.0.0.0.exe` (~35 MB).

A pasta `Setup/Output/` está no `.gitignore` — instaladores não vão para o repo.

### Política de privilégios (intencional)

O instalador é **per-machine** (Program Files), exige UAC para instalar e desinstalar. A consequência prática:

- **Operador comum**: pode rodar o `.exe`, bipar produtos, abrir engrenagem para ver IP/filial — mas **não consegue salvar mudanças** (UAC bloqueia gravação no `.exe.config`).
- **Administrador**: configura senha do banco, IP do servidor e filial. Faz o setup inicial uma vez.
- **Mudanças de config em runtime**: requerem que o app seja reaberto **com botão direito → Executar como administrador** ou que o admin se logue na máquina.

Isto é **por design** — credencial do banco é centralizada, operador não pode reapontar para outro servidor inadvertidamente.

Para mudar essa política no futuro:
- Trocar `DefaultDirName` para `{localappdata}\Programs\Alocador de Produtos` ou `{userpf}\...`
- Trocar `PrivilegesRequired` para `lowest`
- Recompilar o `.iss`
