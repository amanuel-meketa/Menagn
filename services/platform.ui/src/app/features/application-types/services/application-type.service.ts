import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { GetAppTypeModel } from '../../../models/Application-Type/GetAppTypeModel';
import { UpdateAppTypeMode } from '../../../models/Application-Type/UpdateAppTypeMode';

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

  getAppDetails(appId: string): Observable<GetAppTypeModel> {
    return this.http.get<GetAppTypeModel>(`${this.baseUrl}/approvals-type/${appId}`);
  }
  
  updateAppType(id: string, appData: UpdateAppTypeMode): Observable<any> {
    return this.http.put(`${this.baseUrl}/approvals-type/${id}`, appData).pipe(
      tap(() => { this.AppTypeListUpdated.next(true); }) );
  }  
}

