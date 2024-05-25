using System.Net.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var httpClient = new HttpClient();

app.MapGet("/pokemon", async () =>
{
    var pokemonName = "pikachu"; 
    var apiUrl = $"https://pokeapi.co/api/v2/pokemon/{pokemonName.ToLower()}";
    var response = await httpClient.GetFromJsonAsync<PokemonResponse>(apiUrl);

    if (response == null)
    {
        return Results.NotFound();
    }

    var pokemonInfo = new
    {
        Name = response.Name,
        Types = response.Types.Select(t => t.Type.Name).ToList(),
        SpriteUrl = response.Sprites.FrontDefault,
        Moves = response.Moves.Select(m => m.Move.Name).ToList()
    };

    return Results.Ok(pokemonInfo);
})
.WithName("GetPokemonInfo")
.WithOpenApi();

app.Run();

public class PokemonResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonType> Types { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSprites Sprites { get; set; }

    [JsonPropertyName("moves")]
    public List<PokemonMove> Moves { get; set; }
}

public class PokemonType
{
    [JsonPropertyName("type")]
    public TypeInfo Type { get; set; }
}

public class TypeInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class PokemonSprites
{
    [JsonPropertyName("front_default")]
    public string FrontDefault { get; set; }
}

public class PokemonMove
{
    [JsonPropertyName("move")]
    public MoveInfo Move { get; set; }
}

public class MoveInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
