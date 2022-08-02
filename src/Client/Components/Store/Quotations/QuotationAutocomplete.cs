using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Suppliers;

public class QuotationAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<QuotationAutocomplete> L { get; set; } = default!;
    [Inject]
    private IQuotationsClient QuotationsClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<QuotationDto> _quotations = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Quotation"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchQuotation;
        ToStringFunc = GetQuotationName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    // when the value parameter is set, we have to load that one brand to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => QuotationsClient.GetAsync(_value), Snackbar) is { } quotation)
        {
            _quotations.Add(quotation);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchQuotation(string value)
    {
        var filter = new SearchQuotationsRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => QuotationsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfQuotationDto response)
        {
            _quotations = response.Data.ToList();
        }

        return _quotations.Select(x => x.Id);
    }

    private string GetQuotationName(Guid id) =>
        _quotations.Find(b => b.Id == id)?.Name ?? string.Empty;

}