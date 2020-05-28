import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  data = {

    labels: ['BMW', 'AUDI', 'MAZDA'],
    datasets: [

      {
        data: [5, 10, 1],
        backgroundColor: [
          "#ff6384",
          "#36a2eb",
          "#ffce6"
        ]
      }
    ]

  };
  constructor() { }

  ngOnInit() {
  }

}
