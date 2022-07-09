using FileInfo = MediafonTech.ApplicationCore.Entities.FileInfo;

namespace MediafonTech.ApplicationCore.Interfaces.Repositories
{
    public interface IFileInfoRepository
    {
        Task<IList<string>> GetOldFiles(IList<string> fileNames);

        Task InsertAsync(IList<FileInfo> fileInfoList);
    }
}
