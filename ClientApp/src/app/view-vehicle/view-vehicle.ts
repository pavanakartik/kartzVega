import { ToastyService } from 'ng2-toasty';

import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VehicleService } from '../services/vehicle.service';
import { PhotoService } from '../services/photo.service';
import { FileUploader } from 'ng2-file-upload';


@Component({
  templateUrl: 'view-vehicle.html'
})



export class ViewVehicleComponent implements OnInit {





  @ViewChild('fileInput', { static: false }) fileInput: ElementRef;
  vehicle: any;
  vehicleId: number;
  selectedFile: File = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toasty: ToastyService,
    private photoService: PhotoService,
    private vehicleService: VehicleService) {

    route.params.subscribe(p => {
      this.vehicleId = +p['id'];
      if (isNaN(this.vehicleId) || this.vehicleId <= 0) {
        router.navigate(['/vehicles']);
        return;
      }
    });
  }

  ngOnInit() {
    this.vehicleService.getVehicle(this.vehicleId)
      .subscribe(
        v => this.vehicle = v,
        err => {
          if (err.status == 404) {
            this.router.navigate(['/vehicles']);
            return;
          }
        });
  }

  delete() {
    if (confirm("Are you sure?")) {
      this.vehicleService.delete(this.vehicle.id)
        .subscribe(x => {
          this.router.navigate(['/vehicles']);
        });
    }
  }

  uploadPhoto(event) {

    this.selectedFile = <File>event.target.files[0];



    console.log(event)

    this.photoService.upload(this.vehicleId, this.selectedFile);
    /*  var nativeElement: any = this.fileInput.nativeElement;
 
     console.log(nativeElement.files[0].name);
 
     this.photoService.upload(this.vehicleId, nativeElement.files)
       .subscribe(x => console.log(x)); */
  }
}



