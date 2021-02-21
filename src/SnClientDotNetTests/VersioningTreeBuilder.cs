using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SenseNet.Client;

namespace SnClientDotNetTests
{
    internal class VersioningTreeBuilder
    {
        private string _rootPath;
        private int _mode;
        private Content _container;

        public VersioningTreeBuilder(string rootPath)
        {
            _rootPath = rootPath;
        }

        public async Task Build(int mode, int maxDepth)
        {
            _mode = mode;
            _container = await VersioningForestBuilder.CreateBrandNewContent(_rootPath, ((Mode)_mode).ToString(),
                "DocumentLibrary", c =>
                {
                    c["InheritableVersioningMode"] = new[] { (_mode / 2) + 1 };
                    c["InheritableApprovingMode"] = new[] { (_mode % 2) + 1 };
                });

            var opChain = new OperationChain(maxDepth);

            int lastItem;
            do
            {
                lastItem = await CreateBranch(opChain);
            } while (opChain.Next(lastItem));

        }

        private async Task<int> CreateBranch(OperationChain opChain)
        {
            Console.WriteLine(string.Join("\t", opChain.Operations.Select(x => x.ToString())));

            var path = _container.Path;
            for (int opIndex = 0; opIndex < opChain.Length; opIndex++)
            {
                var op = opChain.Operations[opIndex];
                var parentPath = path;
                path += "/" + op;
                if (await Content.ExistsAsync(path))
                    continue;

                // Create
                var content = Content.CreateNew(parentPath, "File", op.ToString());
                content["AllowedChildTypes"] = new[] { "SystemFolder", "File" };
                await content.SaveAsync();
                if (opIndex > 0)
                {
                    // Play previous operations 
                    foreach (var operation in opChain.Operations.Skip(1).Take(opIndex - 1))
                        await ExecuteOperation(content, operation);

                    // Play the active operation
                    if (!await ExecuteOperation(content, op))
                    {
                        await Content.DeleteAsync(path, true, CancellationToken.None);
                        return opIndex;
                    }
                    if (await CutDown(content, opChain, opIndex))
                    {
                        await Content.DeleteAsync(path, true, CancellationToken.None);
                        return opIndex;
                    }
                }
            }



            return opChain.Length;
        }

        private Task<bool> CutDown(Content content, OperationChain opChain, int opIndex)
        {
            return Task.FromResult(opIndex > 4);
        }

        private async Task<bool> ExecuteOperation(Content content, Operation operation)
        {
            try
            {
                switch (operation)
                {
                    case Operation.Save:
                        content["Index"] = 1;
                        await content.SaveAsync();
                        break;
                    case Operation.CheckOut:
                        await content.CheckOutAsync();
                        break;
                    case Operation.CheckIn:
                        await content.CheckInAsync();
                        break;
                    case Operation.Undo:
                        await content.UndoCheckOutAsync();
                        break;
                    case Operation.Publish:
                        var json1 = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
                        {
                            IsCollectionRequest = false,
                            Path = content.Path,
                            ActionName = "Publish",
                        });
                        break;
                    case Operation.Approve:
                        var json2 = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
                        {
                            IsCollectionRequest = false,
                            Path = content.Path,
                            ActionName = "Approve",
                        });
                        break;
                    case Operation.Reject:
                        var json3 = await RESTCaller.GetResponseJsonAsync(method: HttpMethod.Post, requestData: new ODataRequest
                        {
                            IsCollectionRequest = false,
                            Path = content.Path,
                            ActionName = "Reject",
                        });
                        break;
                    //case Operation.Create: // Need to be handled.
                    default:
                        break; // do nothing
                }

                return true;
            }
            catch (Exception e)
            {
                // do nothing
                return false;
            }
        }
    }

    internal class OperationChain
    {
        private int _opMaxValue = 7;
        private readonly Operation[] _operations;
        public int Length => _operations.Length;
        public Operation[] Operations => _operations;

        public OperationChain(int maxDepth)
        {
            // Initialize opChain: first is Create (0) all the rest is Save (1)
            _operations = new Operation[maxDepth];
            for (var i = 1; i < _operations.Length; i++)
                _operations[i] = Operation.Save;
        }

        public bool Next(int? currentItem = null)
        {
            var index = currentItem ?? _operations.Length;

            while (--index > 0)
            {
                if (_operations[index] < Operation.Reject)
                {
                    _operations[index]++;
                    for (var i = index + 1; i < _operations.Length; i++)
                        _operations[i] = Operation.Save;
                    return true;
                }
            }

            return false;
        }
    }
}
