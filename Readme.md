# 🏠 Gerador XML Netwin

Sistema desktop para conversão de arquivos CSV em estrutura de XML organizada para edificações, com suporte a complementos e validação de dados.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Como Usar](#como-usar)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Processamento de Dados](#processamento-de-dados)
- [Validações](#validações)
- [Suporte](#suporte)
- [Versão](#versão)

## 📌 Sobre o Projeto

O **Gerador XML Netwin** é uma aplicação Windows Forms desenvolvida em C#/.NET 8.0 que automatiza o processo de conversão de arquivos CSV contendo dados de endereços e edificações em uma estrutura de pastas e arquivos XML organizada. O sistema é especialmente desenvolvido para atender às necessidades da **Telemont**.

## ⚙️ Funcionalidades

### Principais Funcionalidades

- ✅ **Leitura de arquivos CSV** com validação de colunas obrigatórias
- ✅ **Geração automática de XML** seguindo estrutura padronizada
- ✅ **Suporte a múltiplos complementos** (1, 2 ou 3 complementos)
- ✅ **Organização em pastas** - Uma pasta por moradia
- ✅ **Compressão automática** em arquivo ZIP
- ✅ **Validação de dados** com feedback detalhado
- ✅ **Barra de progresso** com percentual em tempo real
- ✅ **Limpeza automática** de campos após processamento

### Funcionalidades do Conversor CSV

- 🔄 **Conversão de separador** (| para ;)
- 📊 **Pré-visualização dos dados** (primeiras 50 linhas)
- 📁 **Geração de arquivo Power Query** (opcional)
- 🎯 **Processamento completo** de todas as colunas
- 📈 **Barra de progresso** durante conversão
- 🧹 **Limpeza automática** após conversão

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|------------|--------|-------------|
| .NET | 8.0 | Framework principal |
| C# | 12.0 | Linguagem de programação |
| Windows Forms | 8.0 | Interface gráfica |
| System.Text.Json | 8.0 | Serialização JSON |
| System.IO.Compression | 8.0 | Criação de arquivos ZIP |

## 📋 Pré-requisitos

- **Windows 10/11** (64 bits)
- **.NET 8.0 Runtime** ou superior
- **Espaço em disco:** mínimo 100 MB
- **Memória RAM:** mínimo 2 GB (recomendado 4 GB para arquivos grandes)

## 🚀 Instalação

### Usando o executável

1. Baixe o arquivo `GeradorXMLNetwin.exe`
2. Execute como administrador (recomendado)
3. O programa será iniciado imediatamente

### Compilando a partir do código fonte

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/GeradorXMLNetwin.git

# Acesse a pasta do projeto
cd GeradorXMLNetwin

# Restaure os pacotes
dotnet restore

# Compile o projeto
dotnet build --configuration Release

# Execute o programa
dotnet run