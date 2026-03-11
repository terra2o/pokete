using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

ConfigurationManager.Enable(ConfigLocations.All);
Application
    .Create()
    .Run<terminal_guiWindow>()
    .Dispose();

// This attribute ensures the JSON metadata survives the trimming process
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<Pokemon>))]
internal partial class MyJsonContext : JsonSerializerContext { }

public class Pokemon
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public int HP { get; set; }
}

internal class terminal_guiWindow : Window
{
    public terminal_guiWindow()
    {
        string jsonString = File.ReadAllText("pokeInfos.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var pokemonList = JsonSerializer.Deserialize(jsonString, MyJsonContext.Default.ListPokemon);

        var sideBar = new View()
        {
            Title = "Pokedex",
            Width = 18,
            Height = Dim.Fill(),
            BorderStyle = LineStyle.Dotted,
        };

        var mainWindow = new View()
        {
            Title = "Main Window",
            X = 18,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.HeavyDashed
        };

        View pokeDetail = new Label()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2,
            // (why is this not in docs??)
            TextFormatter = { WordWrap = true }
        };

        int row = 0; // initialising row counter
        foreach (var p in pokemonList!)
        {
            var pokeButton = new Button
            {
                Title = p.Name,
                X = 0,
                Y = row++
            };

            pokeButton.Accepting += (s, e) =>
            {
                pokeDetail.Text = $"{p.Name} ({p.Type}) (Health: {p.HP})\n\n{p.Description}";

                e.Handled = true;
            };
            sideBar.Add(pokeButton);
            sideBar.SetContentSize(new(16, pokemonList.Count));
            sideBar.ViewportSettings |= ViewportSettingsFlags.AllowYGreaterThanContentHeight;
            sideBar.VerticalScrollBar.Visible = true;
        }

        mainWindow.Add(pokeDetail);
        Add(sideBar, mainWindow);
    }
}
