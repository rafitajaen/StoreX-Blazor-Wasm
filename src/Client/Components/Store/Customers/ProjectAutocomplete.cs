using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Suppliers;

public class ProjectAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<SharedResource> L { get; set; } = default!;
    [Inject]
    private IProjectsClient ProjectsClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<ProjectDto> _projects = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Project"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchProject;
        ToStringFunc = GetProjectName;
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
                () => ProjectsClient.GetAsync(_value), Snackbar) is { } project)
        {
            _projects.Add(project);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchProject(string value)
    {
        var filter = new SearchProjectsRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ProjectsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfProjectDto response)
        {
            _projects = response.Data.ToList();
        }

        return _projects.Select(x => x.Id);
    }

    private string GetProjectName(Guid id) =>
        _projects.Find(b => b.Id == id)?.Name ?? string.Empty;

}