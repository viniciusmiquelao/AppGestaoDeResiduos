using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SpecFlow.Actions.Selenium;
using TechTalk.SpecFlow;
using AppGestaoDeResiduos.Dto;

namespace TesteEndpoint.Steps;

[Binding]
public class CaminhaoSteps
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private HttpResponseMessage _response;
    private CaminhaoDto _caminhaoDto;

    public CaminhaoSteps(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Given(@"que estou autenticado no sistema")]
    public void DadoQueEstouAutenticadoNoSistema()
    {
        // Implementar autenticação se necessário
    }

    [When(@"eu envio os dados de um novo caminhão")]
    public async Task QuandoEuEnvioOsDadosDeUmNovoCaminhao(Table table)
    {
        var row = table.Rows[0];
        _caminhaoDto = new CaminhaoDto
        {
            Placa = row["Placa"],
            Modelo = row["Modelo"],
            Capacidade = decimal.Parse(row["Capacidade"])
        };

        _response = await _client.PostAsJsonAsync("/api/caminhao", _caminhaoDto);
    }

    [Then(@"o sistema deve retornar status (.*)")]
    public void EntaoOSistemaDeveRetornarStatus(int statusCode)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"o caminhão deve ser cadastrado com sucesso")]
    public async Task EntaoOCaminhaoDeveSerCadastradoComSucesso()
    {
        var caminhao = await _response.Content.ReadFromJsonAsync<CaminhaoDto>();
        caminhao.Should().NotBeNull();
        caminhao.Placa.Should().Be(_caminhaoDto.Placa);
    }

    [Given(@"que existe um caminhão cadastrado com a placa ""(.*)""")]
    public async Task DadoQueExisteUmCaminhaoCadastradoComAPlaca(string placa)
    {
        _caminhaoDto = new CaminhaoDto
        {
            Placa = placa,
            Modelo = "Teste",
            Capacidade = 10
        };

        await _client.PostAsJsonAsync("/api/caminhao", _caminhaoDto);
    }

    [When(@"eu tento cadastrar um novo caminhão com a mesma placa")]
    public async Task QuandoEuTentoCadastrarUmNovoCaminhaoComAMesmaPlaca()
    {
        _response = await _client.PostAsJsonAsync("/api/caminhao", _caminhaoDto);
    }

    [Then(@"deve exibir mensagem de erro informando placa duplicada")]
    public async Task EntaoDeveExibirMensagemDeErroInformandoPlacaDuplicada()
    {
        var response = await _response.Content.ReadAsStringAsync();
        response.Should().Contain("placa duplicada");
    }

    [Given(@"que existem caminhões cadastrados no sistema")]
    public async Task DadoQueExistemCaminhoesCadastradosNoSistema()
    {
        // Cadastrar alguns caminhões de teste
        var caminhoes = new[]
        {
            new CaminhaoDto { Placa = "ABC1234", Modelo = "Volvo", Capacidade = 10 },
            new CaminhaoDto { Placa = "DEF5678", Modelo = "Scania", Capacidade = 15 }
        };

        foreach (var caminhao in caminhoes)
        {
            await _client.PostAsJsonAsync("/api/caminhao", caminhao);
        }
    }

    [When(@"eu solicito a listagem de caminhões")]
    public async Task QuandoEuSolicitoAListagemDeCaminhoes()
    {
        _response = await _client.GetAsync("/api/caminhao");
    }

    [Then(@"deve retornar uma lista com todos os caminhões cadastrados")]
    public async Task EntaoDeveRetornarUmaListaComTodosOsCaminhoesCadastrados()
    {
        var caminhoes = await _response.Content.ReadFromJsonAsync<List<CaminhaoDto>>();
        caminhoes.Should().NotBeNull();
        caminhoes.Should().HaveCountGreaterThan(0);
    }
}