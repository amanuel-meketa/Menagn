import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApprovalRequest } from '../../../models/Approval-Instance/ApprovalRequest';

@Injectable({
  providedIn: 'root'
})
export class AppInstanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost/approvals/approval-instance';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  startApprovalInstance(data: ApprovalRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/start`, data, { headers: this.jsonHeaders });
  }
}
