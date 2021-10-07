using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public interface IBlobProvider
    {
        Task SaveAsync(BlobProviderSaveArgs args);
        
        Task<bool> DeleteAsync(BlobProviderDeleteArgs args);
        
        Task<bool> ExistsAsync(BlobProviderExistsArgs args);
        
        Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
        Task<List<string>> GetListAsync(BlobProviderGetArgs args);
    }
}