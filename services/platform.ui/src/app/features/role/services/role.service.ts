import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetRole } from '../../../models/User/GetRole';
import { CreateRole } from '../../../models/Role/CreateRole';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:9090/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  getRoleList(): Observable<GetRole[]> {
    return this.http.get<GetRole[]>(`${this.baseUrl}/role`, { headers: this.jsonHeaders });
  }

  createRole(role: CreateRole): Observable<CreateRole> {
    return this.http.post<CreateRole>(`${this.baseUrl}/role`, role);
  }  

  deleteRole(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/role/id?id=${userId}`);
  }
}
