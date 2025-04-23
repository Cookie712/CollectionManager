using CollectionManager.Models;

namespace CollectionManager.Views;

public partial class CollectionPage : ContentPage
{
    private Collection _collection;
    private readonly FileService _fileService = new();

    public CollectionPage(Collection collection)
    {
        InitializeComponent();
        _collection = collection;
        Title = _collection.Name;
        ItemList.ItemsSource = _collection.Items;
    }

    private async void AddItem_Clicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nowy przedmiot", "Nazwa:");
        if (string.IsNullOrWhiteSpace(name)) return;

        string desc = await DisplayPromptAsync("Nowy przedmiot", "Opis:");
        if (string.IsNullOrWhiteSpace(desc)) return;

        // Sprawdzenie duplikatu
        if (_collection.Items.Any(item => item.Name == name && item.Description == desc))
        {
            bool continue = await DisplayAlert("Uwaga",
                "Przedmiot o takiej nazwie i opisie ju¿ istnieje.\nCzy mimo to chcesz go dodaæ?",
                "Tak", "Nie");
            if (!continue) return;
        }

        string other = await DisplayPromptAsync("Nowy przedmiot", "Inne:");
        string priceStr = await DisplayPromptAsync("Nowy przedmiot", "Cena (np. 25.50):");
        string status = await DisplayPromptAsync("Nowy przedmiot", "Status (np. Nowy, U¿ywany, Na sprzeda¿):");
        string ratingStr = await DisplayPromptAsync("Nowy przedmiot", "Ocena (1–10):");
        string comment = await DisplayPromptAsync("Nowy przedmiot", "Komentarz:");

        decimal.TryParse(priceStr, out var price);
        int.TryParse(ratingStr, out var rating);

        string imagePath = "";
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Wybierz zdjêcie",
            FileTypes = FilePickerFileType.Images
        });
        if (result != null)
            imagePath = result.FullPath;

        var newItem = new CollectionItem
        {
            Name = name,
            Description = desc,
            Other = other,
            Price = price,
            Status = status,
            Rating = rating,
            Comment = comment,
            ImagePath = imagePath,
            IsSold = false
        };

        _collection.Items.Add(newItem);
        await _fileService.SaveCollection(_collection);
        ItemList.ItemsSource = null;
        ItemList.ItemsSource = _collection.Items;
    }


    private async void EditItem_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is CollectionItem item)
        {
            string newName = await DisplayPromptAsync("Edytuj nazwê", "", initialValue: item.Name);
            string newDesc = await DisplayPromptAsync("Edytuj opis", "", initialValue: item.Description);

            bool isDuplicate = _collection.Items.Any(i =>
                i != item && i.Name == newName && i.Description == newDesc);

            if (isDuplicate)
            {
                bool continue = await DisplayAlert("Uwaga",
                    "Inny przedmiot o takiej nazwie i opisie ju¿ istnieje.\nCzy mimo to chcesz zapisaæ zmiany?",
                    "Tak", "Nie");
                if (!continue) return;
            }

            string newOther = await DisplayPromptAsync("Edytuj inne", "", initialValue: item.Other);
            string newStatus = await DisplayPromptAsync("Edytuj status", "", initialValue: item.Status);
            string newPriceStr = await DisplayPromptAsync("Edytuj cenê", "", initialValue: item.Price.ToString());
            string newRatingStr = await DisplayPromptAsync("Edytuj ocenê", "", initialValue: item.Rating.ToString());
            string newComment = await DisplayPromptAsync("Edytuj komentarz", "", initialValue: item.Comment);

            if (int.TryParse(newRatingStr, out int rating)) item.Rating = rating;
            if (decimal.TryParse(newPriceStr, out decimal price)) item.Price = price;

            item.Name = newName;
            item.Description = newDesc;
            item.Other = newOther;
            item.Status = newStatus;
            item.Comment = newComment;

            // Obs³uga zmiany zdjêcia
            bool changeImage = await DisplayAlert("Zdjêcie", "Czy chcesz zmieniæ zdjêcie?", "Tak", "Nie");
            if (changeImage)
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Wybierz nowe zdjêcie",
                    FileTypes = FilePickerFileType.Images
                });
                if (result != null)
                    item.ImagePath = result.FullPath;
            }

            await _fileService.SaveCollection(_collection);
            ItemList.ItemsSource = null;
            ItemList.ItemsSource = _collection.Items;
        }
    }



    private async void DeleteItem_Clicked(object sender, EventArgs e)
    {
        if (ItemList.SelectedItem is CollectionItem selected)
        {
            _collection.Items.Remove(selected);
            await _fileService.SaveCollection(_collection);
            ItemList.ItemsSource = null;
            ItemList.ItemsSource = _collection.Items;
        }
    }


    private void UpdateItemDisplay()
    {
        foreach (var item in _collection.Items)
        {
            var viewCell = ItemList.FindByName<ViewCell>(item.Name);
            if (viewCell != null)
            {
                var stackLayout = viewCell.FindByName<StackLayout>("ItemStackLayout");
                if (item.IsSold)
                {
                    stackLayout.BackgroundColor = Colors.Gray; 
                }
                else
                {
                    stackLayout.BackgroundColor = Colors.Transparent; 
                }
            }
        }
    }

    private async void MarkAsSold_Clicked(object sender, EventArgs e)
    {
        if (ItemList.SelectedItem is CollectionItem selected)
        {
            selected.IsSold = true;
            selected.Status = "Sprzedany"; 
            _collection.Items.Remove(selected);
            _collection.Items.Add(selected);

            await _fileService.SaveCollection(_collection);
            ItemList.ItemsSource = null;
            ItemList.ItemsSource = _collection.Items;
            UpdateItemDisplay(); 
        }
    }


}
