using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Timers;

namespace PSC.Blazor.Components.AutoComplete
{
    /// <summary>
    /// Class AutoComplete.
    /// Implements the <see cref="ComponentBase" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <typeparam name="TItem">The type of the t item.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="ComponentBase" />
    /// <seealso cref="System.IDisposable" />
    public partial class Autocomplete<TItem, TValue> : ComponentBase, IDisposable
    {
        #region Variables

        /// <summary>
        /// The debounce timer
        /// </summary>
        private System.Timers.Timer _debounceTimer;

        /// <summary>
        /// The edit context
        /// </summary>
        private EditContext _editContext;

        /// <summary>
        /// The events hooked up
        /// </summary>
        private bool _eventsHookedUp = false;

        /// <summary>
        /// The field identifier
        /// </summary>
        private FieldIdentifier _fieldIdentifier;

        /// <summary>
        /// The mask
        /// </summary>
        private ElementReference _mask;

        /// <summary>
        /// The resetting control
        /// </summary>
        private bool _resettingControl = false;

        /// <summary>
        /// The search input
        /// </summary>
        private ElementReference _searchInput;

        /// <summary>
        /// The search text
        /// </summary>
        private string _searchText = string.Empty;

        #endregion Variables

        #region Inject

        /// <summary>
        /// Gets or sets the cascaded edit context.
        /// </summary>
        /// <value>The cascaded edit context.</value>
        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        /// <summary>
        /// Gets or sets the js runtime.
        /// </summary>
        /// <value>The js runtime.</value>
        [Inject] private IJSRuntime JSRuntime { get; set; }

        #endregion Inject

        #region Parameters

        /// <summary>
        /// Gets or sets the add item on empty result method.
        /// </summary>
        /// <value>The add item on empty result method.</value>
        [Parameter] public Func<string, Task<TItem>> AddItemOnEmptyResultMethod { get; set; }

        /// <summary>
        /// Gets or sets the additional attributes.
        /// </summary>
        /// <value>The additional attributes.</value>
        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets or sets the convert method.
        /// </summary>
        /// <value>The convert method.</value>
        [Parameter] public Func<TItem, TValue> ConvertMethod { get; set; }

        /// <summary>
        /// Gets or sets the debounce.
        /// </summary>
        /// <value>The debounce.</value>
        [Parameter] public int Debounce { get; set; } = 300;

        /// <summary>
        /// Gets or sets a value indicating whether [disable clear].
        /// </summary>
        /// <value><c>true</c> if [disable clear]; otherwise, <c>false</c>.</value>
        [Parameter] public bool DisableClear { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Autocomplete{TItem, TValue}"/> is disabled.
        /// </summary>
        /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
        [Parameter] public bool Disabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [enable drop down].
        /// </summary>
        /// <value><c>true</c> if [enable drop down]; otherwise, <c>false</c>.</value>
        [Parameter] public bool EnableDropDown { get; set; } = false;

        /// <summary>
        /// Gets or sets the footer template.
        /// </summary>
        /// <value>The footer template.</value>
        [Parameter] public RenderFragment? FooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        [Parameter] public RenderFragment? HeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the help template.
        /// </summary>
        /// <value>The help template.</value>
        [Parameter] public RenderFragment? HelpTemplate { get; set; }

        /// <summary>
        /// Gets or sets the maximum suggestions.
        /// </summary>
        /// <value>The maximum suggestions.</value>
        [Parameter] public int MaximumSuggestions { get; set; } = 10;

        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>The minimum length.</value>
        [Parameter] public int MinimumLength { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Autocomplete{TItem, TValue}"/> is multiselect. 
        /// This will be combined with <seealso cref="ValueExpression"/>
        /// </summary>
        /// <value><c>null</c> if the component can accept multiple selection, <c>true</c> if [multiselect]; otherwise, <c>false</c>.</value>
        [Parameter] public bool? Multiselect { get; set; }

        /// <summary>
        /// Gets or sets the not found template.
        /// </summary>
        /// <value>The not found template.</value>
        [Parameter] public RenderFragment<string>? NotFoundTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [prevent default].
        /// </summary>
        /// <value><c>true</c> if [prevent default]; otherwise, <c>false</c>.</value>
        [Parameter] public bool PreventDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets the result container CSS.
        /// </summary>
        /// <value>The result container CSS.</value>
        [Parameter] public string? ResultContainerCSS { get; set; }

        /// <summary>
        /// Gets or sets the result item CSS.
        /// </summary>
        /// <value>The result item CSS.</value>
        [Parameter] public string? ResultItemCSS { get; set; }

        /// <summary>
        /// Gets or sets the result template.
        /// </summary>
        /// <value>The result template.</value>
        [Parameter] public RenderFragment<TItem>? ResultTemplate { get; set; }

        /// <summary>
        /// Gets or sets the search method.
        /// </summary>
        /// <value>The search method.</value>
        [Parameter] public Func<string, Task<IEnumerable<TItem>>>? SearchMethod { get; set; }

        /// <summary>
        /// Gets or sets the selected template.
        /// </summary>
        /// <value>The selected template.</value>
        [Parameter] public RenderFragment<TValue>? SelectedTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show drop down on focus].
        /// </summary>
        /// <value><c>true</c> if [show drop down on focus]; otherwise, <c>false</c>.</value>
        [Parameter] public bool ShowDropDownOnFocus { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [stop propagation].
        /// </summary>
        /// <value><c>true</c> if [stop propagation]; otherwise, <c>false</c>.</value>
        [Parameter] public bool StopPropagation { get; set; } = false;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Parameter] public TValue? Value { get; set; }

        /// <summary>
        /// Gets or sets the value changed.
        /// </summary>
        /// <value>The value changed.</value>
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets the value expression to determine if the multiselect is allow. 
        /// This is combined with <seealso cref="Multiselect"/>
        /// </summary>
        /// <value>The value expression.</value>
        [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        [Parameter] public IList<TValue>? Values { get; set; }

        /// <summary>
        /// Gets or sets the values changed.
        /// </summary>
        /// <value>The values changed.</value>
        [Parameter] public EventCallback<IList<TValue>> ValuesChanged { get; set; }

        /// <summary>
        /// Gets or sets the values expression.
        /// </summary>
        /// <value>The values expression.</value>
        [Parameter] public Expression<Func<IList<TValue>>> ValuesExpression { get; set; }

        #endregion Variables

        #region Properties

        /// <summary>
        /// Gets the field CSS classes.
        /// </summary>
        /// <value>The field CSS classes.</value>
        private string FieldCssClasses => _editContext?.FieldCssClass(_fieldIdentifier) ?? "";

        /// <summary>
        /// Gets a value indicating whether this instance is multiselect.
        /// </summary>
        /// <value><c>true</c> if this instance is multiselect; otherwise, <c>false</c>.</value>
        private bool IsMultiselect => Multiselect == null ? ValuesExpression != null : (bool)Multiselect;

        private bool IsSearching { get; set; } = false;
        private bool IsShowingMask { get; set; } = false;
        private bool IsShowingSuggestions { get; set; } = false;

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>The search text.</value>
        private string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                    SelectedIndex = -1;
                }
                else
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
        }

        private int SelectedIndex { get; set; }
        private bool ShowHelpTemplate { get; set; } = false;
        private TItem[] Suggestions { get; set; } = new TItem[0];

        #endregion Properties

        /// <summary>
        /// Gets a value indicating whether this instance is searching or debouncing.
        /// </summary>
        /// <value><c>true</c> if this instance is searching or debouncing; otherwise, <c>false</c>.</value>
        private bool IsSearchingOrDebouncing => IsSearching || _debounceTimer.Enabled;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Focuses this instance.
        /// </summary>
        public async Task Focus()
        {
            await HandleClickOnMask();
        }

        /// <summary>
        /// Resets the control blur.
        /// </summary>
        [JSInvokable("ResetControlBlur")]
        public async Task ResetControlBlur()
        {
            await ResetControl();
            StateHasChanged();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer.Dispose();
            }
        }

        /// <summary>
        /// On after render as an asynchronous operation.
        /// </summary>
        /// <param name="firstRender">Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.</remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if ((firstRender && !Disabled) || (!_eventsHookedUp && !Disabled))
            {
                await AutocompleteInterop.AddKeyDownEventListener(JSRuntime, _searchInput);
                _eventsHookedUp = true;
            }
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected override void OnInitialized()
        {
            if (SearchMethod == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(SearchMethod)} parameter.");
            }

            if (ConvertMethod == null)
            {
                if (typeof(TItem) != typeof(TValue))
                {
                    throw new InvalidOperationException($"{GetType()} requires a {nameof(ConvertMethod)} parameter.");
                }

                ConvertMethod = item => item is TValue value ? value : default;
            }

            if (SelectedTemplate == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(SelectedTemplate)} parameter.");
            }

            if (ResultTemplate == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(ResultTemplate)} parameter.");
            }

            _debounceTimer = new System.Timers.Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;

            _editContext = CascadedEditContext;
            _fieldIdentifier = IsMultiselect ? FieldIdentifier.Create(ValuesExpression) : FieldIdentifier.Create(ValueExpression);

            Initialize();
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the selected suggestion class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        private string GetSelectedSuggestionClass(TItem item, int index)
        {
            const string resultClass = "autocomplete__active-item";
            TValue value = ConvertMethod(item);

            if (Equals(value, Value) || (Values?.Contains(value) ?? false))
            {
                if (index == SelectedIndex)
                {
                    return "autocomplete__selected-item-highlighted";
                }

                return "autocomplete__selected-item";
            }

            if (index == SelectedIndex)
            {
                return resultClass;
            }

            return Equals(value, Value) ? resultClass : string.Empty;
        }

        /// <summary>
        /// Gets the selected suggestion class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        private string GetSelectedSuggestionClass(int index)
        {
            const string resultClass = "autocomplete__active-item";

            return index == SelectedIndex ? resultClass : string.Empty;
        }

        /// <summary>
        /// Handles the clear.
        /// </summary>
        private async Task HandleClear()
        {
            SearchText = "";
            IsShowingMask = false;

            if (IsMultiselect)
            {
                await ValuesChanged.InvokeAsync(new List<TValue>());
            }
            else
            {
                await ValueChanged.InvokeAsync(default);
            }

            _editContext?.NotifyFieldChanged(_fieldIdentifier);

            await Task.Delay(250); // Possible race condition here.
            await AutocompleteInterop.Focus(JSRuntime, _searchInput);
        }

        /// <summary>
        /// Handles the click on mask.
        /// </summary>
        private async Task HandleClickOnMask()
        {
            SearchText = "";
            IsShowingMask = false;

            await Task.Delay(250); // Possible race condition here.
            await AutocompleteInterop.Focus(JSRuntime, _searchInput);
            await HookOutsideClick();
        }

        /// <summary>
        /// Handles the input focus.
        /// </summary>
        private async Task HandleInputFocus()
        {
            if (ShowDropDownOnFocus)
            {
                await ShowMaximumSuggestions();
            }
        }

        /// <summary>
        /// Handles the keydown.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        private async Task HandleKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Tab")
            {
                await ResetControl();
            }
        }

        /// <summary>
        /// Handles the keyup.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        private async Task HandleKeyup(KeyboardEventArgs args)
        {
            if ((args.Key == "ArrowDown" || args.Key == "Enter") && EnableDropDown && !IsShowingSuggestions)
            {
                await ShowMaximumSuggestions();
            }

            if (args.Key == "ArrowDown")
            {
                MoveSelection(1);
            }
            else if (args.Key == "ArrowUp")
            {
                MoveSelection(-1);
            }
            else if (args.Key == "Escape")
            {
                Initialize();
            }
            else if (args.Key == "Enter" && Suggestions.Count() == 1)
            {
                await SelectTheFirstAndOnlySuggestion();
            }
            else if (args.Key == "Enter" && ShowNotFound() && AddItemOnEmptyResultMethod != null)
            {
                await SelectNotFoundPlaceholder();
            }
            else if (args.Key == "Enter" && SelectedIndex >= 0 && SelectedIndex < Suggestions.Count())
            {
                await SelectResult(Suggestions[SelectedIndex]);
            }
            else if (IsMultiselect && !IsShowingSuggestions && args.Key == "Backspace")
            {
                if (Values.Any())
                    await RemoveValue(Values.Last());
            }
        }

        /// <summary>
        /// Handles the key up on mask.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        private async Task HandleKeyUpOnMask(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Tab":
                    break; // Don't do anything on tab.
                case "Enter":
                case "Backspace":
                case "Delete":
                case "Escape":
                    await HandleClear();
                    break;

                default:
                    break;
            }

            // You can only start searching if it's not a special key (Tab, Enter, Escape, ...)
            if (args.Key.Length == 1)
            {
                IsShowingMask = false;
                await Task.Delay(250); // Possible race condition here.
                await AutocompleteInterop.Focus(JSRuntime, _searchInput);
                SearchText = args.Key;
            }
        }

        /// <summary>
        /// Handles the key up on show drop down.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        private async Task HandleKeyUpOnShowDropDown(KeyboardEventArgs args)
        {
            if (args.Key == "ArrowDown")
            {
                MoveSelection(1);
            }
            else if (args.Key == "ArrowUp")
            {
                MoveSelection(-1);
            }
            else if (args.Key == "Escape")
            {
                Initialize();
            }
            else if (args.Key == "Enter" && Suggestions.Length == 1)
            {
                await SelectTheFirstAndOnlySuggestion();
            }
            else if (args.Key == "Enter" && SelectedIndex >= 0 && SelectedIndex < Suggestions.Length)
            {
                await SelectResult(Suggestions[SelectedIndex]);
            }
            else if (args.Key == "Enter")
            {
                await ShowMaximumSuggestions();
            }
        }

        /// <summary>
        /// Hooks the outside click.
        /// </summary>
        private async Task HookOutsideClick()
        {
            await JSRuntime.OnOutsideClick(_searchInput, this, "ResetControlBlur", true);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            SearchText = "";
            IsShowingSuggestions = false;
            IsShowingMask = Value != null;
        }

        /// <summary>
        /// Moves the selection.
        /// </summary>
        /// <param name="count">The count.</param>
        private void MoveSelection(int count)
        {
            var index = SelectedIndex + count;

            if (index >= Suggestions.Length)
            {
                index = 0;
            }

            if (index < 0)
            {
                index = Suggestions.Length - 1;
            }

            SelectedIndex = index;
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="item">The item.</param>
        private async Task RemoveValue(TValue item)
        {
            var valueList = Values ?? new List<TValue>();
            if (valueList.Contains(item))
            {
                valueList.Remove(item);
            }

            await ValuesChanged.InvokeAsync(valueList);
            _editContext?.NotifyFieldChanged(_fieldIdentifier);
        }

        /// <summary>
        /// Resets the control.
        /// </summary>
        private async Task ResetControl()
        {
            if (!_resettingControl)
            {
                _resettingControl = true;
                Initialize();
                _resettingControl = false;
            }

            if (IsMultiselect)
            {
                await ValuesChanged.InvokeAsync(Values);
                _editContext?.NotifyFieldChanged(_fieldIdentifier);
            }
        }

        /// <summary>
        /// Searches the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private async void Search(Object source, ElapsedEventArgs e)
        {
            if (_searchText.Length < MinimumLength)
            {
                ShowHelpTemplate = true;
                await InvokeAsync(StateHasChanged);
                return;
            }

            ShowHelpTemplate = false;
            IsSearching = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                Suggestions = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();
            }
            catch (Exception ex) { }

            IsSearching = false;
            IsShowingSuggestions = true;
            await HookOutsideClick();

            SelectedIndex = 0;
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Selects the not found placeholder.
        /// </summary>
        private async Task SelectNotFoundPlaceholder()
        {
            try
            {
                // Potentially dangerous code
                var item = await AddItemOnEmptyResultMethod(SearchText);
                await SelectResult(item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Selects the result.
        /// </summary>
        /// <param name="item">The item.</param>
        private async Task SelectResult(TItem item)
        {
            var value = ConvertMethod(item);

            if (IsMultiselect)
            {
                var valueList = Values ?? new List<TValue>();

                if (valueList.Contains(value))
                    valueList.Remove(value);
                else
                    valueList.Add(value);

                await ValuesChanged.InvokeAsync(valueList);
            }
            else
            {
                if (Value != null && Value.Equals(value)) return;
                Value = value;
                await ValueChanged.InvokeAsync(value);
            }

            _editContext?.NotifyFieldChanged(_fieldIdentifier);

            Initialize();
        }

        /// <summary>
        /// Selects the first and only suggestion.
        /// </summary>
        /// <returns>Task.</returns>
        private Task SelectTheFirstAndOnlySuggestion()
        {
            SelectedIndex = 0;
            return SelectResult(Suggestions[SelectedIndex]);
        }

        /// <summary>
        /// Shoulds the show help template.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ShouldShowHelpTemplate()
        {
            return SearchText.Length > 0 &&
                ShowHelpTemplate &&
                HelpTemplate != null;
        }

        /// <summary>
        /// Shoulds the show suggestions.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ShouldShowSuggestions()
        {
            return IsShowingSuggestions &&
                   Suggestions.Any() && !IsSearchingOrDebouncing;
        }

        /// <summary>
        /// Shows the maximum suggestions.
        /// </summary>
        private async Task ShowMaximumSuggestions()
        {
            if (_resettingControl)
            {
                while (_resettingControl)
                {
                    await Task.Delay(150);
                }
            }

            IsShowingSuggestions = !IsShowingSuggestions;

            if (IsShowingSuggestions)
            {
                SearchText = "";
                IsSearching = true;
                await InvokeAsync(StateHasChanged);

                Suggestions = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

                IsSearching = false;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await ResetControlBlur();
            }
            await HookOutsideClick();
        }

        /// <summary>
        /// Shows the not found.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ShowNotFound()
        {
            return IsShowingSuggestions &&
                   !IsSearchingOrDebouncing &&
                   !Suggestions.Any();
        }
    }
}