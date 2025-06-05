import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { GetAppTypeModel } from '../../../models/Application-Type/GetAppTypeModel';

@Injectable({
  providedIn: 'root',
})
export class AppTypesharedService {
  private appTypeSubject = new BehaviorSubject<GetAppTypeModel | null>(null);
  currentAppType$ = this.appTypeSubject.asObservable();

  setAppType(data: GetAppTypeModel): void {
    this.appTypeSubject.next(data);
  }
}