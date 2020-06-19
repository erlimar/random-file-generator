using System.IO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static McMaster.Extensions.CommandLineUtils.CommandLineApplication;

namespace RandomFileGenerator
{
    [Command(Description = "Gerador de arquivos aleatórios")]
    public class Program
    {

        [Required, Option("-c|--count", Description = "Quantidade de arquivos a gerar")]
        public int FileCount { get; set; }

        [Required, Option("-n|--min-file-size", Description = "Tamanho mínimo dos arquivos gerados (em bytes)")]
        public int MinFileSize { get; set; }

        [Required, Option("-x|--max-file-size", Description = "Tamanho máximo dos arquivos gerados (em bytes)")]
        public int MaxFileSize { get; set; }

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

            if (MinFileSize < 1)
                throw new Exception("O tamanho mínimo do arquivo aleatório precisa ser de pelo menos 1 byte");

            if (MaxFileSize < MinFileSize)
                throw new Exception("O tamanho máximo do arquivo aleatório precisa ser pelo menos o mesmo tamanho mínimo");

            var currentFile = 1;
            var fileCountCharactersSize = FileCount.ToString().Length;
            var rnd = new Random();

            while (currentFile <= FileCount)
            {
                var fileNumber = (currentFile++).ToString().PadLeft(fileCountCharactersSize, '0');
                var fileName = string.Format(FileNameTemplate, fileNumber);
                var filePath = Path.Combine(OutputPath, fileName);
                var fileSize = rnd.Next(MinFileSize, MaxFileSize);

                Console.WriteLine("Gerando arquivo {0} com {1} byte{2}...", fileName, fileSize, (fileSize > 1 ? "s" : string.Empty));

                var fileBytes = new Byte[fileSize];

                rnd.NextBytes(fileBytes);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await stream.WriteAsync(fileBytes, 0, fileSize);
            }
        }
    }
}
