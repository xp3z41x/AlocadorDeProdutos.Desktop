using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace AlocadorDeProdutos
{
    /// <summary>
    /// Centraliza a montagem da connection string do PostgreSQL do Integra.
    /// Template vem de App.config (connectionStrings/IntegraDb); apenas o Host
    /// e injetado em runtime (e editavel via FormConfig + db_config.txt).
    ///
    /// Permite tambem persistir a senha de volta no App.config quando o
    /// operador a configura pela engrenagem (caso o template venha sem senha).
    /// </summary>
    internal static class DbConfig
    {
        public const string ConnectionStringName = "IntegraDb";

        /// <summary>
        /// Le o template do App.config. Lanca <see cref="InvalidOperationException"/>
        /// se a entrada nao existir. Pode retornar template com <c>Password=;</c> vazio.
        /// </summary>
        public static string LerTemplate()
        {
            var entry = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (entry == null || string.IsNullOrEmpty(entry.ConnectionString))
                throw new InvalidOperationException(
                    "App.config sem entrada <connectionStrings name=\"" + ConnectionStringName + "\">.");
            return entry.ConnectionString;
        }

        /// <summary>
        /// True se o template do App.config tem <c>Password=</c> vazio
        /// (Password=; ou Password= no fim da string). Nao considera ausencia
        /// total da chave (devolveria true tambem, e nao queremos quebrar nesse caso).
        /// </summary>
        public static bool SenhaVazia()
        {
            try
            {
                string template = LerTemplate();
                return ExtrairSenha(template).Length == 0;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Monta a connection string final: <c>Host={host};{template}</c>.
        /// NAO valida se a senha esta vazia — a abertura da conexao falhara
        /// naturalmente, e o usuario pode entao ir na engrenagem para configurar.
        /// </summary>
        public static string BuildConnString(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Host nao pode ser vazio.", nameof(host));

            string template = LerTemplate();
            return "Host=" + host + ";" + template;
        }

        /// <summary>
        /// Atualiza apenas o valor de Password= dentro da connection string
        /// "IntegraDb" no App.config rodando ao lado do .exe e salva o arquivo.
        /// Lanca excecao se nao tiver permissao de escrita (ex.: app sob Program Files
        /// sem privilegio de admin).
        /// </summary>
        public static void SalvarSenha(string novaSenha)
        {
            if (novaSenha == null) novaSenha = string.Empty;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.ConnectionStrings;
            var entry = section.ConnectionStrings[ConnectionStringName];
            if (entry == null)
            {
                entry = new ConnectionStringSettings(
                    ConnectionStringName,
                    "Username=postgres;Password=" + novaSenha + ";Database=integrapgsql;Port=5432;" +
                    "Timeout=5;Command Timeout=5;Keepalive=10;TCP Keepalive=true;" +
                    "SSL Mode=Prefer;Trust Server Certificate=true;",
                    "Npgsql");
                section.ConnectionStrings.Add(entry);
            }
            else
            {
                entry.ConnectionString = SubstituirSenha(entry.ConnectionString, novaSenha);
            }

            config.Save(ConfigurationSaveMode.Modified);

            // Forca o ConfigurationManager a recarregar o arquivo no proximo acesso.
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// Substitui o valor de <c>Password=</c> mantendo o resto do template
        /// inalterado. Se a chave nao existir, anexa.
        /// </summary>
        private static string SubstituirSenha(string template, string novaSenha)
        {
            if (string.IsNullOrEmpty(template))
                return "Password=" + novaSenha + ";";

            // Regex: Password= seguido de qualquer coisa ate ; ou fim da string.
            var rx = new Regex(@"(?i)Password\s*=\s*[^;]*;?");
            if (rx.IsMatch(template))
                return rx.Replace(template, "Password=" + novaSenha + ";", 1);

            // Sem Password= no template: anexa no final.
            return template.TrimEnd(';', ' ') + ";Password=" + novaSenha + ";";
        }

        /// <summary>
        /// Extrai (somente para teste/diagnostico) o valor de Password= do template.
        /// Retorna string vazia se nao houver chave ou valor.
        /// </summary>
        public static string ExtrairSenha(string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var m = Regex.Match(template, @"(?i)Password\s*=\s*([^;]*)");
            if (!m.Success) return string.Empty;
            return m.Groups[1].Value;
        }
    }

    /// <summary>
    /// Helper para persistir a filial configurada em <c>filial.txt</c>
    /// ao lado do executavel. Mesma convencao do <c>db_config.txt</c>.
    /// </summary>
    internal static class FilialConfig
    {
        public const int Default = 1;
        public const int Min = 1;
        public const int Max = 99;

        private static readonly string filialPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "filial.txt");

        /// <summary>
        /// Le a filial. Retorna Default e nao avisa nada (compat com codigo antigo).
        /// Nova chamada preferida: <see cref="TryLer(out int)"/> para diferenciar
        /// "arquivo nao existe" (OK) de "arquivo invalido" (ERRO grave).
        /// </summary>
        public static int Ler()
        {
            TryLer(out int valor);
            return valor;
        }

        /// <summary>
        /// Le a filial diferenciando 3 estados:
        /// - Arquivo nao existe → retorna true (primeira execucao, default OK)
        /// - Arquivo existe e e valido → retorna true com valor parseado
        /// - Arquivo existe mas invalido / sem permissao → retorna **false**
        ///   (operador deve ser avisado para nao gravar alocacao na filial errada).
        /// </summary>
        public static bool TryLer(out int filial)
        {
            filial = Default;

            if (!File.Exists(filialPath))
                return true; // primeira execucao — default e aceitavel

            try
            {
                string conteudo = File.ReadAllText(filialPath).Trim();
                if (int.TryParse(conteudo, out int valor) && valor >= Min && valor <= Max)
                {
                    filial = valor;
                    return true;
                }
                // Arquivo existe mas conteudo invalido: NAO silenciar.
                return false;
            }
            catch
            {
                // IO error / permissao: tambem NAO silenciar.
                return false;
            }
        }

        public static void Salvar(int filial)
        {
            if (filial < Min) filial = Min;
            if (filial > Max) filial = Max;
            File.WriteAllText(filialPath, filial.ToString());
        }
    }
}
