import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { GetStageDefiModel } from '../../../models/Stage-Definition/GetStageDefiModel';

@Injectable({
  providedIn: 'root',
})
export class StageDefinitionSharedService {
  private stageDefinSubject = new BehaviorSubject<GetStageDefiModel | null>(null);
  currentstageDefin$ = this.stageDefinSubject.asObservable();

  setStageDefin(data: GetStageDefiModel): void {
    this.stageDefinSubject.next(data);
  }
}