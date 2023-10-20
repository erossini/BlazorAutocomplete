using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PSC.Blazor.Components.AutoComplete
{
    /// <summary>
    /// Class Interop.
    /// </summary>
    public static class AutocompleteInterop
    {
        /// <summary>
        /// Focuses the specified js runtime.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="element">The element.</param>
        /// <returns>ValueTask&lt;System.Object&gt;.</returns>
        internal static ValueTask<object> Focus(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<object>("autocomplete.setFocus", element);
        }

        /// <summary>
        /// Adds the key down event listener.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="element">The element.</param>
        /// <returns>ValueTask&lt;System.Object&gt;.</returns>
        internal static ValueTask<object> AddKeyDownEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<object>("autocomplete.addKeyDownEventListener", element);
        }

        /// <summary>
        /// Called when [outside click].
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="element">The element.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="clearOnFire">if set to <c>true</c> [clear on fire].</param>
        /// <returns>ValueTask&lt;System.Object&gt;.</returns>
        internal static ValueTask<object> OnOutsideClick(this IJSRuntime jsRuntime, ElementReference element, object caller, string methodName, bool clearOnFire = false)
        {
            return jsRuntime.InvokeAsync<object>("autocomplete.onOutsideClick",
                element, DotNetObjectReference.Create(caller), methodName, clearOnFire);
        }
    }
}