using AutocompleteDemo.Shared.Models;

namespace AutocompleteDemo.Client.Services
{
    public class LanguageService
    {
        private readonly List<Tag> _tag = new();

        public LanguageService()
        {
            _tag.AddRange(new List<Tag>
            {
                new() { Id = 1, TagCount = 100, TagName = "c#",                     TagDescription = "C# (pronounced \"see sharp\") is a high-level, statically typed, multi-paradigm programming language developed by Microsoft. C# code usually targets Microsoft's .NET family of tools and run-times, which include .NET, .NET Framework, .NET MAUI, and Xamarin among others. Use this tag for questions about code written in C# or about C#'s formal specification." },
                new() { Id = 2, TagCount = 10,  TagName = "c#-2.0",                 TagDescription = "For issues unique to development in C#, version 2.0" },
                new() { Id = 3, TagCount = 90,  TagName = "azure",                  TagDescription = "Microsoft Azure is a Platform as a Service and Infrastructure as a Service cloud computing platform. Use this tag for programming questions concerning Azure. General server help can be obtained at Super User or Server Fault." },
                new() { Id = 4, TagCount = 90,  TagName = "azure-devops",           TagDescription = "Azure DevOps is a suite of 5 services you use together or independently. For example, Azure Pipelines provides build services (CI) as well as release management for continuous delivery (CD) to any cloud and on-premises servers. Azure Repos provides unlimited private Git hosting, Azure Boards provides agile planning (issues, Kanban, Scrum, and dashboards).\r\nPlease note that there's a separate tag for Azure DevOps Server (formerly TFS) - the on-prem version." },
                new() { Id = 5, TagCount = 90,  TagName = "azure-active-directory", TagDescription = "Microsoft Azure Active Directory (Microsoft Azure AD) is a modern developer platform and IAM service that provides identity management and access control capabilities for your cloud applications. It uses industry standard protocols like OAuth2.0, OpenId Connect, and SAML2.0. " },
                new() { Id = 6, TagCount = 90,  TagName = "azure-functions",        TagDescription = "Azure Functions is an event-driven serverless compute platform in Azure and Azure Stack. Its open-source runtime also works on multiple destinations including Kubernetes, Azure IoT Edge, on-premises, and other clouds." },
                new() { Id = 7, TagCount = 90,  TagName = "azure-web-app-service",  TagDescription = "Use this tag for questions relating to web applications residing on Azure. Azure App Service is a cloud-based platform for hosting web applications, REST APIs, and mobile back ends. It was formerly known as Azure Web Sites. You can deploy applications written in a variety of languages: .NET, .NET Core, Java, Ruby on Rails, Node.js, PHP and Python, using either a Windows or Linux container." },
                new() { Id = 8, TagCount = 70,  TagName = "javascript",             TagDescription = "For questions about programming in ECMAScript (JavaScript/JS) and its different dialects/implementations (except for ActionScript). Keep in mind that JavaScript is NOT the same as Java! Include all labels that are relevant to your question; e.g., [node.js], [jQuery], [JSON], [ReactJS], [angular], [ember.js], [vue.js], [typescript], [svelte], etc." },
                new() { Id = 9, TagCount = 14,  TagName = "java",                   TagDescription = "Java is a high-level object-oriented programming language. Use this tag when you're having problems using or understanding the language itself. This tag is frequently used alongside other tags for libraries and/or frameworks used by Java developers." },
                new() { Id = 10, TagCount = 9,  TagName = "azure-pipeline",         TagDescription = "Azure Pipelines provides build services (CI), that are free for open source projects and available in the GitHub marketplace. Azure Pipelines also provides release management for continuous delivery (CD) to any cloud and on-premises servers. With Azure Pipelines, you’ll be able to continuously build, test and deploy to any platform and cloud. **Do not** use this tag for Azure Data Factory pipeline questions." },
                new() { Id = 11, TagCount = 7,  TagName = "azure-devops-rest-api",  TagDescription = "Azure DevOps Services REST API (previously: Visual Studio Team Services REST APIs) is a set of APIs allowing management of Azure DevOps (formerly: Visual Studio Team Services) accounts as well as TFS 2015/2017/2018/2019 servers." },
                new() { Id = 12, TagCount = 3,  TagName = "azure-devops-extensions", TagDescription = "" }
            });
        }

        /// <summary>
        /// Gets the people local.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>IEnumerable&lt;Person&gt;.</returns>
        public async Task<IEnumerable<Tag>> GetPeopleLocal(string searchText)
                     => await Task.FromResult(_tag.Where(
                         x => x.TagName.ToLower().Contains(searchText.ToLower())).ToList());

        /// <summary>
        /// Loads the selected person.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Nullable&lt;Person&gt;.</returns>
        public Tag? LoadSelectedPerson(int? id) => _tag.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// The random
        /// </summary>
        public readonly Random _random = new();

        /// <summary>
        /// Items the added method.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>Task&lt;Person&gt;.</returns>
        public Task<Tag> ItemAddedMethod(string searchText)
        {
            var randomTag = _tag[_random.Next(_tag.Count - 1)];
            var newTag = new Tag(
                _random.Next(1000, int.MaxValue),
                searchText,
                "",
                _random.Next(10, 7000));
            _tag.Add(newTag);

            return Task.FromResult(newTag);
        }
    }
}
