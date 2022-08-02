using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Suppliers;

public class DeliveryAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<DeliveryAutocomplete> L { get; set; } = default!;
    [Inject]
    private IDeliveriesClient DeliveriesClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<DeliveryDto> _deliveries = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Delivery"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchDelivery;
        ToStringFunc = GetDeliveryName;
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
                () => DeliveriesClient.GetAsync(_value), Snackbar) is { } order)
        {
            _deliveries.Add(order);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchDelivery(string value)
    {
        var filter = new SearchDeliveriesRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => DeliveriesClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfDeliveryDto response)
        {
            _deliveries = response.Data.ToList();
        }

        return _deliveries.Select(x => x.Id);
    }

    private string GetDeliveryName(Guid id) =>
        _deliveries.Find(b => b.Id == id)?.Name ?? string.Empty;

}