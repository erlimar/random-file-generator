Gerador de Arquivos Aleatórios
==============================

Utilitário para gerar arquivos aleatórios.

> Com controle de quantidade, nome e tamano dos arquivos gerados.


# Instação

1) Baixe o arquivo no link [Releases aqui no GitHub](releases) conforme seu sistema operacional;
2) Descompacte em uma pasta de sua escolha
3) Adicione a pasta que você descompactou a variável `$PATH` do sistema

Em uma linha de comando:
```console
$ rfg --help
Gerador de arquivos aleatórios

Usage: rfg [options]

Options:
  -c|--count               Quantidade de arquivos a gerar
  -n|--min-file-size       Tamanho mínimo dos arquivos gerados (em bytes. Ou sufixado com b, KB, MB, GB. Ex: 5KB)
  -x|--max-file-size       Tamanho máximo dos arquivos gerados (em bytes. Ou sufixado com b, KB, MB, GB. Ex: 5KB)
  -o|--output-path         Caminho onde os arquivos aleatórios serão gravados. O padrão é o diretório local.
  -t|--file-name-template  Template do nome do arquivo. Use '{n}' para o número do arquivo. O padrão é 'file-{n}.bin'.
  -?|-h|--help             Show help information
```

> No **Linux** e **macOS** você precisará dar permissão de execução ao utilitário `rfg`

```console
$ cd pasta_onde_voce_descompactou
$ chmod +x rfg
```

# Uso básico

Gerar **10** arquivos aleatórios entre **5KB** e **2MB** de tamanho com 
nome `arquivo-{n}.bin` na pasta local.

```console
$ rfg --count 10 --min-file-size 5KB --max-file-size 2MB --file-name-template "arquivo-{n}.bin" --output-path .
```

Mais simplesmente poderia ser assim também:

```console
$ rfg -c 100 -n 5KB -x 2MB -t "arquivo-{n}.bin"
Gerando arquivo arquivo-01.bin com 1.8MB...
Gerando arquivo arquivo-02.bin com 1.2MB...
Gerando arquivo arquivo-03.bin com 558.2KB...
Gerando arquivo arquivo-04.bin com 1.0MB...
Gerando arquivo arquivo-05.bin com 42.2KB...
Gerando arquivo arquivo-06.bin com 988.8KB...
Gerando arquivo arquivo-07.bin com 1.4MB...
Gerando arquivo arquivo-08.bin com 389.5KB...
Gerando arquivo arquivo-09.bin com 1.2MB...
Gerando arquivo arquivo-10.bin com 63.0KB...
```
