#
# Copyright (c) 2020 Erlimar Silva Campos. Todos os direitos reservados
#

<#
.SYNOPSIS
    Gera o executável de publicação
.DESCRIPTION
    Gera o executável de publicação para a plataforma desejada
#>

[cmdletbinding()]
param(
   [ValidateSet("Debug", "Release")]
   [string]$Configuration="Release",
   [ValidateSet(
        "win-x64", "win-x86", "win-arm", "win-arm64",
        "win7-x64", "win7-x86",
        "win81-x64", "win81-x86", "win81-arm",
        "win10-x64", "win10-x86", "win10-arm", "win10-arm64",
        
        "linux-x64", "linux-musl-x64", "linux-arm", "linux-arm64",
        "rhel-x64", "rhel.6-x64",
        "tizen", "tizen.4.0.0", "tizen.5.0.0",
        
        "osx-x64", "osx.10.10-x64", "osx.10.11-x64",
        "osx.10.12-x64", "osx.10.13-x64", "osx.10.14-x64",
        
        IgnoreCase = $false)]
   [string]$Runtime = "win10-x64",
   [switch]$SelfContained=$False
)

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"

function Say($str) {
    Write-Host "Publish> $str"
}

$ScriptName = $MyInvocation.MyCommand.Name
$Project = "./src/RandomFileGenerator/RandomFileGenerator.csproj"

try {
    $output = "./dist/rfg-${Runtime}"

    if ($SelfContained) {
        $output += "-full"
    }

    Say "Publicando.."
    
    dotnet publish -c $Configuration --self-contained $SelfContained -o $output -r $Runtime $Project

    Say "Compactando artefatos..."

    Compress-Archive -CompressionLevel Optimal -Force -Path $output -DestinationPath "${output}.zip"

    Say "Publicação finalizada!"
}
catch {
    Say "Ocorreu o seguinte erro ao publicar: ${_}"
}
