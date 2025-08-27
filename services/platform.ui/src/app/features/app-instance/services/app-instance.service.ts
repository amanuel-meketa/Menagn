import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApprovalRequest } from '../../../models/Approval-Instances/ApprovalRequest';
import { InstanceList } from '../../../models/Approval-Instances/InstanceList';
export type Guid = string;

@Injectable({
  providedIn: 'root'
})
export class AppInstanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost/approvals/approval-instance';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });
  
  getAllInstances(): Observable<InstanceList[]> {
    return this.http.get<InstanceList[]>(`${this.baseUrl}`, { headers: this.jsonHeaders });
  }

  getInstanceByTempId(tempId: string): Observable<InstanceList[]> {
    return this.http.get<InstanceList[]>(`${this.baseUrl}template/${tempId}/instances`, { headers: this.jsonHeaders });
  }  

  startApprovalInstance(data: ApprovalRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/start`, data, { headers: this.jsonHeaders });
  }
  
  getMyInstances(userId: Guid): Observable<InstanceList[]> {
    return this.http.get<InstanceList[]>(`${this.baseUrl}/user/${userId}/instances`, {
      headers: this.jsonHeaders
    });
  }
}
