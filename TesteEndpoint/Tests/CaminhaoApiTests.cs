using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using AppGestaoDeResiduos.Dto;
using Xunit;

namespace TesteEndpoint.Tests;

public class CaminhaoApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CaminhaoApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_Caminhao_DeveRetornar201()
    {
        // Arrange
        var caminhao = new CaminhaoDto
        {
            Placa = "ABC1234",
            Modelo = "Volvo",
            Capacidade = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/caminhao", caminhao);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CaminhaoDto>();
        result.Should().NotBeNull();
        result.Placa.Should().Be(caminhao.Placa);
    }

    [Fact]
    public async Task Post_CaminhaoComPlacaDuplicada_DeveRetornar400()
    {
        // Arrange
        var caminhao = new CaminhaoDto
        {
            Placa = "ABC1234",
            Modelo = "Volvo",
            Capacidade = 10
        };

        await _client.PostAsJsonAsync("/api/caminhao", caminhao);

        // Act
        var response = await _client.PostAsJsonAsync("/api/caminhao", caminhao);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("placa duplicada");
    }

    [Fact]
    public async Task Get_Caminhoes_DeveRetornar200()
    {
        // Arrange
        var caminhoes = new[]
        {
            new CaminhaoDto { Placa = "ABC1234", Modelo = "Volvo", Capacidade = 10 },
            new CaminhaoDto { Placa = "DEF5678", Modelo = "Scania", Capacidade = 15 }
        };

        foreach (var caminhao in caminhoes)
        {
            await _client.PostAsJsonAsync("/api/caminhao", caminhao);
        }

        // Act
        var response = await _client.GetAsync("/api/caminhao");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<CaminhaoDto>>();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task Get_CaminhaoPorId_DeveRetornar200()
    {
        // Arrange
        var caminhao = new CaminhaoDto
        {
            Placa = "ABC1234",
            Modelo = "Volvo",
            Capacidade = 10
        };

        var postResponse = await _client.PostAsJsonAsync("/api/caminhao", caminhao);
        var createdCaminhao = await postResponse.Content.ReadFromJsonAsync<CaminhaoDto>();

        // Act
        var response = await _client.GetAsync($"/api/caminhao/{createdCaminhao.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CaminhaoDto>();
        result.Should().NotBeNull();
        result.Id.Should().Be(createdCaminhao.Id);
    }
}