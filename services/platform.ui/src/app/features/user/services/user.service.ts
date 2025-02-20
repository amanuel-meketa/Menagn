import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterPostData } from '../../../models/RegisterPostData';
import { UserListData } from '../../../models/UserListData';
import { LoginPostData } from '../../../models/LoginPostData';
import { GetCurrentUser } from '../../../models/User/GetCurrentUser';
import { UserSession } from '../../../models/User/UserSession';
import { GetRole } from '../../../models/User/GetRole';

@Injectable({  providedIn: 'root' })

export class UserService { [x: string]: any;
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:9090/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  login(postData: LoginPostData): Observable<{ access_token: string; refresh_token: string; token_type: string }> 
  {
    return this.http.post<{ access_token: string; refresh_token: string; token_type: string }>(
         `${this.baseUrl}/account/log-in`,postData);
  }

  registerUser(postData: RegisterPostData): Observable<any> {
    return this.http.post(`${this.baseUrl}/user`, postData);
  }

  getUserList(): Observable<UserListData[]> {
    return this.http.get<UserListData[]>(`${this.baseUrl}/user`);
  }

  getUserDetails(userId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/user/${userId}`);
  }

  updateUser(userId: string, updatedData: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/user/${userId}`, updatedData);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/user/${userId}`);
  }

  resetPassword(userId: string, newPassword: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/user/${userId}/reset-password`, 
           JSON.stringify(newPassword), { headers: this.jsonHeaders }
    );}

  getCurrentUser(): Observable<GetCurrentUser> {
    return this.http.get<GetCurrentUser>(`${this.baseUrl}/account/userinfo`);
  }
  
  getUserSeesion(userId: string): Observable<UserSession[]> {
    return this.http.get<UserSession[]>(`${this.baseUrl}/user/${userId}/sessions`);
  }
  
  deleteUserSession(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/user/${userId}/remove-sessions`);
  }
  
  assignedUserRole(userId: string): Observable<GetRole[]> {
    return this.http.get<GetRole[]>(`${this.baseUrl}/user/${userId}/roles/assigned`);
  }

  unAssignedUserRole(userId: string): Observable<GetRole[]> {
    return this.http.get<GetRole[]>(`${this.baseUrl}/user/${userId}/roles/unassigned`);
  }
}
