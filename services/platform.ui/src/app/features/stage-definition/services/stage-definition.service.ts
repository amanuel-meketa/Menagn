import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { GetStageDefiModel } from '../../../models/Stage-Definition/GetStageDefiModel';
import { UpdateStageDefiModel } from '../../../models/Stage-Definition/UpdateStageDefiModel';
import { AddStageDefiModel } from '../../../models/Stage-Definition/AddStageDefiModel';

@Injectable({
  providedIn: 'root'
})

export class StageDefinitionService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:8000/approvals/stage-definition';
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });
 
  private StageDefiListUpdated = new BehaviorSubject<boolean>(false);

  get AppTypeListUpdated$(): Observable<boolean> {
    return this.StageDefiListUpdated.asObservable();
  }

  getStageDefiList(): Observable<GetStageDefiModel[]> {
    return this.http.get<GetStageDefiModel[]>(`${this.baseUrl}`, { headers: this.jsonHeaders });
  }

  getStageDefiDetails(stageId: string): Observable<GetStageDefiModel> {
    return this.http.get<GetStageDefiModel>(`${this.baseUrl}/${stageId}`);
  }
  
  updateStageDefi(id: string, stageData: UpdateStageDefiModel): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, stageData).pipe( tap(() => { this.StageDefiListUpdated.next(true); }) );
  } 

  getStagesByTempId(tempId: string): Observable<GetStageDefiModel[]> {
    return this.http.get<GetStageDefiModel[]>(`${this.baseUrl}/${tempId}/stages`, {
      headers: this.jsonHeaders
    });
  } 
  
  addStages(stages: AddStageDefiModel[]): Observable<void> {
    return this.http.post<void>(this.baseUrl, stages, { headers: this.jsonHeaders });
  }
  
  assignedTasks(userId: string): Observable<GetStageDefiModel[]> { 
    return this.http.get<GetStageDefiModel[]>( `${this.baseUrl}/user/${userId}/assigned-task`, { headers: this.jsonHeaders });
  }
  
}
