import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { GetAppTypeModel } from '../../../models/Application-Type/GetAppTypeModel';

@Injectable({
  providedIn: 'root'
})
export class ApplicationTypeService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost/approvals/api';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  private AppTypeListUpdated = new BehaviorSubject<boolean>(false);

  get AppTypeListUpdated$(): Observable<boolean> {
    return this.AppTypeListUpdated.asObservable();
  }

  getAppTypeList(): Observable<GetAppTypeModel[]> {
    return this.http.get<GetAppTypeModel[]>(`${this.baseUrl}/approvals-type`, { headers: this.jsonHeaders });
  }
}
