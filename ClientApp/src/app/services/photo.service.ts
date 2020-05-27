import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {

  photos: any;
  constructor(private http: HttpClient) { }


  upload(vehicleId, photo) {
    var formData = new FormData();
    formData.append('file', photo, photo.name);

    console.log(photo);

    return this.http.post(`/api/vehicles/${vehicleId}/photos`, formData
    );
  }


  getPhotos(vehicleId) {


    return this.http.get(`/api/vehicles/${vehicleId}/photos`);
  }
}
