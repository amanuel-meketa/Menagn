import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterPostData } from '../../../models/RegisterPostData';
import { UserListData } from '../../../models/UserListData';
import { LoginPostData } from '../../../models/LoginPostData';

@Injectable({
  providedIn: 'root'
})
export class UserService {
    private readonly baseUrl = 'http://localhost:5087/api';
    http = inject(HttpClient)

    loginPostData(postData: LoginPostData): Observable<any>
    {
      const url = `${this.baseUrl}/account/log-in`;
      return this.http.post(url, postData);
    }
    
    registerUser(postData: RegisterPostData): Observable<any>
    {
      const url = `${this.baseUrl}/user`;
      return this.http.post(url, postData);
    }

    userList(): Observable<UserListData[]> {
      const url = `${this.baseUrl}/user`;
      return this.http.get<UserListData[]>(url);
    }

    deleteUser(userId: string): Observable<any> {
      const url = `${this.baseUrl}/user/${userId}`;
      return this.http.delete(url);
    }    
} 