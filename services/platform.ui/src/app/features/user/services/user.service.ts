import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterPostData } from '../models/RegisterPostData';

@Injectable({
  providedIn: 'root'
})
export class UserService {
    private readonly baseUrl = 'http://localhost:5087/api';
    constructor(private http: HttpClient) {}
  
    registerUser(postData: RegisterPostData): Observable<any>
    {
      const url = `${this.baseUrl}/user`;
      return this.http.post(url, postData);
    }
} 