Feature: Gestão de Caminhões
    Como um usuário do sistema
    Eu quero gerenciar caminhões
    Para controlar a frota de veículos

    Scenario: Cadastrar um novo caminhão com sucesso
        Given que estou autenticado no sistema
        When eu envio os dados de um novo caminhão
            | Placa    | Modelo | Capacidade |
            | ABC1234  | Volvo  | 10         |
        Then o sistema deve retornar status 201
        And o caminhão deve ser cadastrado com sucesso

    Scenario: Tentar cadastrar caminhão com placa duplicada
        Given que existe um caminhão cadastrado com a placa "ABC1234"
        When eu tento cadastrar um novo caminhão com a mesma placa
        Then o sistema deve retornar status 400
        And deve exibir mensagem de erro informando placa duplicada

    Scenario: Listar todos os caminhões
        Given que existem caminhões cadastrados no sistema
        When eu solicito a listagem de caminhões
        Then o sistema deve retornar status 200
        And deve retornar uma lista com todos os caminhões cadastrados 