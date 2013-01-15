using System;
using System.Collections.Generic;
using System.IO;

namespace Platform.StreamStorage.File
{
    /// <summary>
    /// This interface is not really needed in the codebase,
    /// but is introduced to explicitly demonstrate the concept
    /// </summary>
    public interface IContainerManagerConcept : IDisposable
    {
        
    }
    public class FileContainerManager : IContainerManagerConcept
    {
        readonly IDictionary<string, FileContainer> _stores = new Dictionary<string, FileContainer>();

        readonly string _rootDirectory;

        readonly ILogger Log = LogManager.GetLoggerFor<FileContainerManager>();

        public FileContainerManager(string rootDirectory)
        {
            if (null == rootDirectory)
                throw new ArgumentNullException("rootDirectory");

            _rootDirectory = rootDirectory;

            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            var info = new DirectoryInfo(rootDirectory);
            foreach (var child in info.GetDirectories())
            {
                if (EventStoreName.IsValid(child.Name) != EventStoreName.Rule.Valid)
                {
                    Log.Error("Skipping invalid folder {0}", child.Name);
                    continue;
                }
                var container = EventStoreName.Create(child.Name);
                if (FileContainer.ExistsValid(rootDirectory, container))
                {
                    var writer = FileContainer.OpenExistingForWriting(rootDirectory, container);
                    _stores.Add(container.Name, writer);
                }
                else
                {
                    Log.Error("Skipping invalid folder {0}", child.Name);
                }
            }
        }

        public void Reset()
        {
            foreach (var store in _stores)
            {
                store.Value.Reset();
            }
        }

        public void Append(EventStoreName container, string streamKey, IEnumerable<byte[]> data)
        {
            FileContainer value;
            if (!_stores.TryGetValue(container.Name, out value))
            {
                value = FileContainer.CreateNew(_rootDirectory, container);
                _stores.Add(container.Name, value);
            }
            value.Write(streamKey, data);
        }


        public void Dispose()
        {
            foreach (var writer in _stores.Values)
            {
                using (writer)
                {
                    writer.Dispose();
                }
            }
        }
    }
}