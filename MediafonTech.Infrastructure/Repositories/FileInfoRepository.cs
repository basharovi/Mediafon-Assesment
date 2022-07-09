using MediafonTech.ApplicationCore.Interfaces.Repositories;
using MediafonTech.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FileInfo = MediafonTech.ApplicationCore.Entities.FileInfo;

namespace MediafonTech.Infrastructure.Repositories
{
    public class FileInfoRepository : IFileInfoRepository
    {
        private readonly AppDbContext _dbContext;

        public FileInfoRepository(IServiceProvider serviceProvider)
        {
            // Resolved the dependency of AppDbContext as Scoped
            _dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public async Task<IList<string>> GetOldFiles(IList<string> fileNames)
        {
            return await _dbContext.FilesInfo.Where(x => fileNames.Contains(x.Name)).Select(x => x.Name).ToListAsync();
        }

        public async Task InsertAsync(IList<FileInfo> fileInfoList)
        {
            await _dbContext.FilesInfo.AddRangeAsync(fileInfoList);

            await _dbContext.SaveChangesAsync();
        }
    }
}
