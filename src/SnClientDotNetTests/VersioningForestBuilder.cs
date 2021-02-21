using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SenseNet.Client;

namespace SnClientDotNetTests
{
    internal class VersioningForestBuilder
    {
        private const int MaxDepth = 5;
        private const int TreeCount = 1;
        private Content _versioningTestRoot;

        public async Task RunAsync()
        {
            var branchPath = new List<Operation>();

            await InitializeFeature().ConfigureAwait(false);

            var tasks = new Task[TreeCount];
            for (var i = 0; i < TreeCount; i++)
            {
                var treeBuilder = new VersioningTreeBuilder(_versioningTestRoot.Path);
                tasks[i] = treeBuilder.Build(i, MaxDepth);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task InitializeFeature()
        {
            await EnsureContent("/Root", "Content", "Folder").ConfigureAwait(false);
            _versioningTestRoot = await EnsureContent("/Root/Content", "VersioningTest", "Workspace").ConfigureAwait(false);
        }

        private async Task CreateTree(int tree, List<Operation> branchPath)
        {
            //while (Next(branchPath))
            //{
            //    await CreateTreeRoot(tree).ConfigureAwait(false);
            //    await CreateBranch(tree, branchPath);
            //}
        }

        private async Task CreateTreeRoot(int tree)
        {
            //_contents[tree] = Content.CreateNew(_containers[tree].Path, "File", "File1.txt");
            //await _contents[tree].SaveAsync().ConfigureAwait(false);

            //if (_forest[tree] == null)
            //    _forest[tree] = await CreateTreeNode(Operation.Create, null, _contents[tree]).ConfigureAwait(false);
        }
        private async Task CreateBranch(int tree, List<Operation> branchPath)
        {
            //var content = _contents[tree];
            //var treeNode = _forest[tree];
            //var steps = GetSteps(branchPath);
        }

        private Operation[] GetSteps(List<Operation> branchPath)
        {
            throw new NotImplementedException();
        }


        private async Task<TreeNode> CreateTreeNode(Operation op, TreeNode parent, Content content)
        {
            var treeNode = new TreeNode
            {
                Operation = op,
                Versions = await GetVersions(content.Path).ConfigureAwait(false)
            };

            if (parent != null)
            {
                treeNode.Parent = parent;
                parent.Children.Add(treeNode);
            }

            return treeNode;
        }
        private async Task<ContentVersion[]> GetVersions(string path)
        {
            var req = new ODataRequest
            {
                Path = path,
                ActionName = "Versions",
                Select = new[] { "VersionId", "Version" }
            };

            var response = await RESTCaller.GetResponseStringAsync(req, HttpMethod.Get)
                .ConfigureAwait(false);

            var list = new List<ContentVersion>();
            var jroot = (JObject)JsonConvert.DeserializeObject(response);
            var array = (JArray)jroot.SelectToken("$.d.results");
            foreach (var item in array)
            {
                var versionId = item["VersionId"].Value<int>();
                var version = item["Version"].Value<string>();
                list.Add(new ContentVersion(version, versionId));
            }

            return list.ToArray();
        }

        async Task<Content> EnsureContent(string parentPath, string name, string contentType)
        {
            var c = await Content.LoadAsync($"{parentPath}/{name}").ConfigureAwait(false);
            if (c == null)
            {
                c = Content.CreateNew(parentPath, contentType, name);
                await c.SaveAsync().ConfigureAwait(false);
            }
            return c;
        }
        internal static async Task<Content> CreateBrandNewContent(string parentPath, string name, string contentType, Action<Content> setPropertiesCallback = null)
        {
            var c = await Content.LoadAsync($"{parentPath}/{name}").ConfigureAwait(false);
            if (c != null)
                await Content.DeleteAsync(c.Id, true, CancellationToken.None).ConfigureAwait(false);
            c = Content.CreateNew(parentPath, contentType, name);
            setPropertiesCallback?.Invoke(c);
            await c.SaveAsync().ConfigureAwait(false);
            return c;
        }
    }

    internal enum Mode
    {
        NoneFalse = 0,
        NoneTrue = 1,
        MajorFalse = 2,
        MajorTrue = 3,
        FullFalse = 4,
        FullTrue = 5
    }

    internal enum Operation
    {
        Create = 0,
        Save = 1,
        CheckOut = 2,
        CheckIn = 3,
        Undo = 4,
        Publish = 5,
        Approve = 6,
        Reject = 7
    }

    internal class ContentVersion
    {
        public int VersionId { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public string State { get; set; }
        public bool IsPublic => Minor == 0;

        public ContentVersion(string version, int versionId)
        {
            var a = version.TrimStart('V').Split('.');
            Major = int.Parse(a[0]);
            Minor = int.Parse(a[0]);
            State = a[2];

            VersionId = versionId;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{State}";
        }
    }
    internal class TreeNode
    {
        public Operation Operation { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
        public string Path => $"{(Parent?.Path ?? "")}/{Operation}_{LastDraftVersion}";
        public ContentVersion[] Versions { get; set; }
        public ContentVersion LastDraftVersion => Versions.LastOrDefault();
        public ContentVersion LastPublicVersion => Versions.LastOrDefault(x => x.IsPublic);
    }
}
