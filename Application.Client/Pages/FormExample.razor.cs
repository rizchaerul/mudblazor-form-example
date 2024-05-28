using System.ComponentModel.DataAnnotations;
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
            ValidationMessageStore.Add(
                fieldIdentifier: EditContext.Field(nameof(Form.FullName)),
                message: "FullName cannot be empty."
            );
        }

        if (Form.DateOfBirth <= new DateTime(2000, 1, 1))
        {
            ValidationMessageStore.Add(
                fieldIdentifier: EditContext.Field(nameof(Form.DateOfBirth)),
                message: "You're too old."
            );
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
}

public class StudentFormModel
{
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
}
