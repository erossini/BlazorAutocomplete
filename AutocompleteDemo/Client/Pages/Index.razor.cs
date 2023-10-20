using AutocompleteDemo.Client.Services;
using AutocompleteDemo.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace AutocompleteDemo.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public PeopleService? _service { get; set; }

        private bool _isDisabledMulti;
        private IList<Person>? _selectedPeopleWithNotFoundTemplate;

        private async Task<IEnumerable<Person>> GetPeopleLocal(string searchText) => await _service.GetPeopleLocal(searchText);
        private async Task<Person> ItemAddedMethod(string searchText) => await _service.ItemAddedMethod(searchText);
    }
}