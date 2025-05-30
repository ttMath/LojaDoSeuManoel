## Como Executar a Aplicação

o arquivo `.env` na raiz do projeto já está configurado com as senhas e chaves secretas `SQL_PASSWORD`, `JWT_SETTINGS__KEY` e `APPSETTINGS__FRASESECRETAPARATOKEN`.

### 1. Iniciar a Aplicação (Primeira Vez)

* Abra o seu terminal na pasta raiz do projeto (onde está o arquivo `docker-compose.yml`).
* Execute o comando:
    ```bash
    docker-compose up --build
    ```

### 2. Acessar o Swagger

* Após a inicialização, abra o seu navegador e vá para:
    **`http://localhost:5000`**
* Você verá a interface do Swagger, que permite interagir com a API.

### 3. Autenticação no Swagger (Obter Token para Testes)

1.  **Gerar Token de Teste:**
    * No Swagger, localize o controller `Auth` e o endpoint `POST /api/Auth/gerar-token`.
    * Clique em "Try it out".
    * No corpo da requisição ("Request body"), insira a frase secreta configurada no seu arquivo `.env` (variável `APPSETTINGS__FRASESECRETAPARATOKEN`):
        ```json
        {
          "fraseSecreta": "sua-frase-secreta-configurada-no-env"
        }
        ```
    * Clique em "Execute".
    * Na resposta, copie o valor do `accessToken`.

2.  **Autorizar no Swagger:**
    * No topo da página do Swagger, clique no botão "Authorize".
    * Na janela que surgir, cole o `accessToken` copiado no campo "Value", prefixado com `Bearer ` (com um espaço). Exemplo:
        `Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...`
      
### 4. Executar os Testes Unitários

Se você tiver o .NET SDK instalado na sua máquina (além do Docker), pode executar os testes unitários do projeto.

* **Via Terminal:**
    1.  Abra um novo terminal.
    2.  Navegue até a pasta do projeto de testes:
        ```bash
        cd tests/LojaDoSeuManoel.Tests 
        ```
        (Ajuste o caminho se a sua estrutura de pastas for diferente).
    3.  Execute o comando:
        ```bash
        dotnet test
        ```
    Os resultados dos testes serão exibidos no terminal.

* **Via Visual Studio:**
    1.  Abra a solução (`LojaDoSeuManoel.sln`) no Visual Studio.
    2.  Abra o "Gerenciador de Testes" (Test Explorer). Você pode encontrá-lo no menu `Testar > Gerenciador de Testes` (ou `Test > Test Explorer`).
    3.  No Gerenciador de Testes, você pode executar todos os testes, testes de um projeto específico, ou testes individuais clicando com o botão direito e selecionando "Executar".
