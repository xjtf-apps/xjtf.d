namespace xjtf.d;

public sealed partial class ServiceStore
{
    public sealed class ServiceStorageUnit
    {
        private readonly DirectoryInfo _storage;
        internal string InstallFolder => _storage.FullName;
        public bool Empty => EnumerateContents().Count() == 0;

        internal ServiceStorageUnit(ServiceStore serviceStore, string serviceName)
        {
            if (!serviceStore.ExistsStore(serviceName))
                serviceStore.CreateStore(serviceName);

            _storage = serviceStore[serviceName];
        }

        public IEnumerable<dynamic> EnumerateContents()
        {
            string SanitizePath(string path)
                => path.Replace($"{InstallFolder}{Path.DirectorySeparatorChar}", "");

            string GenerateFolderId(string path)
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(path));

            var enumOptions = new EnumerationOptions() { RecurseSubdirectories = true };
            return _storage.EnumerateFileSystemInfos("*", enumOptions)
            .Where(ServiceStoreExtensions.IsNotServiceTag_)
            .Select(fsi => new
            {
                Name = fsi.Name,
                Fullpath = SanitizePath(fsi.FullName),
                Length = fsi is FileInfo fi ? fi.Length : -1,
                Type = fsi is DirectoryInfo ? "directory" : "file",
                ParentId = GenerateFolderId(Path.GetDirectoryName(fsi.FullName)!),
                ChildrenId = fsi is DirectoryInfo ? GenerateFolderId(fsi.FullName) : null
            });
        }

        public async Task AddFormFilesAsync(IEnumerable<IFormFile> formfiles)
        {
            await Task.WhenAll(formfiles.Select(ff => Task.Run(() =>
            {
                var filename = ff.FileName;
                var filestream = ff.OpenReadStream();
                return AddFileAsync(filename, filestream);
            })));
        }

        public async Task AddFileAsync(string filename, Stream filestream)
        {
            var container = _storage;
            var separator = Path.DirectorySeparatorChar;
            var names = filename.Split(separator);

            foreach (var parent in names[..^1])
            {
                var containerPath = Path.Combine(container.FullName, parent);
                container = new DirectoryInfo(containerPath);
                if (!container.Exists) container.Create();
            }

            using var writer = File.OpenWrite(filename);
            await filestream.CopyToAsync(writer);
        }

        public void Clear()
        {
            _storage.Delete(recursive: true);
            _storage.CreateStorageUnit();
        }
    }
}