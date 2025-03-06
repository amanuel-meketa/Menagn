import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { GetRole } from '../../../models/User/GetUserRole';
import { CreateRole } from '../../../models/Role/CreateRole';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:9090/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  private roleListUpdated = new BehaviorSubject<boolean>(false);

  get roleListUpdated$(): Observable<boolean> {
    return this.roleListUpdated.asObservable();
  }

  getRoleList(): Observable<GetRole[]> {
    return this.http.get<GetRole[]>(`${this.baseUrl}/role`, { headers: this.jsonHeaders });
  }

  getroleDetails(roleId: string): Observable<GetRole> {
    const params = new HttpParams().set('id', roleId);
    return this.http.get<GetRole>(`${this.baseUrl}/role/id`, { params });
  }  

  createRole(role: CreateRole): Observable<CreateRole> {
    return this.http.post<CreateRole>(`${this.baseUrl}/role`, role).pipe(
      tap(() => {
        this.roleListUpdated.next(true);
      })
    );
  }
  
  updateRole(id: string, roleData: CreateRole): Observable<any> {
    const params = new HttpParams().set('id', id);
    return this.http.put(`${this.baseUrl}/role/id`, roleData, { params }).pipe(
      tap(() => {
        this.roleListUpdated.next(true);
      })
    );
  }

  deleteRole(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/role/id?id=${userId}`).pipe(
      tap(() => {
        this.roleListUpdated.next(true);
      })
    );
  }
}
