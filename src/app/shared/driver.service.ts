  import { Injectable } from '@angular/core';
  import { HttpClient, HttpHeaders } from '@angular/common/http';
  import { Driver } from 'src/app/driver';
  import { Observable, throwError } from 'rxjs';
  import { retry, catchError } from 'rxjs/operators';
  
  @Injectable({
    providedIn: 'root'
  })
  
  export class DriverService {
  
    // Base url
    baseurl = 'https://localhost:5001/drivers';
  
    constructor(private http: HttpClient) { }
  
    // Http Headers
    httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    }
  
  
    // GET
    GetIssues(): Observable<Driver> {
      return this.http.get<Driver>(this.baseurl)
      .pipe(
        retry(1),
        catchError(this.errorHandl)
      )
    }
    errorHandl(error) {
      let errorMessage = '';
      if(error.error instanceof ErrorEvent) {
        // Get client-side error
        errorMessage = error.error.message;
      } else {
        // Get server-side error
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
      console.log(errorMessage);
      return throwError(errorMessage);
   }
    
  
    
  
    
  
  }
