import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterPostData } from '../../../models/RegisterPostData';
import { UserListData } from '../../../models/UserListData';
import { LoginPostData } from '../../../models/LoginPostData';

@Injectable({  providedIn: 'root' })

export class UserService { [x: string]: any;
  private readonly baseUrl = 'http://localhost:9090/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  constructor(private readonly http: HttpClient) {}

  login(postData: LoginPostData): Observable<any> {
    return this.http.post(`${this.baseUrl}/account/log-in`, postData, { headers: this.jsonHeaders });
  }

  registerUser(postData: RegisterPostData): Observable<any> {
    return this.http.post(`${this.baseUrl}/user`, postData, { headers: this.jsonHeaders });
  }

  getUserList(): Observable<UserListData[]> {
    return this.http.get<UserListData[]>(`${this.baseUrl}/user`);
  }

  getUserDetails(userId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/user/${userId}`);
  }

  updateUser(userId: string, updatedData: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/user/${userId}`, updatedData, { headers: this.jsonHeaders });
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/user/${userId}`);
  }

  resetPassword(userId: string, newPassword: string) {
    return this.http.put(`${this.baseUrl}/user/${userId}/reset-password`, JSON.stringify(newPassword), { headers: this.jsonHeaders }
    );
  }
}
