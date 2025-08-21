// active-templates.component.ts
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { Subject, takeUntil } from 'rxjs';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTemplateService } from '../../services/app-template.service';
import { StartAppInstanceComponent } from '../../../app-instance/components/start-app-instance/start-app-instance.component';
import { NzEmptyModule } from 'ng-zorro-antd/empty';

@Component({
  selector: 'app-active-templates',
  standalone: true,
  imports: [ CommonModule, FormsModule, NzButtonModule, NzCardModule, NzInputModule, NzIconModule, NzModalModule,
             NzEmptyModule, StartAppInstanceComponent ],
  templateUrl: './app-active-templates.component.html',
  styleUrl: './app-active-templates.component.css'
})
export class ActiveTemplatesComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly destroy$ = new Subject<void>();
  private readonly modal = inject(NzModalService);
  
  listOfData: (GetAppTypeModel & { showFullDescription?: boolean })[] = [];
  searchQuery = '';

  ngOnInit(): void {
    this.loadAppTypeList();

    this._appTemplateService.AppTypeListUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadAppTypeList());
  }

  private loadAppTypeList(): void {
    this._appTemplateService.getAppTemplateList().subscribe({
      next: (data: GetAppTypeModel[]) => {
        console.log('Loaded templates:', data); // << check this
        this.listOfData = data.map(t => ({ ...t, showFullDescription: false }));
        this.cdr.detectChanges();
      },
      error: () => this.message.error('Failed to load templates')
    });
  }
  

  filteredTemplates(): (GetAppTypeModel & { showFullDescription?: boolean })[] {
    if (!this.searchQuery) return this.listOfData;
    return this.listOfData.filter(t =>
      t.name.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  /** Open Start Instance modal for selected template */
  openStartInstance(templateId: string): void {
    this.modal.create({
      nzTitle: 'Start Template Instance',
      nzContent: StartAppInstanceComponent,
      nzFooter: null,
      nzWidth: 600,
      nzMaskClosable: false,
      ...( { nzComponentParams: { templateId } } as any )
    });
    
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
