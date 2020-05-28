
import { AuthService } from './services/auth.service';

import { ChartModule } from 'angular2-chartjs';

import { PhotoService } from './services/photo.service';
import { ViewVehicleComponent } from './view-vehicle/view-vehicle';

import { VehicleListComponent } from './vehicle-list/vehicle-list.component';
import { AppErrorHandler } from './app.error-handler';

import { VehicleService } from './services/vehicle.service';
import { VehicleFormComponent } from './vehicle-form/vehicle-form.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { ToastyModule } from 'ng2-toasty';
import { FileUploadModule } from 'ng2-file-upload';


import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { PaginationComponent } from './shared/pagination.component';

import { ProfileComponent } from './profile/profile.component';
import { AdminComponent } from './admin/admin.component';




@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent, VehicleFormComponent, VehicleListComponent, PaginationComponent, ViewVehicleComponent, ProfileComponent, AdminComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule, ChartModule,
    FormsModule, FileUploadModule,
    ToastyModule.forRoot(),
    RouterModule.forRoot([
      {
        path: 'profile',
        component: ProfileComponent
      },

      { path: '', redirectTo: 'vehicles', pathMatch: 'full' },


      { path: 'vehicles/new', component: VehicleFormComponent },
      { path: 'vehicles/edit/:id', component: VehicleFormComponent },
      { path: 'vehicles/:id', component: ViewVehicleComponent },
      { path: 'vehicles', component: VehicleListComponent },
      { path: 'home', component: HomeComponent },
      { path: 'admin', component: AdminComponent },

      { path: 'counter', component: CounterComponent },

      { path: 'fetch-data', component: FetchDataComponent },
      { path: '**', redirectTo: 'home' }
    ])
  ],
  providers: [VehicleService, PhotoService, AuthService, { provide: ErrorHandler, useClass: AppErrorHandler }],
  bootstrap: [AppComponent]
})
export class AppModule { }
