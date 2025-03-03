import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { GetRoleList } from '../../../models/Role/GetRoleList';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:9090/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  getRoleList(): Observable<GetRoleList[]> {
    return this.http.get<GetRoleList[]>(`${this.baseUrl}/role`, { headers: this.jsonHeaders });
  }

  deleteRole(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/role/id?id=${userId}`);
  }
}
