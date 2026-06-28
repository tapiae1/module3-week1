using System.Net.Http.Json;

// for using the converter attribute on the Name pokemon type property. 
using System.Text.Json.Serialization; 

string pokemonlink = "https://pokeapi.co/api/v2/pokemon"; 
await RunApp(); 

async Task RunApp()
{
    using var http = new HttpClient();
    
    //var response = await http.GetStringAsync("https://pokeapi.co/api/v2/pokemon");


    // Deserialize JSON and store it in "response", then print it out
    var response = await http.GetFromJsonAsync<PokemonListResponse?>(pokemonlink); 
    DateTime now = DateTime.Now; 
    Console.WriteLine($"Date and Time of data retrieval: {now}"); 




    // If the response was empty, nothing in the link 
    if (response == null)
    {
        Console.WriteLine("No data received"); 
        return; 
    }

    // Print out pokemon to check if we got data. 
    foreach (var pokemon in response.Results)
    {
        var detail = await http.GetFromJsonAsync<PokemonData>(pokemon.Url);
        if (detail == null) continue;  

        // Print out the types for each pokemon 
        var types = string.Join(", ", detail.Types.Select(t => t.Type.Name));     
        Console.WriteLine($"{detail.Name} | Type: {types} | Height: {detail.Height} | Weight: {detail.Weight}"); 
    }

}
// Were making an enum, full of pokemon type values, previously, they were just strings. 
enum PokemonType
{
    Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}


// Make two classes to use when the JSON is deserialized 

class PokemonListResponse
{
    public int Count { get; set; }
    public string? Next { get; set;}
    public string? Previous { get; set; }

    public List<PokemonListItem> Results { get; set; } = []; 
}

class PokemonListItem
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = ""; 
}


// Create a new class that will hold the individual pokemon data. 
class PokemonData
{
    public int Id { get; set; }
    public string Name { get; set; } = ""; 
    public int Height { get; set; }
    public int Weight { get; set; }
    public List<PokemonTypeSlot> Types { get; set; } = []; 
}

// Create new class that will handle Types slot 
class PokemonTypeSlot
{
    public int Slot { get; set; }
    public PokemonTypeInfo Type { get; set; } = new(); 
}

class PokemonTypeInfo
{
    // Converter attribute on Name property to let the deserializer convert from string
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PokemonType Name { get; set; }
}