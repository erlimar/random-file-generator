@echo off
:;
:: Copyright (c) 2020 Erlimar Silva Campos. Todos os direitos reservados
::
:: Gera o executável de publicação para a plataforma desejada
:;

PowerShell -NoLogo -File .\publish.ps1 %*
