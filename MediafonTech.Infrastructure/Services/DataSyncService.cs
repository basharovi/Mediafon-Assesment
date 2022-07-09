using MediafonTech.ApplicationCore.Interfaces.Services;
using MediafonTech.Infrastructure.Configurations;
using Renci.SshNet;
using MediafonTech.ApplicationCore.Interfaces.Repositories;
using FileInfo = MediafonTech.ApplicationCore.Entities.FileInfo;
using Serilog;

namespace MediafonTech.Infrastructure.Services
{
    public class DataSyncService : IDataSyncService
    {
        private readonly FtpConfig _ftpConfig;
        private readonly IFileInfoRepository _fileRepository;

        public DataSyncService(FtpConfig ftpConfig,
            IFileInfoRepository fileRepository
            )
        {
            _ftpConfig = ftpConfig;
            _fileRepository = fileRepository;
        }

        public async Task StartDataProcessing()
        {
            try
            {
                using var sftpClient = new SftpClient(_ftpConfig.Host, _ftpConfig.Port, _ftpConfig.Username, _ftpConfig.Password);
                sftpClient.Connect();

                var newFiles = await GetNewFiles(sftpClient);

                var downloadedFiles = DownloadFiles(sftpClient, newFiles);

                await InsertDownloadedFiles(downloadedFiles);

                sftpClient.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task<ICollection<string>> GetNewFiles(SftpClient sftpClient)
        {
            // Read the file names(with extn.) from the current directory of Sftp server
            var diretoryFiles = sftpClient.ListDirectory(string.Empty).Where(f => !f.IsDirectory)
                    .Select(f => f.Name).ToList();

            // Checking which file names exist in database
            var oldFiles = await _fileRepository.GetOldFiles(diretoryFiles);

            // Finally, retrieves the new files by comparing them with old files.
            var newFiles = diretoryFiles.Where(file => !oldFiles.Contains(file)).ToList();

            Log.Information("Total new files found = {0}", newFiles.Count);

            return newFiles;
        }

        private IList<FileInfo> DownloadFiles(SftpClient sftpClient, ICollection<string> newFileNames)
        {
            var downloadedFiles = new List<FileInfo>();

            if (newFileNames == null || newFileNames.Count == 0)
            {
                Log.Information("No file could be downloaded, because no new file was found!");
                return downloadedFiles;
            }

            // Downloads new files from sftp server to the local pc one by one
            foreach (var fileName in newFileNames)
            {
                try
                {
                    using var fileStream = File.OpenWrite($"{_ftpConfig.LocalPath}/{fileName}");
                    sftpClient.DownloadFile(fileName, fileStream);

                    // Maintaining a list for tracing which files are downloaded
                    downloadedFiles.Add(new FileInfo()
                    {
                        Name = fileName,
                    });
                }
                // Added Exception handling so that if one file download fails, then could not hamper the others
                catch (Exception ex)
                {
                    Log.Information("Couldn't download file {0}", fileName);
                    Log.Error(ex.ToString());

                    continue;
                }
            }

            Log.Information("Total files Downloaded = {0}", downloadedFiles.Count);

            return downloadedFiles;
        }

        private async Task InsertDownloadedFiles(IList<FileInfo> filesInfo)
        {
            if (filesInfo == null || filesInfo.Count == 0)
                return;

            await _fileRepository.InsertAsync(filesInfo);

            Log.Information("Total files Inserted = {0}", filesInfo.Count);
        }
    }
}
