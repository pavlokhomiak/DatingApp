import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// service can be injected
@Injectable({
  providedIn: 'root'
})
// services is better then components to store state 
export class AccountService {

  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  login(model: any) : Observable<any> {
    return this.http.post(this.baseUrl + 'account/login', model);
  }
}
