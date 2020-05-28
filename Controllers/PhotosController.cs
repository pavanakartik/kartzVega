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

        private readonly IPhotoService photoService;
        public PhotosController (IWebHostEnvironment host,
            IVehicleRepository repository,
            IPhotoRepository photoRepository,

            IMapper mapper,
            IOptionsSnapshot<PhotoSettings> options,
            IPhotoService photoService
        ) {
            this.photoRepository = photoRepository;

            this.photoSettings = options.Value;
            this.mapper = mapper;

            this.repository = repository;
            this.host = host;

            this.photoService = photoService;

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
            var photo = await photoService.UploadPhoto (vehicle, file, uploadsFolderPath);

            return Ok (mapper.Map<Photo, PhotoResource> (photo));

        }

        [HttpGet]
        public async Task<IEnumerable<PhotoResource>> GetPhotos (int vehicleId) {

            var photos = await photoRepository.GetPhotos (vehicleId);

            return mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>> (photos);
        }

    }
}