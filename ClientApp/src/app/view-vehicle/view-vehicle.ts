import { NgZone } from '@angular/core';
import { ToastyService } from 'ng2-toasty';

import { Component, OnInit, ElementRef, ViewChild, ɵConsole } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VehicleService } from '../services/vehicle.service';
import { PhotoService } from '../services/photo.service';
import { HttpEventType } from '@angular/common/http';



@Component({
  templateUrl: 'view-vehicle.html'
})



export class ViewVehicleComponent implements OnInit {





  @ViewChild('fileInput', { static: false }) fileInput: ElementRef;
  vehicle: any;
  vehicleId: number;
  selectedFile: File = null;
  photos: any;
  progress: number =0 ;

  constructor(

    private zone: NgZone,
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

    this.photoService.getPhotos(this.vehicleId).subscribe(photos => this.photos = photos);

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



    console.log(event);

    this.photoService.upload(this.vehicleId, this.selectedFile).subscribe(

      event => {

        if (event.type === HttpEventType.UploadProgress) {

          console.log('Upload progress: ' + Math.round((event.loaded / event.total) * 100) + "%");

         this.zone.run(() => { 
            this.progress = Math.round((event.loaded / event.total) * 100);

            console.log("My Photo Upload progress : " +

              this.progress);


        }) 

        }

        else if (event.type === HttpEventType.Response) {

          console.log(event);

          this.photos.push(event.body);

        }


      },
      null,
      () => { this.progress = null }











    );


  }
}



