import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Driver } from './driver';
import { DriverComponent } from './driver/driver.component';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  apiURL: string = 'https://localhost:5001/drivers';
  public getDrivers(){
    return this.httpClient.get<Driver[]>(`${this.apiURL}`);
}
  constructor(private httpClient: HttpClient) { }
}