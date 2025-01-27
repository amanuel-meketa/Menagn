import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterPostData } from '../interfaces/users/register-post-data';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5087/api';
  constructor(private http: HttpClient) {}

  registerUser(postData: RegisterPostData): Observable<any> {
    const url = `${this.baseUrl}/user`;
    return this.http.post(url, postData);
  }
}