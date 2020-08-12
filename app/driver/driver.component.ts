import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { DriverService } from 'src/app/shared/driver.service';

@Component({
  selector: 'app-driver',
  templateUrl: './driver.component.html',
  styleUrls: ['./driver.component.css']
})
export class DriverComponent implements OnInit {
  drivers: any = [];
  constructor(public driverService: DriverService) { }

  ngOnInit() {
    this.loadDrivers();
    // this.apiService.getDrivers().subscribe((data)=>{
    //   console.log(data);
    //   this.drivers = data['drivers'];
    // });}
  }
  loadDrivers(){
    return this.driverService.GetIssues().subscribe((data: {}) => {
      this.drivers = data;
 
  })
}
}

