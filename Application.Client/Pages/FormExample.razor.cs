using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Application.Client.Pages;

public partial class FormExample : ComponentBase
{
    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    public StudentFormModel Form { get; set; } = default!;
    public EditContext EditContext { get; set; } = default!;
    public ValidationMessageStore ValidationMessageStore { get; set; } = default!;

    public bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // inisialisasi di sini
        ResetForm();
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        // get data di sini
        await Task.Delay(1000);

        IsLoading = false;
    }

    void ResetForm()
    {
        Form = new StudentFormModel();
        EditContext = new EditContext(Form);
        ValidationMessageStore = new ValidationMessageStore(EditContext);
    }

    async Task HandleSubmit()
    {
        IsLoading = true;

        ValidationMessageStore.Clear();

        if (string.IsNullOrWhiteSpace(Form.FullName))
        {
            ValidationMessageStore.Add(EditContext.Field(nameof(Form.FullName)), "FullName cannot be empty.");
        }

        if (Form.DateOfBirth <= new DateTime(2000, 1, 1))
        {
            ValidationMessageStore.Add(EditContext.Field(nameof(Form.DateOfBirth)), "You're too old.");
        }

        var isValid = EditContext.Validate();

        if (isValid)
        {
            // delay submit data ke server
            await Task.Delay(2000);
            Snackbar.Add("Success.", Severity.Success);

            // cara 1
            ResetForm();

            // cara 2, reset satu-satu
            // Form.FullName = string.Empty;
        }
        else
        {
            Console.WriteLine("not valid");
        }

        IsLoading = false;
    }

    async Task<IEnumerable<AutoCompleteModel?>> GetSongs(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            searchText = "Hey Jude";

        try
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri($"https://itunes.apple.com/search?term={searchText}&media=music&entity=musicTrack")
            };

            var response = await client.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get, });
            // throw error jika bukan 200
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ITunesResponse>();

            return result!.Results.Select(x => new AutoCompleteModel
            {
                Id = x.TrackId.ToString(),
                Description = x.ArtistName + " - " + x.TrackName
            });
        }
        catch
        {
            return new List<AutoCompleteModel?>();
        }
    }
}

public class StudentFormModel
{
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public AutoCompleteModel? FavoriteSong { get; set; }
}

public class AutoCompleteModel
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ITunesResponse
{
    public int ResultCount { get; set; }
    public List<ITunesResponseDetail> Results { get; set; } = new();
}

public class ITunesResponseDetail
{
    public string ArtistName { get; set; } = string.Empty;
    public int TrackId { get; set; }
    public string TrackName { get; set; } = string.Empty;
}
