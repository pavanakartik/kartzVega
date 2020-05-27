using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using vega.Controllers.Resources;
using vega.Core;
using vega.Core.Models;
using vega.Persistence;

namespace vega.Controllers { //api/vehicles/1/photos

    [Route ("api/vehicles/{vehicleId}/photos")]
    public class PhotosController : Controller {

        private readonly IWebHostEnvironment host;
        private readonly IVehicleRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly PhotoSettings photoSettings;
        private readonly IMapper mapper;
        private readonly IPhotoRepository photoRepository;
        public PhotosController (IWebHostEnvironment host, IVehicleRepository repository, IPhotoRepository photoRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IOptionsSnapshot<PhotoSettings> options) {
            this.photoRepository = photoRepository;

            this.photoSettings = options.Value;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
            this.host = host;

        }

        [HttpPost]
        public async Task<IActionResult> Upload (int vehicleId, IFormFile file) {
            var vehicle = await repository.GetVehicle (vehicleId, includeRelated : false);

            if (vehicle == null)
                return NotFound ();

            // check to see if fileinput is null or not

            if (file == null) return BadRequest ("File Not Found");
            if (file.Length == 0) return BadRequest ("EmptyFile");
            if (file.Length > photoSettings.MaxBytes) return BadRequest ("Maximum File Size Exceeded");

            if (!photoSettings.IsSupported (file.FileName)) return BadRequest ("Invalid file name");

            var uploadsFolderPath = Path.Combine (host.WebRootPath, "uploads");
            if (!Directory.Exists (uploadsFolderPath))
                Directory.CreateDirectory (uploadsFolderPath);

            var fileName = Guid.NewGuid ().ToString () + Path.GetExtension (file.FileName);

            var filePath = Path.Combine (uploadsFolderPath, fileName);

            using (var stream = new FileStream (filePath, FileMode.Create)) {
                await file.CopyToAsync (stream);

            }

            var photo = new Photo { FileName = fileName };

            vehicle.Photos.Add (photo);

            await unitOfWork.CompleteAsync ();

            return Ok (mapper.Map<Photo, PhotoResource> (photo));

        }

        [HttpGet]
        public async Task<IEnumerable<PhotoResource>> GetPhotos (int vehicleId) {

            var photos = await photoRepository.GetPhotos (vehicleId);

            return mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>> (photos);
        }

    }
}