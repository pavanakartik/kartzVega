using System.Collections.Generic;
using System.Threading.Tasks;
using vega.Core.Models;

namespace vega.Persistence
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetPhotos(int vehicleId);
    }
}