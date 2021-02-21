#define FUTURE
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SenseNet.Client;

namespace SnClientDotNetTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var url = "https://localhost:44305";
            //var url = "https://localhost:5001";
            var url = "https://localhost:44362";
            //var url = "https://dev.demo.sensenet.com";
            ClientContext.Current.AddServer(new ServerContext
            {
                Url = url,
                Username = "builtin\\admin",
                Password = "admin"
            });
            //ClientContext.Current.ChunkSizeInBytes = 1000;

            var builder = new VersioningForestBuilder();
            await builder.RunAsync().ConfigureAwait(false);
            return;


            //await EnsureBasicStructureAsync();

            //await BugReproduction_1292_broken_contenttypes();
            //await PlayLotOfVersionedDocsTestAsync();

            //await BasicConcepts();
            //await Querying();
            //await ContentManagement();
            //await Sharing();
            //await Preview();
            //await CollabVersioning();
            //await CollabApproval();
            //await CollabSavedQueries();
            //await UsersAndGroups();
            //await Permissions();
            //await PermissionManagement();
            //await PermissionQueries();
            //await UsersAndGroups_GroupMembership();
        }

        private static async Task EnsureBasicStructureAsync()
        {
            var c = await Content.LoadAsync("/Root/Content");
            if (c == null)
            {
                c = Content.CreateNew("/Root", "Folder", "Content");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/Content/IT");
            if (c == null)
            {
                c = Content.CreateNew("/Root/Content", "Workspace", "IT");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            if (c == null)
            {
                c = Content.CreateNew("/Root/Content/IT", "DocumentLibrary", "Document_Library");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/Content/IT/Document_Library/Calgary");
            if (c == null)
            {
                c = Content.CreateNew("/Root/Content/IT/Document_Library", "Folder", "Calgary");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx");
            if (c == null)
            {
                c = Content.CreateNew("/Root/Content/IT/Document_Library/Calgary", "File", "BusinessPlan.docx");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/IMS/Public");
            if (c == null)
            {
                c = Content.CreateNew("/Root/IMS", "Domain", "Public");
                await c.SaveAsync();
            }
            c = await Content.LoadAsync("/Root/IMS/Public/Editors");
            if (c == null)
            {
                c = Content.CreateNew("/Root/IMS/Public", "Group", "Editors");
                await c.SaveAsync();
            }
        }

        private static string LoremIpsum = @"";

        private static async Task PlayLotOfVersionedDocsTestAsync()
        {
            var c = await Content.LoadAsync("/Root/Content/IT/Document_Library/MyFile0.txt");
            if (c == null)
            {
                var content = await Content.LoadAsync("/Root/Content/IT");
                content["InheritableVersioningMode"] = new[] { 3 };
                await content.SaveAsync();


                for (int i = 0; i < 10; i++)
                {
                    await Content.UploadTextAsync("/Root/Content/IT/Document_Library", $"MyFile{i}.txt",
                        $"{LoremIpsum} #{i}", CancellationToken.None, "File");
                }

                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine("{0} {1}", j, i);
                        // Checkout a content for edit
                        var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
                        {
                            IsCollectionRequest = false,
                            Path = $"/Root/Content/IT/Document_Library/MyFile{i}.txt",
                            ActionName = "Checkout",
                        });

                        // Update metadata
                        content = await Content.LoadAsync($"/Root/Content/IT/Document_Library/MyFile{i}.txt");
                        content["Index"] = ((JValue)content["Index"]).Value<int>() + 1;
                        await content.SaveAsync();

                        // Checkin a content
                        result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
                        {
                            IsCollectionRequest = false,
                            Path = $"/Root/Content/IT/Document_Library/MyFile{i}.txt",
                            ActionName = "CheckIn",
                            //Parameters = { { "CheckInComments", "Adding new contract" } }
                        });
                    }
                }
            }
        }


        private static async Task BugReproduction_1292_broken_contenttypes()
        {
//            var stream1 = Tools.GenerateStreamFromString(@"<ContentType name=""MyType1"" parentType=""GenericContent"" handler=""SenseNet.ContentRepository.GenericContent"" xmlns=""http://schemas.sensenet.com/SenseNet/ContentRepository/ContentTypeDefinition"">
//<Fields>
//  <Field name=""Image2"" type=""Image""></Field>
//</Fields>
//</ContentType>");

//            var uploaded1 = await Content.UploadAsync("/Root/System/Schema/ContentTypes", "MyType1",
//                stream1, "ContentType");
//            Console.WriteLine($"Name: {uploaded1.Name}");

//            /* =============================================================================================  */

//            var stream2 = Tools.GenerateStreamFromString(@"<ContentType name=""MyType2"" parentType=""GenericContent"" handler=""SenseNet.ContentRepository.GenericContent"" xmlns=""http://schemas.sensenet.com/SenseNet/ContentRepository/ContentTypeDefinition"">
//<Fields>
//  <Field name=""Image2"" type=""ShortText""></Field>
//</Fields>
//</ContentType>");

//            var uploaded2 = await Content.UploadAsync("/Root/System/Schema/ContentTypes", "MyType2",
//                stream2, "ContentType");
//            Console.WriteLine($"Name: {uploaded2.Name}");
        }

        private static async Task BasicConcepts()
        {
            #region// ---- Entry
            //// Get a single content by Id
            //var content = await Content.LoadAsync(3);

            //// Get a single content by Path
            //var content = await Content.LoadAsync("/Root/IMS/BuiltIn");

            //// Addressing a single property of a content
            //var result = await RESTCaller.GetResponseStringAsync("/Root/IMS", "DisplayName");

            //// Addressing a property value
            //var result = await RESTCaller.GetResponseStringAsync(
            //    new Uri(url + "/OData.svc/Root('IMS')/DisplayName/$value"));

            //// Accessing binary stream
            //????

            #endregion
            //UNDONE:- Feature request: Content.GetPropertyAsync
            //UNDONE:- Feature request: Content.GetPropertyValueAsync
#if FUTURE
            //var result1 = await Content.GetPropertyAsync("/Root/IMS", "DisplayName");
            //var result2 = await Content.GetPropertyValueAsync("/Root/IMS", "DisplayName");
#endif

            #region// ---- Collection

            //// Children of a content (collection)
            //var entities = await Content.LoadCollectionAsync("/Root/IMS/BuiltIn");

            //// Count of a collection
            //await Content.GetCountAsync("/Root/IMS/BuiltIn/Portal", null);

            //// $inlinecount query option
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = true,
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Top = 3,
            //    Skip = 4,
            //    Parameters = { { "$inlinecount", "allpages" } }
            //});

            #endregion
            //UNDONE:- Feature request: should returns a collection with property: TotalCount
#if FUTURE
            //var result3 = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Top = 3,
            //    Skip = 4,
            //    InlineCount = InlineCountOptions.AllPages // Default, AllPages, None
            //});
#endif

            #region // ---- Select and expand

            //// Select
            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    //Select = new[] { "DisplayName", "Description" }
            //});
            //Console.WriteLine(content.DisplayName);

            //// Expand
            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    Expand = new[] { "CreatedBy" },
            //    Select = new[] { "Name", "CreatedBy/Name" }
            //});
            //Console.WriteLine(content.CreatedBy.Name);

            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    Expand = new[] { "CreatedBy/CreatedBy" },
            //    //Select = new[] { "Name", "CreatedBy/Name", "CreatedBy/CreatedBy/Name" }
            //});
            //Console.WriteLine(content.CreatedBy.CreatedBy.Name);

            //// Actions
            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    Expand = new[] { "Actions" },
            //    //Select = new[] { "Name", "CreatedBy/Name" }
            //});
            //Console.WriteLine(content.Actions.Count);

            //// AllowedChildTypes
            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/System/Schema/ContentTypes",
            //    Expand = new[] { "AllowedChildTypes" },
            //    Select = new[] { "Name", "AllowedChildTypes" }
            //});
            //Console.WriteLine(content.AllowedChildTypes.Count);

            #endregion

            #region // ---- Ordering and Pagination

            //// Ordering
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = {{"$orderby", "Name desc"}},
            //    Select = new[] { "Name" }
            //});
            //foreach (var content in result)
            //    Console.WriteLine(content.Name);

            //// Top Skip
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    //Parameters = { { "$orderby", "Name" } },
            //    Top = 3,
            //    Skip = 4,
            //    //Select = new[] { "Name" }
            //});
            //foreach (var content in result)
            //    Console.WriteLine(content.Name);

            //// Ordering
            //var result4 = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    OrderBy = new[] { "Type asc", "Name" },
            //});
            //foreach (var content in result4)
            //    Console.WriteLine(content.Name);
            #endregion

            #region // ---- Searching and filtering

            //// Filtering by Field value
            // $filter=Index gt 11
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = { { "$filter", "Id gt 1358" } },
            //    //Select = new[] { "Id" }
            //});
            //foreach (var content in result)
            //    Console.WriteLine(content.Id);

            //// $filter=substringof('Lorem', Description) eq true
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = { { "$filter", "substringof('perform', Description) eq true" } },
            //    //Select = new[] { "Name, Description" }
            //});
            //foreach (var content in result)
            //    Console.WriteLine(content.Id);

            //// $filter=startswith(Name, 'Document') eq true
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = true,
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = { { "$filter", "startswith(Name, 'O') eq true" } },
            //    //Select = new[] { "Name" }
            //});

            //// $filter=endswith(Name, 'Library') eq true
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = true,
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = { { "$filter", "endswith(Name, 's') eq true" } },
            //    //Select = new[] { "Name" }
            //});

            //// Filtering by Date
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = true,
            //    Path = "/Root/IMS/BuiltIn/Portal",
            //    Parameters = { { "$filter", "CreationDate gt datetime'2010-02-01T01:01:01'" } },
            //    //Select = new[] { "Name", "CreationDate" }
            //});

            //// Filtering by an exact Type
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn",
            //    Parameters = { { "$filter", "ContentType eq 'OrganizationalUnit'" } },
            //    //Select = new[] { "Name", "Type" }
            //});
            //foreach (dynamic content in result)
            //    Console.WriteLine(content.Type);

            //// Filtering by Type family
            //var result5 = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn",
            //    ChildrenFilter = "isof('Folder')",
            //});
            //foreach (var content in result5)
            //    Console.WriteLine(content.Name);

            #endregion

            #region // ---- Metadata

            //// Metadata
            //var result = await Content.LoadAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn",
            //    Metadata = MetadataFormat.None,
            //    Select = new[] { "Name", "Type" }
            //});

            //// $metadata
            // OData.svc/$metadata
            //var result1 = await RESTCaller.GetResponseStringAsync(
            //    new Uri(url + "/OData.svc/$metadata"));
            // OData.svc/Root/Content/IT/Document_Library/$metadata
            //var result2 = await RESTCaller.GetResponseStringAsync(
            //    new Uri(url + "/OData.svc/Root/IMS/BuiltIn/$metadata"));

            #endregion

            #region // ---- System Content

            //// Accessing system content
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root",
            //    AutoFilters = FilterStatus.Disabled
            //});
            //foreach(var content in result)
            //    Console.WriteLine(content.Name);

            #endregion

            #region // Lifespan

            //// Filter content by lifespan validity
            //var result = await Content.LoadCollectionAsync(new ODataRequest
            //{
            //    Path = "/Root",
            //    LifespanFilter = FilterStatus.Enabled
            //});
            //foreach (var content in result)
            //    Console.WriteLine(content.Name);

            #endregion

            #region // Actions

            //// Actions
            //dynamic content = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    Expand = new[] { "Actions" },
            //    Select = new[] { "Name", "Actions" },
            //    //Parameters = { { "scenario", "UserMenu" } },
            //});
            //foreach(var item in content.Actions)
            //    Console.WriteLine(item.Name);

            #endregion
            //UNDONE:- Actions: Check scenario filter validity
#if FUTURE
            //dynamic content8 = await Content.LoadAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS",
            //    Expand = new[] { "Actions" },
            //    Select = new[] { "Name", "Actions" },
            //    Scenario = "UserMenu",
            //});
#endif

            #region // Get schema

            //// Actions
            //var schema = await RESTCaller.GetResponseStringAsync("/Root", "GetSchema");
            //Console.WriteLine(schema.Substring(0, 200));

            //// Change the schema
            //string ctd = null;
            //await RESTCaller.GetStreamResponseAsync(1064, async message =>
            //{
            //    ctd = await message.Content.ReadAsStringAsync();
            //});
            //Console.WriteLine(ctd.Substring(0, 200));

            #endregion

        }

        private static async Task Querying()
        {
            #region // Querying

            //// Wildcard search (wildcard-search-single)
            //var result1 = await Content.QueryAsync("query=tru?k");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Wildcard search (wildcard-search-multiple)
            //var result2 = await Content.QueryAsync("query=app*");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Fuzzy search (fuzzy-search)
            //var result = await Content.QueryAsync("Description:code~0.8");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Proximity search (proximity-search)
            //var result = await Content.QueryAsync("Description:'kickoff project'~7");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Escaping special characters (special-characters-escaping)
            //var result = await Content.QueryAsync(@"Name:\(apps\) .AUTOFILTERS:OFF");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Escaping special characters (special-character-apostrophe)
            //var result = await Content.QueryAsync("InFolder:\"/Root/(1+1):2\"");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Quick queries (quick-query)
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Id%3A%3C42%20.QUICK
            //var result = await Content.QueryAsync("Id:<42 .QUICK");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Query by Id or Path

            //// Query a content by its Id
            //// https://dev.demo.sensenet.com/OData.svc/Root/?query=Id%3A1607
            //var result = await Content.QueryAsync("Id:1607");
            //foreach (dynamic content in result)
            //    Console.WriteLine(content.Name);

            //// Query multiple content by their Ids
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Id%3A%281607%201640%201645%29
            //var result = await Content.QueryAsync("Id:(1607 1640 1645)");
            //foreach (dynamic content in result)
            //    Console.WriteLine(content.Name);

            //// Search in a folder
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=InFolder%3A%22/Root/Content/IT/Document_Library/Calgary%22
            //// https://localhost:5001/OData.svc/Root?metadata=no&$select=Id,Name&query=.AUTOFILTERS:OFF%20InFolder:%22/Root/IMS/BuiltIn/Portal%22
            //var result = await Content.QueryAsync("InFolder:'/Root/Content/IT/Document_Library/Calgary'");
            //foreach (dynamic content in result)
            //    Console.WriteLine(content.Name);

            //// Search in a branch of the content tree
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=InTree%3A%22/Root/Content/IT/Document_Library%22
            //var result = await Content.QueryAsync("InTree:'/Root/Content/IT/Document_Library'");
            //foreach (dynamic content in result)
            //    Console.WriteLine(content.Name);

            #endregion

            #region // Query by a field

            //// Query by a text field
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Name%3ABusinessPlan.docx
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Description%3A*company*
            //var result1 = await Content.QueryAsync("Name:Visitor");
            //foreach (dynamic content in result1)
            //    Console.WriteLine(content.Name);
            //var result2 = await Content.QueryAsync("Name:*dmi*");
            //foreach (dynamic content in result2)
            //    Console.WriteLine(content.Name);

            //// Query by a number field
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=TaskCompletion%3A%3C50
            //var result = await Content.QueryAsync("Id:<10");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Query by a boolean field
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content?query=IsCritical%3Atrue
            //var result = await Content.QueryAsync("TrashDisabled:true");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Fulltext Search

            //// Fulltext search
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Lorem
            //var result = await Content.QueryAsync("administrative");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Query by date

            //UNDONE:- QUERY "Query by an exact date": don't work
            //// Query by an exact date
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A'2019-02-15'
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=StartDate%3A'2019-02-15 09%3A30%3A00'
            //var result1 = await Content.QueryAsync("CreationDate:'2019-02-15");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("StartDate:'2019-02-15 09:30:00'");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Query before or after a specific date
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A<'2019-01-10'
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=ModificationDate%3A>'2019-01-10'
            //var result1 = await Content.QueryAsync("CreationDate:<'2019-01-10'");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("ModificationDate:>'2019-01-10'");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Query by a date range
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A{'2010-08-30' TO '2010-10-30'}
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A['2010-08-30' TO '2010-10-30']
            //// 3: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A['2010-08-30' TO '2010-10-30'}
            //var result1 = await Content.QueryAsync("CreationDate:{'2010-08-30' TO '2020-10-30'}");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("CreationDate:['2010-08-30' TO '2020-10-30']");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result3 = await Content.QueryAsync("CreationDate:['2010-08-30' TO '2020-10-30'}");
            //foreach (dynamic content in result3)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Querying with dynamic template parameters
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=ModificationDate%3A@Yesterday@
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=StartDate%3A>@NextMonth@
            //// 3: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A@PreviousYear@
            //var result1 = await Content.QueryAsync("ModificationDate:>@@Yesterday@@");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("StartDate:<@@NextMonth@@");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result3 = await Content.QueryAsync("CreationDate:>@@PreviousYear@@");
            //foreach (dynamic content in result3)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Query by lifespan validity
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content?query=+TypeIs%3AArticle .LIFESPAN%3AON
            //var result = await Content.QueryAsync("+TypeIs:Article.LIFESPAN:ON");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Query by related content

            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=+CreatedBy%3A{{+Jobtitle%3A%27developer%27}}
            //// CreatedBy%3A{{Name%3A%27admin%27}}%20.TOP%3A10&metadata=no&$select=Id,Name,CreatedById
            //var result = await Content.QueryAsync("CreatedBy:{{Name:'admin'}} .TOP:10");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Query by Type

            //// Query by a type
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Type%3ADocumentLibrary
            //var result = await Content.QueryAsync("Type:DocumentLibrary");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Query by a type and its subtypes
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=TypeIs:Folder
            //var result = await Content.QueryAsync("TypeIs:Folder");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Ordering

            //// Order by a field - lowest to highest
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=+Type%3AFolder .SORT%3AName
            //var result = await Content.QueryAsync("Type:Folder .SORT:Name");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Order by a field - highest to lowest
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=+Type%3AFolder .REVERSESORT%3AName
            //var result = await Content.QueryAsync("Type:Folder .REVERSESORT:Name");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Order by multiple fields
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Type%3AFolder .SORT%3AName .SORT%3AIndex
            //var result = await Content.QueryAsync("Type:Folder .SORT:Name .SORT:Index");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Order by multiple fields in different directions
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Type%3AFolder .SORT%3AName .REVERSESORT%3AIndex
            //var result = await Content.QueryAsync("Type:Folder .SORT:Name .REVERSESORT:Index");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Order by date
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT/Document_Library/Calgary?query=TypeIs%3AFile%20.SORT%3AModificationDate
            //var result = await Content.QueryAsync("TypeIs:File .SORT:ModificationDate");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Paging

            //// Limit result count
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Type%3AFolder .TOP%3A10
            //var result = await Content.QueryAsync("Type:Folder .TOP:10");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Jump to page
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=Type%3AFolder .SKIP%3A3 .TOP%3A3
            //var result = await Content.QueryAsync("Type:Folder .SKIP:3 .TOP:3");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Multiple predicates

            //// Operators
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=apple OR melon
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=EventType%3ADemo AND EventType%3AMeeting
            //// 3: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=+EventType%3ADemo +EventType%3AMeeting
            //// 4: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=apple NOT melon
            //// 5: https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=apple -melon
            //var result1 = await Content.QueryAsync("sensenet OR github");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("sensenet AND github");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result3 = await Content.QueryAsync("+sensenet +github");
            //foreach (dynamic content in result3)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result4 = await Content.QueryAsync("sensenet NOT github");
            //foreach (dynamic content in result4)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result5 = await Content.QueryAsync("+sensenet -github");
            //foreach (dynamic content in result5)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Grouping
            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=(EventType%3ADemo AND EventType%3AMeeting) OR EventType%3ADeadline
            //var result = await Content.QueryAsync("(EventType:Demo AND EventType:Meeting) OR EventType:Deadline");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Query system content

            //// https://dev.demo.sensenet.com/OData.svc/Root/Content/IT?query=+Type%3AContentType .AUTOFILTERS%3AOFF
            //var result = await Content.QueryAsync("Type:ContentType .AUTOFILTERS:OFF");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

            #region // Template parameters

            //// List of builtin template parameters
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=SharedWith%3A@@CurrentUser@@
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=TypeIs%3ACalendarEvent AND StartDate%3A@@Today@@
            //var result1 = await Content.QueryAsync("SharedWith:@@CurrentUser@@");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("TypeIs:CalendarEvent AND StartDate:@@Today@@");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Templates with properties
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=+TypeIs%3ATask +InTree%3A'@@CurrentWorkspace.Path@@'+DueDate%3A@@NextWeek@@
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/IMS?query=TypeIs%3AUser +CreationDate%3A%3C@@CurrentWorkspace.Manager.CreationDate@@
            //var result1 = await Content.QueryAsync("+TypeIs:Task +InTree:'@@CurrentWorkspace.Path@@' +DueDate:@@NextWeek@@");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            ////UNDONE:- QUERY "Templates with properties" throws an error
            //var result2 = await Content.QueryAsync("TypeIs:User +CreationDate:<@@CurrentWorkspace.Manager.CreationDate@@");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Template expressions
            //// 1: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A<@@CurrentDate-5days)@@
            //// 2: https://dev.demo.sensenet.com/OData.svc/Root/Content?query=CreationDate%3A<@@CurrentDate.AddDays(-5)@@
            //var result1 = await Content.QueryAsync("CreationDate:<@@CurrentDate-5days)@@");
            //foreach (dynamic content in result1)
            //    Console.WriteLine($"{content.Id} {content.Name}");
            //var result2 = await Content.QueryAsync("CreationDate:<@@CurrentDate.AddDays(-5)@@");
            //foreach (dynamic content in result2)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            #endregion

        }

        private static async Task ContentManagement()
        {
            #region ---- Create
            //// create
            //var content = Content.CreateNew("/Root/Content/IT", "Folder", "My new folder");
            //await content.SaveAsync();

            //// createWs
            //var content = Content.CreateNew("/Root/Content", "Workspace", "My workspace");
            //await content.SaveAsync();

            //// createDocLib
            //var content = Content.CreateNew("/Root/Content/IT", "DocumentLibrary", "My Doclib");
            //await content.SaveAsync();

            //// createUser
            //var content = Content.CreateNew("/Root/IMS/Public", "User", "alba");
            //content["LoginName"] = "alba";
            //content["Enable"] = true;
            //await content.SaveAsync();

            //// createByTemplate
            //var content = Content.CreateNew("/Root/Content/IT", "EventList", "My Calendar",
            //    "/Root/ContentTemplates/DemoWorkspace/Demo_Workspace/Calendar");
            //content["DisplayName"] = "Calendar";
            //content["Index"] = 2;
            //await content.SaveAsync();

            #endregion

            #region ---- Update

            //// updatePatch
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["Index"] = 142;
            //await content.SaveAsync();

            //// updateMultipleFields
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["Name"] = "NewName";
            //content["Index"] = 142;
            //await content.SaveAsync();

            //// updateDate
            //var content = await Content.LoadAsync("/Root/Content/IT/Calendar/Release");
            //content["StartDate"] = new DateTime(2020, 3, 4, 9, 30, 0);
            //await content.SaveAsync();

            //// updateChoice
            //var content = await Content.LoadAsync("/Root/Content/IT/Calendar/Release");
            //content["EventType"] = new[] { "Demo", "Meeting" };
            //await content.SaveAsync();

            //// updateReference
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["Manager"] = 12345;
            //await content.SaveAsync();

            //// updateReferenceMultiple
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["Customers"] = new[] { "/Root/Customer1", "/Root/Customer2" };
            //await content.SaveAsync();

            //// updatePut
            //var postData = new Dictionary<string, object>
            //{
            //    {"DisplayName", "Readable name"},
            //    {"Index", 42},
            //    {"Manager", "/Root/IMS/Public/businesscat"}
            //};
            //await RESTCaller.PutContentAsync("/Root/Content/IT", postData);


            #endregion

            #region ---- Delete
            //// Delete a single content
            //var c = Content.CreateNew("/Root/Content/IT", "Folder", "Folder1");
            //await c.SaveAsync();
            //var content = await Content.LoadAsync(c.Id);
            //await content.DeleteAsync();

            // Delete multiple content at once
            //UNDONE:- ContentManagement: Missing batch delete operation (delete multiple content at once) Content.Delete(params int[] idsToDelete) + Content.Delete(params string[] pathsToDelete)

            //// Move items to the trash
            //var c = Content.CreateNew("/Root/Content/IT", "Folder", "Folder1");
            //await c.SaveAsync();
            //var content = await Content.LoadAsync(c.Id);
            //await content.DeleteAsync();

            #endregion






            //string path = null;
            //using (var stream = new FileStream(@"D:\dev\Examples\TestCTD.xml", FileMode.Open))
            //{
            //    var uploaded = await Content.UploadAsync("/Root/System/Schema/ContentTypes", "Test.xml",
            //        stream, "ContentType").ConfigureAwait(false);
            //    path = uploaded.Path;
            //    Console.WriteLine($"CREATED: Id: {uploaded.Id}, Name: {uploaded.Name}, Path: {path}");
            //}

            //Thread.Sleep(1000);
            //Console.WriteLine("DELETING...");

            //await Content.DeleteAsync(path, true, CancellationToken.None).ConfigureAwait(false);

            //Console.WriteLine($"DELETED: {path}");
            /*
            var imgLib = await Content.LoadAsync("/Root/System/ImageLibrary1").ConfigureAwait(false);
            if (imgLib == null)
            {
                var content = Content.CreateNew("/Root/System", "ImageLibrary", "ImageLibrary1");
                await content.SaveAsync().ConfigureAwait(false);
            }

            using (var stream = new FileStream(@"D:\dev\Examples\ImageLibraryCtd.xml", FileMode.Open))
            {
                var uploaded = await Content.UploadAsync("/Root/System/Schema/ContentTypes", "ImageLibraryCtd.xml",
                    stream, "ContentType");
                Console.WriteLine($"Name: {uploaded.Name}");
            }
            */




            #region ---- Upload

            //var uploadRootPath = "/Root/UploadTests";
            //var uploadFolder = await Content.LoadAsync(uploadRootPath).ConfigureAwait(false);
            //if (uploadFolder == null)
            //{
            //    uploadFolder = Content.CreateNew("/Root", "SystemFolder", "UploadTests");
            //    await uploadFolder.SaveAsync().ConfigureAwait(false);
            //}

            //// Upload a file
            //Content uploaded;
            //using (var fileStream = new FileStream(@"D:\dev\Examples\MyFile.txt", FileMode.Open))
            //    uploaded = await Content.UploadAsync("/Root/UploadTests", "MyFile.txt", fileStream, "File");

            //// Create a file with raw text
            //var fileText = " *** file text data ***";
            //await Content.UploadTextAsync("/Root/Content/IT/Document_Library", "MyFile.txt",
            //    fileText, CancellationToken.None, "File");

            //// Update a CTD
            //string fileText;
            //using (var reader = new StreamReader(@"D:\dev\Examples\DomainsCtd.xml"))
            //    fileText = reader.ReadToEnd();
            //var uploaded = await Content.UploadTextAsync("/Root/System/Schema/ContentTypes", "Domains",
            //    fileText, CancellationToken.None, "ContentType");

            //using (var stream = new FileStream(@"D:\dev\Examples\DomainsCtd.xml", FileMode.Open))
            //{
            //    var uploaded = await Content.UploadAsync("/Root/System/Schema/ContentTypes", "Domains",
            //        stream, "ContentType");
            //    Console.WriteLine($"Name: {uploaded.Name}");
            //}

            //// Update a Settings file
            //var uploaded = await Content.UploadTextAsync("/Root/System/Settings", "MyCustom.settings",
            //    "{Key:'Value'}", CancellationToken.None, "Settings");

            // Upload whole files instead of chunks

            // Upload a structure

            // Interrupted uploads

            //// check
            //string fileContent = null;
            //await RESTCaller.GetStreamResponseAsync(uploaded.Id, async response =>
            //{
            //    if (response == null)
            //        return;
            //    using (var stream = await response.Content.ReadAsStreamAsync())
            //    using (var reader = new StreamReader(stream))
            //        fileContent = reader.ReadToEnd();
            //}, CancellationToken.None);

            //await uploaded.DeleteAsync();

            #endregion

            #region ---- Copy or move

            //// copyContent
            //var body = @"models=[{""targetPath"": ""/Root/Content/IT/Document_Library/Munich"",
            //            ""paths"": [""/Root/Content/IT/Document_Library/Chicago/100Pages.pdf""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root", "CopyBatch", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// copyMultiple
            //var body = @"models=[{""targetPath"": ""/Root/Content/IT/Document_Library/Munich"",
            //            ""paths"": [""/Root/Content/IT/Document_Library/Chicago/100Pages.pdf"",
            //                        ""/Root/Content/IT/Document_Library/Chicago/400Pages.pdf""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root", "CopyBatch", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// moveContent
            //var body = @"models=[{""targetPath"": ""/Root/Content/IT/Document_Library/Munich"",
            //            ""paths"": [""/Root/Content/IT/Document_Library/Chicago/100Pages.pdf""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root", "MoveBatch", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// moveMultiple
            //var body = @"models=[{""targetPath"": ""/Root/Content/IT/Document_Library/Munich"",
            //            ""paths"": [""/Root/Content/IT/Document_Library/Chicago/100Pages.pdf"",
            //                        ""/Root/Content/IT/Document_Library/Chicago/400Pages.pdf""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root", "MoveBatch", HttpMethod.Post, body);
            //Console.WriteLine(result);

            #endregion

            #region ---- Allowed Childtypes

            //// effectivelyAllowed
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "Root/Content/IT", "EffectiveAllowedChildTypes");
            //Console.WriteLine(result);

            //// allowedChildTypes
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "Root/Content/IT", "AllowedChildTypes");
            //Console.WriteLine(result);

            //// allowedChildTypesFromCTD
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "Root/Content/IT", "GetAllowedChildTypesFromCTD");
            //Console.WriteLine(result);

            //// updateAllowedChildTypes
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["AllowedChildTypes"] = new[] {"ImageLibrary", "DocumentLibrary", "TaskList"};
            //await content.SaveAsync();

            //// addTypes
            //var body = @"models=[{""contentTypes"": [""Task"", ""Image""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "AddAllowedChildTypes", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// removeTypes
            //var body = @"models=[{""contentTypes"": [""Task"", ""Image""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "RemoveAllowedChildTypes", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //UNDONE:- ContentManagement: Missing attr on action CheckAllowedChildTypesOfFolders: [AllowedRoles(N.R.Everyone)]
            //// checkAllowedTypes
            //
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "CheckAllowedChildTypesOfFolders");
            //Console.WriteLine(result);

            #endregion

            #region ---- Trash

            //var content1 = Content.CreateNew("/Root", "SystemFolder", "target");
            //await content1.SaveAsync();
            //var content2 = Content.CreateNew("/Root", "SystemFolder", "test");
            //await content2.SaveAsync();
            //await content2.DeleteAsync(false);
            //int q = 1;


            //// disableTrashGlobally
            //var body = @"models=[{""IsActive"": false}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash", null, HttpMethod.Patch, body);
            //Console.WriteLine(result);


            //// disableTrashOnAContent
            //var body = @"models=[{""TrashDisabled"": true}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", null, HttpMethod.Patch, body);
            //Console.WriteLine(result);


            //// trashOptions 
            //var body = @"models=[{""SizeQuota"": 20,
            //    ""BagCapacity"": 100,
            //    ""MinRetentionTime"": 14}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash", null, HttpMethod.Patch, body);
            //Console.WriteLine(result);


            //// restoreFromTrash
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash/TrashBag-20200622231439", "Restore", HttpMethod.Post);
            //Console.WriteLine(result);

            //// restoreToAnotherDestination
            //var body = @"models=[{""destination"": ""/Root/target""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash/TrashBag-20200622232234", "Restore", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// restoreWithNewName
            //var body = @"models=[{""newname"": true}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash/TrashBag-20200622233412", "Restore", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// deleteFromTrash
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Trash/TrashBag-20200622234314", "Delete", HttpMethod.Post);
            //Console.WriteLine(result);

            #endregion

            #region ---- List Fields

            //// Browsing list fields (selectByListField)
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library",
            //    Select = new[] { "%23CustomField" },
            //});
            //Console.WriteLine(result);


            //// List expando fields defined on a specified list (metadata)
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library", "$metadata", HttpMethod.Get);
            //Console.WriteLine(result);


            //// Add a new list field (addField)
            //var body = @"models=[{""__ContentType"": ""IntegerFieldSetting"",
            //    ""Name"": ""MyField1"",
            //    ""DisplayName"": ""My Field 1"",
            //    ""Compulsory"": true,
            //    ""MinValue"": 10}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library", null, HttpMethod.Post, body);
            //Console.WriteLine(result);


            //// Edit expando fields (editFieldVirtualChildPatch)
            //var body = @"models=[{""MinValue"": 5, ""MaxValue"": 20}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/MyField1", null, HttpMethod.Patch, body);
            //Console.WriteLine(result);


            //// (editFieldVirtualChildPut)
            //var body = @"models=[{""MinValue"": 5, ""DisplayName"": ""My field 2""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/MyField1", null, HttpMethod.Put, body);
            //Console.WriteLine(result);


            //// (editFieldWithAction)
            //var body = @"models=[{""Name"": ""MyField1"", ""MinValue"": 3, ""MaxValue"": 19}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library", "EditField", HttpMethod.Post, body);
            //Console.WriteLine(result);


            //// Remove a list field (removeFieldVirtualChild)
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/MyField1", null, HttpMethod.Delete, null);
            //Console.WriteLine(result);


            //// (removeFieldAction)
            //var body = @"models=[{""Name"": ""MyField1""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library", "DeleteField", HttpMethod.Post, body);
            //Console.WriteLine(result);

            #endregion

        }

        private static async Task Sharing()
        {
            //// Share with a user
            //var body = @"models=[{
            //  ""token"": ""alba@sensenet.com"",
            //  ""level"": ""Open"",
            //  ""mode"": ""Private"",
            //  ""sendNotification"": true}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "Share", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Share content with external users via email
            //var body = @"models=[{
            //  ""token"": ""alba@sensenet.com"",
            //  ""level"": ""Open"",
            //  ""mode"": ""Public"",
            //  ""sendNotification"": true}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "Share", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Sharing levels
            //var body = @"models=[{
            //  ""token"": ""alba@sensenet.com"",
            //  ""level"": ""Edit"",
            //  ""mode"": ""Private"",
            //  ""sendNotification"": true}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "Share", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Remove sharing
            //var body = @"models=[{""id"": ""1b9abb5f-ed49-48c8-8edd-2c7e634bd77b""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "RemoveSharing", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Getting sharing entries for a content
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "GetSharing");
            //Console.WriteLine(result);

            //// Content shared with a specific user
            //var result = await Content.QueryAsync("SharedWith:@@CurrentUser@@");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Content shared by a specific user
            //var result = await Content.QueryAsync("SharedBy:@@CurrentUser@@");
            //foreach (dynamic content in result)
            //    Console.WriteLine($"{content.Id} {content.Name}");

            //// Notifications
            //var body = @"models=[{
            //  ""token"": ""alba@sensenet.com"",
            //  ""level"": ""Edit"",
            //  ""mode"": ""Private"",
            //  ""sendNotification"": false}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT", "Share", HttpMethod.Post, body);
            //Console.WriteLine(result);
        }

        private static async Task Preview()
        {
            //// Get page count
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "GetPageCount", HttpMethod.Post);
            //Console.WriteLine(result);

            //// Check previews
            //var body = @"models=[{
            //  ""generateMissing"": false}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "CheckPreviews",
            //    HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Regenerate previews
            //await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "RegeneratePreviews",
            //    HttpMethod.Post);

            //// Add comment
            //var body = @"models=[{
            //    ""page"": 3,
            //    ""x"": 100,
            //    ""y"": 100,
            //    ""text"": ""Lorem ipsum dolor sit amet""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "AddPreviewComment",
            //    HttpMethod.Post, body);
            //Console.WriteLine(result);

            //UNDONE:- Preview: GetResponseJsonAsync cannot return with Array
            //UNDONE:- Preview: GetResponseStringAsync(ODataRequest...)
            //// Get comments for a page
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "GetPreviewComments",
            //    Parameters = { { "page", "3" } }
            //});
            //Console.WriteLine(result);

            //// Remove comment
            //var body = @"models=[{
            //  ""id"": ""839ba802-d587-4153-b4e8-ccd4c593e1f4""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "DeletePreviewComment",
            //    HttpMethod.Post, body);
            //Console.WriteLine(result);

        }

        private static async Task CollabVersioning()
        {
            //// Enable versioning
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["InheritableVersioningMode"] = new[] { 3 };
            //await content.SaveAsync();

            //// Get current version of a content (versionNumber)
            //dynamic content = await Content.LoadAsync("/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx");
            //var version = content.Version;
            //Console.WriteLine(version);

            //// Get a specific version of a content (specificVersion)
            //var req = new ODataRequest(ClientContext.Current.Server)
            //{
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    Version = "V1.0.A"
            //};
            //dynamic content = await Content.LoadAsync(req);
            //Console.WriteLine(content.Version);

            //// Checkout a content for edit (checkout)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "Checkout",
            //});
            //Console.WriteLine(result);

            //// Checkin a content (checkin)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "CheckIn",
            //    Parameters = { { "CheckInComments", "Adding new contract" } }
            //});
            //Console.WriteLine(result);

            //// How to know if a content is locked (locked)
            //dynamic content = await Content.LoadAsync(new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    Expand = new List<string> { "CheckedOutTo" },
            //    Select = new List<string> { "Locked", "CheckedOutTo/Name" },
            //});
            //var locked = content.Locked;
            //var lockedBy = content.CheckedOutTo.Name;
            //Console.WriteLine($"Locked: {locked}, LockedBy: {lockedBy}");

            //// Publish a new major version (publish)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "Publish",
            //});
            //Console.WriteLine(result);

            //// Undo changes (undoChanges)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "UndoCheckOut",
            //});
            //Console.WriteLine(result);

            //// Force undo changes (forceUndoChanges)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "ForceUndoCheckOut",
            //});
            //Console.WriteLine(result);

            //// Take lock over (takeLockOver)
            //var body = @"models=[{""user"": ""12345""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "TakeLockOver", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Get version history of a content (versionHistory)
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "Versions", HttpMethod.Get);
            //Console.WriteLine(result);

            //// Restore an old version (recallVersion)
            //var body = @"models=[{""version"": ""V1.0.A""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "RestoreVersion", HttpMethod.Post, body);
            //Console.WriteLine(result);
        }

        private static async Task CollabApproval()
        {
            //// Enable simple approval (enableApproval)
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //content["InheritableApprovingMode"] = new[] { 2 };
            //content["InheritableVersioningMode"] = new[] { 3 };
            //await content.SaveAsync();

            //// Approve a content (approve)
            //var result = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "Approve",
            //});
            //Console.WriteLine(result);

            //// Reject a content (reject)
            //var body = @"models=[{""rejectReason"": ""Reject reason""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "Reject", HttpMethod.Post, body);
            //Console.WriteLine(result);
        }

        private static async Task CollabSavedQueries()
        {
            //// Save a query (saveQuery)
            //var body = @"models=[{
            //  ""query"": ""+TypeIs:File +InTree:/Root/Content/IT"",
            //  ""displayName"": ""Public query"",
            //  ""queryType"": ""Public""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library", "SaveQuery", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Save a private query (savePrivateQuery)
            //var body = @"models=[{
            //  ""query"": ""+TypeIs:File +InTree:/Root/Content/IT"",
            //  ""displayName"": ""My query"",
            //  ""queryType"": ""Private""}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx", "SaveQuery", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Get saved queries (getSavedQueries)
            //var result = await RESTCaller.GetResponseJsonAsync(new ODataRequest
            //{
            //    IsCollectionRequest = false,
            //    Path = "/Root/Content/IT/Document_Library/Calgary/BusinessPlan.docx",
            //    ActionName = "GetQueries",
            //    Parameters = { { "onlyPublic", "true" } }
            //});
            //Console.WriteLine(result);
        }

        private static async Task UsersAndGroups()
        {
            //// (createUser)
            //var content = Content.CreateNew("/Root/IMS/Public", "User", "alba");
            //content["LoginName"] = "alba";
            //content["Email"] = "alba@sensenet.com";
            //content["Password"] = "alba";
            //content["Enabled"] = true;
            //await content.SaveAsync();

            //// (disableUser)
            //var content = await Content.LoadAsync("/Root/IMS/Public/alba");
            //content["Enabled"] = false;
            //await content.SaveAsync();

            //// (createRole)
            //var content = Content.CreateNew("/Root/IMS/Public", "Group", "Publishers");
            //content["Members"] = new[] { 1, 1358 };
            //await content.SaveAsync();
        }

        private static async Task Permissions()
        {
            //UNDONE:- Feature request: Content.GetPermissionAsync
            ////Get permission entries of a content (getPermissionEntries)
            //var result = await RESTCaller.GetResponseStringAsync("/Root/Content/IT", "GetPermissions");

            //UNDONE:- Feature request: Content.GetPermissionAsync(identity)
            //// Get a permissions entry of a specific user or group (getPermissionEntry)
            //var req = new ODataRequest
            //{
            //    ActionName = "GetPermissions",
            //    Path = "/Root/Content/IT",
            //    Parameters = { { "identity", "/Root/IMS/Public/Editors" } }
            //};
            //var result = await RESTCaller.GetResponseStringAsync(req);

            //// Check user access (hasPermission)
            //var content = await Content.LoadAsync("/Root/Content/IT").ConfigureAwait(false);
            //var hasPermission = await content.HasPermissionAsync(new[] { "Open" });

            //// How can I check why a user cannot access a content? (hasPermissionUser)
            //var content = await Content.LoadAsync("/Root/Content/IT").ConfigureAwait(false);
            //var hasPermission = await content.HasPermissionAsync(new[] { "Open" }, "/Root/IMS/BuiltIn/Portal/Visitor");

            //// How can I check why a user cannot save a content? (canSave)
            //var content = await Content.LoadAsync("/Root/Content/IT").ConfigureAwait(false);
            //var hasPermission = await content.HasPermissionAsync(new[] { "Open,Save" }, "/Root/IMS/BuiltIn/Portal/Visitor");

            ////  Check if I can see the permission settings (canSeePermissions)
            //var content = await Content.LoadAsync("/Root/Content/IT").ConfigureAwait(false);
            //var hasPermission = await content.HasPermissionAsync(new[] { "SeePermissions" }, "/Root/IMS/BuiltIn/Portal/Visitor");

        }

        private static async Task PermissionManagement()
        {
            //UNDONE:- PermissionManagement: missing content.SetPermissionAsync(...)
            //// Allow a user to save a content (allowSave)
            //var permissionRequest = new[]
            //{
            //    new SetPermissionRequest
            //    {
            //        Identity = "/Root/IMS/BuiltIn/Portal/Visitor",
            //        Save = PermissionValue.Allow,
            //    }
            //};
            //var content = await Content.LoadAsync("/Root/Content/IT");
            //await SecurityManager.SetPermissionsAsync(content.Id, permissionRequest);

            //// Allow a group (role) to approve content in a document library (allowApproveForAGroup)
            //var content = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            //var permissionRequest = new[]
            //{
            //    new SetPermissionRequest
            //    {
            //        Identity = "/Root/IMS/Public/Editors",
            //        Approve = PermissionValue.Allow,
            //    }
            //};
            //await SecurityManager.SetPermissionsAsync(content.Id, permissionRequest);

            //// Prohibit a user from deleting content from a folder (denyDelete)
            //var content = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            //var permissionRequest = new[]
            //{
            //    new SetPermissionRequest
            //    {
            //        Identity = "/Root/IMS/Public/Editors",
            //        Delete = PermissionValue.Deny,
            //    }
            //};
            //await SecurityManager.SetPermissionsAsync(content.Id, permissionRequest);

            //// Break inheritance (breakInheritance)
            //var content = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            //await content.BreakInheritanceAsync();

            //// Local only (localOnly)
            //var content = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            //var permissionRequest = new[]
            //{
            //    new SetPermissionRequest
            //    {
            //        Identity = "/Root/IMS/Public/Editors",
            //        LocalOnly = true,
            //        AddNew = PermissionValue.Allow,
            //    }
            //};
            //await SecurityManager.SetPermissionsAsync(content.Id, permissionRequest);

            //// Using custom permissions (customPermission)
            //var content = await Content.LoadAsync("/Root/Content/IT/Document_Library");
            //var permissionRequest = new[]
            //{
            //    new SetPermissionRequest
            //    {
            //        Identity = "/Root/IMS/Public/Editors",
            //        Custom01 = PermissionValue.Allow,
            //    }
            //};
            //await SecurityManager.SetPermissionsAsync(content.Id, permissionRequest);
        }

        private static async Task PermissionQueries()
        {
            //// Get all identities connected to a content (getRelatedIdentities)
            //var result = await RESTCaller.GetResponseStringAsync(new ODataRequest
            //{
            //    ActionName = "GetRelatedIdentities",
            //    Path = "/Root/Content/IT",
            //    Select = new[] { "Id", "Path", "Type" },
            //    Parameters =
            //    {
            //        { "permissionLevel", "AllowedOrDenied" },
            //        { "identityKind", "Groups" }
            //    }
            //});
            //Console.WriteLine(result);

            //UNDONE:- PermissionQueries: this call can be GET if the "includedTypes" is optional param on the server side.
            //// Count number of permissions settings per identity (getRelatedPermissions)
            //var body = @"models=[{""permissionLevel"": ""AllowedOrDenied"", ""explicitOnly"": true,
            //            ""memberPath"": ""/Root/IMS/BuiltIn/Portal/Visitor"", ""includedTypes"": null,}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root", "GetRelatedPermissions", HttpMethod.Post, body);
            //Console.WriteLine(result);

            //UNDONE:- PermissionQueries: this call can be GET if the "permissions" is ODataParameterCollection on the server side.
            //// Get content with permission settings for a specific identity (getRelatedItems)
            //var req = new ODataRequest
            //{
            //    Path = "/Root",
            //    ActionName = "GetRelatedItems",
            //    Select = new[] { "Id", "Path", "Type" },
            //};
            //var body = @"models=[{""permissionLevel"": ""AllowedOrDenied"", ""explicitOnly"": true,
            //            ""memberPath"": ""/Root/IMS/BuiltIn/Portal/Admin"", ""permissions"": [""Save""],}]";
            //var result = await RESTCaller.GetResponseStringAsync(req, HttpMethod.Post, body);
            //Console.WriteLine(result);

            //UNDONE:- PermissionQueries: this call can be GET if the "permissions" is ODataParameterCollection on the server side.
            //// Get identities related to a permission in a subtree (getRelatedIdentitiesByPermissions)
            //var req = new ODataRequest
            //{
            //    Path = "/Root",
            //    ActionName = "GetRelatedIdentitiesByPermissions",
            //    Select = new[] { "Id", "Path", "Type" },
            //};
            //var body = @"models=[{""permissionLevel"": ""AllowedOrDenied"",
            //            ""identityKind"": ""Groups"", ""permissions"": [""Save""],}]";
            //var result = await RESTCaller.GetResponseStringAsync(req, HttpMethod.Post, body);
            //Console.WriteLine(result);

            //UNDONE:- PermissionQueries: this call can be GET if the "permissions" is ODataParameterCollection on the server side.
            //// Get contents related to a permission in a container (getRelatedItemsOneLevel)
            //var req = new ODataRequest
            //{
            //    Path = "/Root",
            //    ActionName = "GetRelatedItemsOneLevel",
            //    Select = new[] { "Id", "Path", "Type" },
            //};
            //var body = @"models=[{""permissionLevel"": ""AllowedOrDenied"",
            //            ""memberPath"": ""/Root/IMS/BuiltIn/Portal/Admin"", ""permissions"": [""Open""]}]";
            //var result = await RESTCaller.GetResponseStringAsync(req, HttpMethod.Post, body);
            //Console.WriteLine(result);

            //// Get list of users allowed to do something (getAllowedUsers)
            //var result = await RESTCaller.GetResponseStringAsync(new ODataRequest
            //{
            //    Path = "/Root/Content/IT",
            //    ActionName = "GetAllowedUsers",
            //    Select = new[] { "Id", "Path", "Type" },
            //    Parameters = { { "permissions", "Open" } }
            //});
            //Console.WriteLine(result);

            //// List of group memberships of a user (getParentGroups)
            //var result = await RESTCaller.GetResponseStringAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal/Admin",
            //    ActionName = "GetParentGroups",
            //    Select = new[] { "Id", "Path", "Type" },
            //    Parameters = { { "directOnly", "true" } }
            //});
            //Console.WriteLine(result);

        }

        private static async Task UsersAndGroups_GroupMembership()
        {
            // Load members of a group (loadMembers)
            //dynamic administrators = await RESTCaller.GetContentAsync(new ODataRequest
            //{
            //    Path = "/Root/IMS/BuiltIn/Portal/Administrators",
            //    IsCollectionRequest = false,
            //    Expand = new[] { "Members" },
            //    Select = new[] { "Members/LoginName" }
            //});
            //foreach (dynamic content in administrators.Members)
            //{
            //    Console.WriteLine(content.LoginName);
            //}


            // Add members to a group (addMember)
            //var body = @"models=[{""contentIds"": [ 6 ]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/IMS/BuiltIn/Portal/Administrators", "AddMembers", HttpMethod.Post, body);
            //Console.WriteLine(result);

            // Remove members from a group ()
            //var body = @"models=[{""contentIds"": [ 6 ]}]";
            //var result = await RESTCaller.GetResponseStringAsync(
            //    "/Root/IMS/BuiltIn/Portal/Administrators", "RemoveMembers", HttpMethod.Post, body);
            //Console.WriteLine(result);


        }
    }
}
