using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Resources;
using vega.Core;
using vega.Core.Models;
using vega.Models;

namespace vega.Controllers {

    [Route ("api/vehicles")]
    public class VehiclesController : Controller {

        private readonly IVehicleRepository repository;
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;
        public VehiclesController (IMapper mapper, IVehicleRepository repository, IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
            this.repository = repository;

            this.mapper = mapper;

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVehicle ([FromBody] SaveVehicleResource vehicleResource) {

            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            // validation which is kind of a overkill  will get through this later
            /*var model=  await context.Models.FindAsync(vehicleResource.ModelId);

            if(model==null){
                ModelState.AddModelError("ModelId","Invalid modelId.");
                return BadRequest(ModelState);eb
            }*/

            var vehicle = mapper.Map<SaveVehicleResource, Vehicle> (vehicleResource);
            vehicle.LastUpdate = DateTime.Now;

            repository.Add (vehicle);

            await unitOfWork.CompleteAsync ();

            vehicle = await repository.GetVehicle (vehicle.Id);
            var result = mapper.Map<Vehicle, VehicleResource> (vehicle);

            return Ok (result);

        }

        [HttpPut ("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicle (int id, [FromBody] SaveVehicleResource vehicleResource) {

            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            var vehicle = await repository.GetVehicle (id);
            if (vehicle == null)
                return NotFound ();

            mapper.Map<SaveVehicleResource, Vehicle> (vehicleResource, vehicle);
            vehicle.LastUpdate = DateTime.Now;

            await unitOfWork.CompleteAsync ();

            vehicle = await repository.GetVehicle (vehicle.Id);
            var result = mapper.Map<Vehicle, VehicleResource> (vehicle);

            return Ok (result);

        }

        [HttpDelete ("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle (int id) {

            var vehicle = await repository.GetVehicle (id, includeRelated : false);

            if (vehicle == null)
                return NotFound ();
            repository.Remove (vehicle);

            await unitOfWork.CompleteAsync ();
            return Ok (id);
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> GetVehicle (int id) {

            var vehicle = await repository.GetVehicle (id);

            if (vehicle == null)
                return NotFound ();
            var vehicleResource = mapper.Map<Vehicle, VehicleResource> (vehicle);
            return Ok (vehicleResource);
        }

        [HttpGet]
        public async Task<QueryResultResource<VehicleResource>> GetVehicles (VehicleQueryResource filterResource) {
            var filter = mapper.Map<VehicleQueryResource, VehicleQuery> (filterResource);

            var queryResult = await repository.GetVehicles (filter);

            return mapper.Map<QueryResult<Vehicle>, QueryResultResource<VehicleResource>> (queryResult);
        }

    }

}