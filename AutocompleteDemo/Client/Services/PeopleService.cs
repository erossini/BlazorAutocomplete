using AutocompleteDemo.Shared.Models;

namespace AutocompleteDemo.Client.Services
{
    public class PeopleService
    {
        private readonly List<Person> _people = new();

        public PeopleService()
        {
            _people.AddRange(new List<Person>
            {
                new() { Id = 1, Firstname = "Martelle", Lastname = "Cullon" },
                new() { Id = 2, Firstname = "Zelda", Lastname = "Abrahamsson" },
                new() { Id = 3, Firstname = "Benedetta", Lastname = "Posse" },
                new() { Id = 4, Firstname = "Benoite", Lastname = "Gobel" },
                new() { Id = 5, Firstname = "Charlot", Lastname = "Fullicks" },
                new() { Id = 6, Firstname = "Vinson", Lastname = "Turbat" },
                new() { Id = 7, Firstname = "Lenore", Lastname = "Malam" },
                new() { Id = 8, Firstname = "Emanuele", Lastname = "Kolakovic" },
                new() { Id = 9, Firstname = "Rosalyn", Lastname = "Mackin" },
                new() { Id = 10, Firstname = "Yanaton", Lastname = "Krishtopaittis" },
                new() { Id = 11, Firstname = "Frederik", Lastname = "McGeachie" },
                new() { Id = 12, Firstname = "Parrnell", Lastname = "Ramsby" },
                new() { Id = 13, Firstname = "Coreen", Lastname = "McGann" },
                new() { Id = 14, Firstname = "Kyle", Lastname = "Coster" },
                new() { Id = 15, Firstname = "Evangelia", Lastname = "Bowker" },
                new() { Id = 16, Firstname = "Angeli", Lastname = "Collihole" },
                new() { Id = 17, Firstname = "Bill", Lastname = "Lawther" },
                new() { Id = 18, Firstname = "Kore", Lastname = "Reide" },
                new() { Id = 19, Firstname = "Tracy", Lastname = "Gwinnell" },
                new() { Id = 20, Firstname = "Lazaro", Lastname = "Partington" },
                new() { Id = 21, Firstname = "Doretta", Lastname = "Aingell" },
                new() { Id = 22, Firstname = "Olvan", Lastname = "Andraud" },
                new() { Id = 23, Firstname = "Templeton", Lastname = "Chetwynd" },
                new() { Id = 24, Firstname = "Daile", Lastname = "Kelsow" },
                new() { Id = 25, Firstname = "Marcie", Lastname = "Brearty" },
                new() { Id = 26, Firstname = "Irwinn", Lastname = "Lilian" },
                new() { Id = 27, Firstname = "Niki", Lastname = "Moreland" },
                new() { Id = 28, Firstname = "Honey", Lastname = "Waddup" },
                new() { Id = 29, Firstname = "Amber", Lastname = "Hoopper" },
                new() { Id = 30, Firstname = "Delilah", Lastname = "Dougary" },
                new() { Id = 31, Firstname = "Tory", Lastname = "Ovington" },
                new() { Id = 32, Firstname = "Doralin", Lastname = "Conrard" },
                new() { Id = 33, Firstname = "Eugene", Lastname = "Custard" },
                new() { Id = 34, Firstname = "Corella", Lastname = "Peotz" },
                new() { Id = 35, Firstname = "Chris", Lastname = "Rayne" },
                new() { Id = 36, Firstname = "Alexandro", Lastname = "Kwietek" },
                new() { Id = 37, Firstname = "Selie", Lastname = "Tenwick" },
                new() { Id = 38, Firstname = "Corliss", Lastname = "Haensel" },
                new() { Id = 39, Firstname = "Misti", Lastname = "Jikylls" },
                new() { Id = 40, Firstname = "Rosaline", Lastname = "Jephson" },
                new() { Id = 41, Firstname = "Irene", Lastname = "Farnsworth" },
                new() { Id = 42, Firstname = "Dominique", Lastname = "O'Shiels" },
                new() { Id = 43, Firstname = "Mellie", Lastname = "Cyson" },
                new() { Id = 44, Firstname = "Madelena", Lastname = "Chin" },
                new() { Id = 45, Firstname = "Charlotte", Lastname = "Clixby" },
                new() { Id = 46, Firstname = "Samara", Lastname = "Shavel" },
                new() { Id = 47, Firstname = "Brod", Lastname = "Kitt" },
                new() { Id = 48, Firstname = "Maridel", Lastname = "Dalley" },
                new() { Id = 49, Firstname = "Wini", Lastname = "Hundley" }
            });
        }

        /// <summary>
        /// Gets the people local.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>IEnumerable&lt;Person&gt;.</returns>
        public async Task<IEnumerable<Person>> GetPeopleLocal(string searchText)
                     => await Task.FromResult(_people.Where(
                         x => x.Firstname.ToLower().Contains(searchText.ToLower())).ToList());

        /// <summary>
        /// Loads the selected person.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Nullable&lt;Person&gt;.</returns>
        public Person? LoadSelectedPerson(int? id) => _people.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// The random
        /// </summary>
        public readonly Random _random = new();

        /// <summary>
        /// Items the added method.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>Task&lt;Person&gt;.</returns>
        public Task<Person> ItemAddedMethod(string searchText)
        {
            var randomPerson = _people[_random.Next(_people.Count - 1)];
            var newPerson = new Person(
                _random.Next(1000, int.MaxValue),
                searchText,
                randomPerson.Lastname,
                _random.Next(10, 70),
                randomPerson.Location);
            _people.Add(newPerson);

            return Task.FromResult(newPerson);
        }
    }
}
