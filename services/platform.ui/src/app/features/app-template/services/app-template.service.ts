import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { GetAppTypeModel } from '../../../models/Application-Type/GetAppTypeModel';
import { UpdateAppTypeMode } from '../../../models/Application-Type/UpdateAppTypeMode';
import { CreateAppTemplateModel } from '../../../models/Application-Type/CreateAppTemplateModel';

@Injectable({
  providedIn: 'root'
})
export class AppTemplateService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:8000/approvals/approval-template';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  private AppTempListUpdated = new BehaviorSubject<boolean>(false);

  get AppTypeListUpdated$(): Observable<boolean> {
    return this.AppTempListUpdated.asObservable();
  }

  getAppTemplateList(): Observable<GetAppTypeModel[]> {
    return this.http.get<GetAppTypeModel[]>(`${this.baseUrl}`, { headers: this.jsonHeaders });
  }

  getAppDetails(appId: string): Observable<GetAppTypeModel> {
    return this.http.get<GetAppTypeModel>(`${this.baseUrl}/${appId}`);
  }
  
  createAppTemplate(role: any): Observable<CreateAppTemplateModel> {
    return this.http.post<CreateAppTemplateModel>(`${this.baseUrl}`, role).pipe(
      tap(() => {
        this.AppTempListUpdated.next(true);
      }));
  }
  
  updateAppTemplate(id: string, appData: UpdateAppTypeMode): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, appData).pipe(
      tap(() => { this.AppTempListUpdated.next(true); }) );
  } 
  
  deleteAppTemplate(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${userId}`).pipe(tap(() => {
        this.AppTempListUpdated.next(true);
      }));
  }
}

