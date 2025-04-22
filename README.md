# AppGestãoDeResiduos - Projeto com CI/CD, Docker e Orquestração

Este projeto é um exemplo prático de como integrar práticas de DevOps em um aplicativo de Cidades Inteligentes. Utilizando **CI/CD**, **Docker** e **Docker Compose**, este projeto visa automatizar a integração e deploy contínuos, além de garantir que a aplicação seja executada de forma consistente em qualquer ambiente.

## Estrutura do Projeto

O projeto consiste em uma aplicação desenvolvida em **C# (ASP.NET)**. Ele inclui práticas de DevOps para garantir a automação e robustez do processo de entrega.

## Tecnologias Utilizadas

- **CI/CD**: GitHub Actions (ou Azure DevOps, Jenkins)
- **Containerização**: Docker
- **Orquestração**: Docker Compose
- **Ambientes**: Staging e Produção

## Passos para Execução

### 1. Clone o Repositório

Clone o repositório em sua máquina local:

```
git clone https://github.com/viniciusmiquelao/AppGestaoDeResiduos.git cd AppGestaoDeResiduos
```

### 2. Configuração do GitHub Actions

O pipeline de CI/CD está configurado para ser executado automaticamente no **GitHub Actions**. Ao realizar um **push** para a branch `main`, o pipeline executará as etapas de:

- **Compilação**: Build da aplicação C#.
- **Testes**: Execução de testes unitários.
- **Deploy**: Deploy nos ambientes de **Staging** e **Produção**.

Arquivo: `.github/workflows/ci-cd.yml`

### 3. Configuração do Docker

A aplicação está containerizada usando **Docker**. Para construir e rodar o projeto em Docker, siga os passos abaixo:

#### 3.1 Build da Imagem Docker

Crie a imagem Docker com o comando:

```
docker build -t app-gestao-de-residuos .
```

#### 3.2 Rodando a Aplicação

Execute a aplicação utilizando o Docker Compose:

```
docker-compose up
```

A aplicação estará disponível em `http://localhost:5000`.

### 4. Configuração do Docker Compose

O arquivo `docker-compose.yml` orquestra o serviço da aplicação. Ele define como o container deve ser construído e quais portas serão expostas para os ambientes de **staging** e **produção**.

### 5. Deploy em Staging e Produção

Após o push para a branch `main`, o pipeline no GitHub Actions automaticamente realizará o deploy em dois ambientes:

- **Staging**: Ambiente de pré-produção, onde são feitos os testes finais.
- **Produção**: Ambiente final, acessível pelos usuários.

## Documentação de CI/CD

Esta seção descreve o pipeline de CI/CD configurado para automação do fluxo de desenvolvimento e entrega.

- **Passos do Pipeline**:
  1. **Checkout do código**.
  2. **Compilação e execução de testes**.
  3. **Deploy nos ambientes de Staging e Produção**.

### Prints do Pipeline

_Aqui, você deve adicionar prints do GitHub Actions mostrando os logs e resultados do pipeline._

## Conclusão

Com as práticas de CI/CD e Docker configuradas, o projeto "AppGestaoDeResiduos" agora possui um processo de desenvolvimento e deploy totalmente automatizado, garantindo consistência e agilidade. A aplicação é facilmente containerizada e orquestrada para diferentes ambientes, proporcionando robustez e escalabilidade.
