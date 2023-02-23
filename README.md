# AutoComplete for Blazor 

The Autocomplete for Blazor component offers simple and flexible autocomplete type-ahead functionality for [Blazor WebAssembly](https://www.puresourcecode.com/tag/blazor-webassembly/) and [Blazor Server](https://www.puresourcecode.com/tag/blazor-server/). The components is build with [NET6](https://www.puresourcecode.com/tag/net6/).

## Installing

You can install from NuGet using the following command:

`Install-Package PSC.Blazor.Components.AutoComplete`

Or via the Visual Studio package manger.

## Setup
Blazor Server applications will need to include the following CSS and JS files in their `_Host.cshtml` .

Blazor Client applications will need to include the following CSS and JS files in their `Index.html` .

In the `head` tag add the following CSS.

```html
<link href="_content/PSC.Blazor.Components.AutoComplete/autocomplete.css" rel="stylesheet" />
```

Then add the JS script at the bottom of the page using the following script tag.

```html
<script src="_content/PSC.Blazor.Components.AutoComplete/autocomplete.js"></script>
```

I would also suggest adding the following using statement to your main `_Imports.razor` to make referencing the component a bit easier.

```cs
@using PSC.Blazor.Components.AutoComplete
```

## Usage
The component can be used standalone or as part of a form. When used in a form the control fully integrates with Blazors forms and authentication system.

Below is a list of all the options available on the AutoComplete.

### Templates

- `ResultTemplate` (Required) - Allows the user to define a template for a result in the results list
- `SelectedTemplate` (Required) - Allows the user to define a template for a selected item
- `HelpTemplate` - Allows the user to define a template to show when the `MinimumLength` to perform a search hasn't been reached
- `NotFoundTemplate` - Allows the user to define a template when no items are found
- `FooterTemplate` - Allows the user to define a template which is displayed at the end of the results list

### Parameters

- `MinimumLength` (Optional - Default: 1) - Minimum number of characters before starting a search
- `Debounce` (Optional - Default: 300) - Time to wait after last keypress before starting a search
- `MaximumSuggestions` (Optional - Default: 10) - Controls the amount of suggestions which are shown
- `Disabled` (Optional - Default: `false`) - Marks the control as disabled and stops any interaction
- `EnableDropDown` (Optional - Default: `false`) - Allows the control to behave as a dropdown
- `DisableClear` (Optional - Default : `false`) - Hides the clear button from the AutoComplete. Users can still change the selection by clicking on the current selection and typing however, they can't clear the control entirely.'
- `ShowDropDownOnFocus` (Optional - Default: `false`) - When enabled, will show the suggestions dropdown automatically when the control is in search mode. If the control has a current value then the user would need to press the enter key first to enter search mode.
- `StopPropagation` (Optional - Default: `false`) - Control the StopPropagation behavior of the input of this component. See https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.1#stop-event-propagation
- `PreventDefault` (Optional - Default: `false`) - Control the PreventDefault behavior of the input of this component. See https://docs.microsoft.com/en-us/aspnet/core/blazor/components?view=aspnetcore-3.1#prevent-default-actions

The control also requires a `SearchMethod` to be provided with the following signature `Task<IEnumerable<TItem>>(string searchText)`. The control will invoke this method 
passing the text the user has typed into the control. You can then query your data source and return the result as an `IEnumerable` for the control to render.

If you wish to bind the result of the selection in the control to a different type than the type used in the search this is also possible. For example, if you passed in a list
of `Person` but when a `Person` was selected you wanted the control to bind to an `int` value which might be the `Id` of the selected `Person`, you can achieve this by providing
a `ConvertMethod` The convert method will be invoked by the control when a selection is made and will be passed the type selected. The method will need to handle the conversion
and return the new type.

If you want to allow adding an item based on the search when no items have been found, you can achieve this by providing the `AddItemOnEmptyResultMethod` as a parameter.
This method will make the `NotFoundTemplate` selectable the same way a item would normally be, and will be invoked when the user selects the `NotFoundTemplate`.
This method passes the `SearchText` and expects a new item to be returned.

### Local Data Example

```cs
<EditForm Model="MyFormModel" OnValidSubmit="HandlValidSubmit">
    <AutoComplete SearchMethod="SearchFilms"
                            @bind-Value="MyFormModel.SelectedFilm">
        <SelectedTemplate>
            @context.Title
        </SelectedTemplate>
        <ResultTemplate>
            @context.Title (@context.Year)
        </ResultTemplate>
    </AutoComplete>
    <ValidationMessage For="@(() => MyFormModel.SelectedFilm)" />
</EditForm>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        return await Task.FromResult(Films.Where(x => x.Title.ToLower().Contains(searchText.ToLower())).ToList());
    }

}
```

In the example above, the component is setup with the minimum requirements.
You must provide a method which has the following signature `Task<IEnumerable<T> MethodName(string searchText)`, 
to the `SearchMethod` parameter. The control will call this method with the current search text everytime the 
debounce timer expires (default: 300ms). You must also set a value for the `Value` parameter. 
This will be populated with the item selected from the search results. 
As this version of the control is integrated with Blazors built-in forms and validation, it must be wrapped in a `EditForm` component.

The component requires two templates to be provided:

- `SelectedTemplate`
- `ResultTemplates`

The `SelectedTemplate` is used to display the selected item and the `ResultTemplate` is used to display each result in the search list.

### Remote Data Example

```cs
@inject HttpClient httpClient

<BlazoredAutoComplete SearchMethod="@SearchFilms"
                      @bind-Value="@SelectedFilm"
                      Debounce="500">
    <SelectedTemplate>
        @context.Title
    </SelectedTemplate>
    <ResultTemplate>
        @context.Title (@context.Year)
    </ResultTemplate>
    <NotFoundTemplate>
        Sorry, there weren't any search results.
    </NotFoundTemplate>
</BlazoredAutoComplete>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        var response = await httpClient.GetJsonAsync<IEnumerable<Film>>($"https://allfilms.com/api/films/?title={searchText}");
        return response;
    }

}
```

Because you provide the search method to the component, making a remote call is really straight-forward. 
In this example, the `Debounce` parameter has been upped to 500ms and the `NotFoundTemplate` has been specified.

### Subscribing to changes in selected values
It is common to want to be able to know when a value bound to the AutoComplete changes. 
To do this you can't use the standard `@bind-Value` or `@bind-Values` syntax, you must handle the change event manually. 
To do this you must specify the following parameters:

- Value
- ValueChanged
- ValueExpression
- TValue & TItem (these are not always necessary)

The code below shows an example of how these parameters should be used.

```razor
<AutoComplete SearchMethod="SearchPeople"
    TValue="Result"
    TItem="Result"
    Value="selectedResult"
    ValueChanged="SelectedResultChanged" 
    ValueExpression="@(() => selectedResult)"
    placeholder="Search by name...">
</AutoComplete>

@code {
    private MovieCredits movieCredits;
    private Result selectedResult;

    private async Task<IEnumerable<Result>> SearchPeople(string searchText)
    {
        var search = await client.SearchPerson(searchText);
        return search.Results;
    }

    private async Task SelectedResultChanged(Result result)
    {
        selectedResult = result;
        movieCredits = await client.GetPersonMovieCredits(result.Id);
    }
}
```

### Using complex types but only binding to a single property
There are times when you will want to use complex types with the AutoComplete but only bind a certain property of that type. For example, you may want to search against a `Person` but once a person is selected, only bind to it's `Id` property. In order to do this you will need to implement the following:

```razor
<BlazoredAutoComplete SearchMethod="GetPeopleLocal"
                      ConvertMethod="ConvertPerson"
                      @bind-Value="SelectedPersonId"
                      placeholder="Search by first name...">
    <SelectedTemplate Context="personId">
        @{
            var selectedPerson = LoadSelectedPerson(personId);

            <text>@selectedPerson?.Firstname @selectedPerson?.Lastname</text>
        }
    </SelectedTemplate>
    <ResultTemplate Context="person">
        @person.Firstname @person.Lastname (Id: @person.Id)
    </ResultTemplate>
</BlazoredAutoComplete>

@code {
    private List<Person> People = new List<Person>();

    protected override void OnInitialized()
    {
        People.AddRange(new List<Person>() {
            new Person() { Id = 1, Firstname = "Martelle", Lastname = "Cullon" },
            new Person() { Id = 2, Firstname = "Zelda", Lastname = "Abrahamsson" },
            new Person() { Id = 3, Firstname = "Benedetta", Lastname = "Posse" }
        });
    }

    private async Task<IEnumerable<Person>> GetPeopleLocal(string searchText)
    {
        return await Task.FromResult(People.Where(x => x.Firstname.ToLower().Contains(searchText.ToLower())).ToList());
    }

    private int? ConvertPerson(Person person) => person?.Id;

    private Person LoadSelectedPerson(int? id) => People.FirstOrDefault(p => p.Id == id);
}
```

---
    
## PureSourceCode.com

[PureSourceCode.com](https://www.puresourcecode.com/) is my personal blog where I publish posts about technologies and in particular source code and projects in [.NET](https://www.puresourcecode.com/category/dotnet/). 

In the last few months, I created a lot of components for [Blazor WebAssembly](https://www.puresourcecode.com/tag/blazor-webassembly/) and [Blazor Server](https://www.puresourcecode.com/tag/blazor-server/).

My name is Enrico Rossini and you can contact me via:
- [Personal Twitter](https://twitter.com/erossiniuk)
- [LinkedIn](https://www.linkedin.com/in/rossiniuk)
- [Facebook](https://www.facebook.com/puresourcecode)

## Blazor Components

| Component name | Forum | NuGet | Website | Description |
|---|---|---|---|---|
| [Browser Detect for Blazor](https://www.puresourcecode.com/dotnet/blazor/browser-detect-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/browser-detect-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.BrowserDetect) | [Demo](https://browserdetect.puresourcecode.com) | Browser detect for Blazor WebAssembly and Blazor Server |
| [ChartJs for Blazor](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/) | [Forum](https://www.puresourcecode.com/forum/chart-js-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Chartjs) | [Demo](https://chartjs.puresourcecode.com/) | Add beautiful graphs based on ChartJs in your Blazor application |
| [Clippy for Blazor](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/) | [Forum](https://www.puresourcecode.com/forum/clippy/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Clippy) | [Demo](https://clippy.puresourcecode.com/) | Do you miss Clippy? Here the implementation for Blazor |
| [CodeSnipper for Blazor](https://www.puresourcecode.com/dotnet/blazor/code-snippet-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/codesnippet-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.CodeSnippet) | | Add code snippet in your Blazor pages for 196 programming languages with 243 styles |
| [Copy To Clipboard](https://www.puresourcecode.com/dotnet/blazor/copy-to-clipboard-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/copytoclipboard/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.CopyToClipboard) | | Add a button to copy text in the clipboard | 
| [DataTable for Blazor](https://www.puresourcecode.com/dotnet/net-core/datatable-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/datatables/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.DataTable) | [Demo](https://datatable.puresourcecode.com/) | DataTable component for Blazor WebAssembly and Blazor Server |
| [Icons and flags for Blazor](https://www.puresourcecode.com/forum/icons-and-flags-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/icons-and-flags-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Icons) | | Library with a lot of SVG icons and SVG flags to use in your Razor pages |
| [Markdown editor for Blazor](https://www.puresourcecode.com/dotnet/blazor/markdown-editor-with-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/markdown-editor-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.MarkdownEditor) | [Demo](https://markdown.puresourcecode.com/) | This is a Markdown Editor for use in Blazor. It contains a live preview as well as an embeded help guide for users. |
| [Modal dialog for Blazor](https://www.puresourcecode.com/dotnet/blazor/modal-dialog-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/modal-dialog-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.ModalDialog) | | Simple Modal Dialog for Blazor WebAssembly |
| [Modal windows for Blazor](https://www.puresourcecode.com/dotnet/blazor/modal-dialog-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/modal-dialog-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Modals) | | Modal Windows for Blazor WebAssembly |
| [Quill for Blazor](https://www.puresourcecode.com/dotnet/blazor/create-a-blazor-component-for-quill/) | [Forum](https://www.puresourcecode.com/forum/forum/quill-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Quill) | | Quill Component is a custom reusable control that allows us to easily consume Quill and place multiple instances of it on a single page in our Blazor application |
| [ScrollTabs](https://www.puresourcecode.com/dotnet/blazor/scrolltabs-component-for-blazor/) | | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.ScrollTabs) | | Tabs with nice scroll (no scrollbar) and responsive |
| [Segment for Blazor](https://www.puresourcecode.com/dotnet/blazor/segment-control-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/segments-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Segments) | | This is a Segment component for Blazor Web Assembly and Blazor Server |
| [Tabs for Blazor](https://www.puresourcecode.com/dotnet/blazor/tabs-control-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/forum/tabs-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Tabs) | | This is a Tabs component for Blazor Web Assembly and Blazor Server |
| [Timeline for Blazor](https://www.puresourcecode.com/dotnet/blazor/timeline-component-for-blazor/) | [Forum](https://www.puresourcecode.com/forum/timeline/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Timeline) | | This is a new responsive timeline  for Blazor Web Assembly and Blazor Server |
| [Toast for Blazor](https://www.puresourcecode.com/forum/psc-components-and-source-code/) | [Forum](https://www.puresourcecode.com/forum/psc-components-and-source-code/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Toast) | | Toast notification for Blazor applications |
| [Tours for Blazor](https://www.puresourcecode.com/forum/psc-components-and-source-code/) | [Forum](https://www.puresourcecode.com/forum/psc-components-and-source-code/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.Tours) | | Guide your users in your Blazor applications |
| [WorldMap for Blazor]() | [Forum](https://www.puresourcecode.com/forum/worldmap-for-blazor/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Blazor.Components.WorldMap) | [Demo](https://worldmap.puresourcecode.com/) | Show world maps with your data |

## C# libraries for .NET6

| Component name | Forum | NuGet | Description |
|---|---|---|---|
| [PSC.Evaluator](https://www.puresourcecode.com/forum/psc-components-and-source-code/) | [Forum](https://www.puresourcecode.com/forum/forum/psc-extensions/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Evaluator) | PSC.Evaluator is a mathematical expressions evaluator library written in C#. Allows to evaluate mathematical, boolean, string and datetime expressions. |
| [PSC.Extensions](https://www.puresourcecode.com/dotnet/net-core/a-lot-of-functions-for-net5/) | [Forum](https://www.puresourcecode.com/forum/forum/psc-extensions/) | ![NuGet badge](https://img.shields.io/nuget/v/PSC.Extensions) | A lot of functions for .NET5 in a NuGet package that you can download for free. We collected in this package functions for everyday work to help you with claim, strings, enums, date and time, expressions... |

## More examples and documentation
*   [Write a reusable Blazor component](https://www.puresourcecode.com/dotnet/blazor/write-a-reusable-blazor-component/)
*   [Getting Started With C# And Blazor](https://www.puresourcecode.com/dotnet/net-core/getting-started-with-c-and-blazor/)
*   [Setting Up A Blazor WebAssembly Application](https://www.puresourcecode.com/dotnet/blazor/setting-up-a-blazor-webassembly-application/)
*   [Working With Blazor Component Model](https://www.puresourcecode.com/dotnet/blazor/working-with-blazors-component-model/)
*   [Secure Blazor WebAssembly With IdentityServer4](https://www.puresourcecode.com/dotnet/blazor/secure-blazor-webassembly-with-identityserver4/)
*   [Blazor Using HttpClient With Authentication](https://www.puresourcecode.com/dotnet/blazor/blazor-using-httpclient-with-authentication/)
*   [InputSelect component for enumerations in Blazor](https://www.puresourcecode.com/dotnet/blazor/inputselect-component-for-enumerations-in-blazor/)
*   [Use LocalStorage with Blazor WebAssembly](https://www.puresourcecode.com/dotnet/blazor/use-localstorage-with-blazor-webassembly/)
*   [Modal Dialog component for Blazor](https://www.puresourcecode.com/dotnet/blazor/modal-dialog-component-for-blazor/)
*   [Create Tooltip component for Blazor](https://www.puresourcecode.com/dotnet/blazor/create-tooltip-component-for-blazor/)
*   [Consume ASP.NET Core Razor components from Razor class libraries | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries?view=aspnetcore-5.0&tabs=visual-studio)
