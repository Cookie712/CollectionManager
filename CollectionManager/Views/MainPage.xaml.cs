using CollectionManager.Models;

namespace CollectionManager.Views;

public partial class MainPage : ContentPage
{
    private readonly FileService _fileService = new();
    private List<Collection> _collections = new();

    public MainPage()
    {
        InitializeComponent();
        LoadCollections();
    }

    private async void LoadCollections()
    {
        var names = _fileService.ListFiles();
        _collections = new();

        foreach (var name in names)
        {
            var c = await _fileService.LoadCollection(name);
            if (c != null) _collections.Add(c);
        }

        CollectionList.ItemsSource = _collections;
    }

    private async void AddCollection_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Nowa kolekcja", "Podaj nazwê kolekcji:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            var newCol = new Collection { Name = result };
            await _fileService.SaveCollection(newCol);
            LoadCollections();
        }
    }

    private async void CollectionList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var collection = (Collection)e.Item;
        await Navigation.PushAsync(new CollectionPage(collection));
    }
}