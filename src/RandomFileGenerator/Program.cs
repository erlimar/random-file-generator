using System.Globalization;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static McMaster.Extensions.CommandLineUtils.CommandLineApplication;

namespace RandomFileGenerator
{
    [Command(Description = "Gerador de arquivos aleatórios")]
    public class Program
    {
        private readonly List<String> _suffixList = new List<string> { "b", "KB", "MB", "GB" };

        [Required, Option("-c|--count", Description = "Quantidade de arquivos a gerar")]
        public int FileCount { get; set; }

        [Required, Option("-n|--min-file-size", Description = "Tamanho mínimo dos arquivos gerados (em bytes. Ou sufixado com b, KB, MB, GB. Ex: 5KB)")]
        public string MinFileSize { get; set; }

        [Required, Option("-x|--max-file-size", Description = "Tamanho máximo dos arquivos gerados (em bytes. Ou sufixado com b, KB, MB, GB. Ex: 5KB)")]
        public string MaxFileSize { get; set; }

        [Option("-o|--output-path", Description = "Caminho onde os arquivos aleatórios serão gravados. O padrão é o diretório local.")]
        public string OutputPath { get; set; } = "./";

        [Option("-t|--file-name-template", Description = "Template do nome do arquivo. Use '{n}' para o número do arquivo. O padrão é 'file-{n}.bin'.")]
        public string FileNameTemplate { get; set; } = "file-{n}.bin";

        private IConsole Console { get; set; }

        public Program(IConsole console)
        {
            Console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public static async Task<int> Main(string[] args) => await ExecuteAsync<Program>(args);

        async Task OnExecuteAsync()
        {
            OutputPath = Path.GetFullPath(OutputPath);

            if (File.Exists(OutputPath))
                throw new Exception("O caminho onde os arquivos aleatórios serão gravados precisa ser um diretório");

            if (string.IsNullOrWhiteSpace(FileNameTemplate))
                throw new Exception("Você precisa informar um template válido para nome do arquivo");

            FileNameTemplate = FileNameTemplate.Replace("{n}", "{0}");

            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            if (FileCount < 1)
                throw new Exception("Você precisa informar pelo menos 1 arquivo para gerar");

            if (string.IsNullOrWhiteSpace(MinFileSize))
                throw new Exception("Você precisa informar o parâmetro --min-file-size");

            if (string.IsNullOrWhiteSpace(MaxFileSize))
                throw new Exception("Você precisa informar o parâmetro --max-file-size");

            var minFileSize = ParseHumanSize(MinFileSize);
            var maxFileSize = ParseHumanSize(MaxFileSize);

            if (minFileSize < 1)
                throw new Exception("O tamanho mínimo do arquivo aleatório precisa ser de pelo menos 1 byte");

            if (maxFileSize < minFileSize)
                throw new Exception("O tamanho máximo do arquivo aleatório precisa ser pelo menos o mesmo tamanho mínimo");

            var currentFile = 1;
            var fileCountCharactersSize = FileCount.ToString().Length;
            var rnd = new Random();

            while (currentFile <= FileCount)
            {
                var fileNumber = (currentFile++).ToString().PadLeft(fileCountCharactersSize, '0');
                var fileName = string.Format(FileNameTemplate, fileNumber);
                var filePath = Path.Combine(OutputPath, fileName);
                var fileSize = rnd.Next(minFileSize, maxFileSize);

                Console.WriteLine("Gerando arquivo {0} com {1}...", fileName, FormatHumanSize(fileSize));

                var fileBytes = new Byte[fileSize];

                rnd.NextBytes(fileBytes);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await stream.WriteAsync(fileBytes, 0, fileSize);
            }
        }

        int ParseHumanSize(string originalHumanSize)
        {
            var humanSize = originalHumanSize;

            var multiplierIterations = 0;

            if (humanSize.Length > 1)
            {
                var suffix = _suffixList
                    .Where(s => humanSize.EndsWith(s, false, CultureInfo.InvariantCulture))
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(suffix))
                {
                    humanSize = humanSize.Substring(0, humanSize.Length - suffix.Length);
                    multiplierIterations = _suffixList.IndexOf(suffix);
                }
            }

            int resultInt;
            if (!int.TryParse(humanSize, out resultInt))
                throw new Exception(string.Format("O tamanho {0} não está em um formato aceito", originalHumanSize));

            for (; multiplierIterations > 0; multiplierIterations--)
                resultInt *= 1024;

            return resultInt;
        }

        string FormatHumanSize(int size)
        {
            float s = Convert.ToSingle(size);

            int iterations = 0;

            while (iterations++ < _suffixList.Count() && s > 1024.0f)
            {
                s /= 1024.0f;
            }

            var humanSize = Convert.ToSingle(Math.Floor(s)) < s
                ? s.ToString("0.0", CultureInfo.InvariantCulture)
                : s.ToString("0", CultureInfo.InvariantCulture);

            if (--iterations >= 0)
                humanSize += _suffixList[iterations];

            return humanSize;
        }
    }
}
