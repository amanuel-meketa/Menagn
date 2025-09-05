import { Component, inject, Input, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';
import { StageDefinitionService } from '../../services/stage-definition.service';
import { finalize } from 'rxjs/operators';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzCardModule } from 'ng-zorro-antd/card';

@Component({
  selector: 'app-stage-progress',
  standalone: true,
  imports: [ CommonModule, NzButtonModule, NzIconModule, NzModalModule, NzStepsModule, NzTagModule, NzSkeletonModule,
            NzToolTipModule, NzProgressModule, NzCardModule ],
  templateUrl: './stage-progress.component.html',
  styleUrl: './stage-progress.component.css'
})
export class StageProgressComponent implements OnDestroy {
  private readonly _stageDefiService = inject(StageDefinitionService);
  private readonly cdr = inject(ChangeDetectorRef);

  @Input() templateId: string | null | undefined = null;
  @Input() currentStageOrder?: number | null = null; // 1-based

  isVisible = false;
  loading = false;
  error: string | null = null;

  stages: GetStageDefiModel[] = [];
  index = 0; // 0-based
  private highlightTimer: any = null;
  private lastPinnedEl: Element | null = null;

  // open modal and fetch stages
  openModal(): void {
    if (!this.templateId) {
      this.error = 'Template id missing';
      console.warn('Template ID is not set!');
      return;
    }

    this.loading = true;
    this.error = null;
    this._stageDefiService.getStagesByTempId(this.templateId)
      .pipe(finalize(() => { this.loading = false; this.cdr.markForCheck(); }))
      .subscribe({
        next: (data) => {
          this.stages = (data || []).slice().sort((a, b) => (a.sequenceOrder ?? 0) - (b.sequenceOrder ?? 0));

          if (this.currentStageOrder != null && !isNaN(Number(this.currentStageOrder))) {
            const desired = Number(this.currentStageOrder) - 1;
            this.index = Math.max(0, Math.min(desired, Math.max(0, this.stages.length - 1)));
          } else {
            const inProgress = this.stages.findIndex(s => this.isInProgress(s));
            this.index = inProgress >= 0 ? inProgress : (this.stages.length ? 0 : 0);
          }

          this.isVisible = true;
          this.scheduleHighlight();
        },
        error: (err) => {
          console.error('Failed to fetch stages', err);
          this.error = 'Failed to load stages';
          this.stages = [];
          this.isVisible = true;
        }
      });
  }

  handleCancel(): void {
    this.isVisible = false;
    this.clearPinned();
    this.stages = [];
    this.index = 0;
    this.error = null;
  }

  onIndexChange(event: any): void {
    const i = typeof event === 'number' ? event : (event?.value ?? event?.index ?? event);
    this.index = Math.max(0, Number(i) || 0);
    this.scheduleHighlight();
  }

  prev(): void { if (this.index > 0) { this.index--; this.scheduleHighlight(); } }
  next(): void { if (this.index < this.stages.length - 1) { this.index++; this.scheduleHighlight(); } }

  // schedule DOM highlight so steps render first
  private scheduleHighlight(): void {
    if (this.highlightTimer) clearTimeout(this.highlightTimer);
    this.highlightTimer = setTimeout(() => { this.highlightCurrentStep(); this.highlightTimer = null; }, 80);
  }

  private clearPinned(): void {
    if (this.lastPinnedEl) { this.lastPinnedEl.classList.remove('instance-current'); this.lastPinnedEl = null; }
  }

  private highlightCurrentStep(): void {
    this.clearPinned();
    try {
      const steps = Array.from(document.querySelectorAll('.ant-steps-item'));
      if (!steps.length) return;
      const idx = Math.max(0, Math.min(this.index, steps.length - 1));
      const el = steps[idx] as HTMLElement | undefined;
      if (el) {
        el.classList.add('instance-current');
        this.lastPinnedEl = el;
        // scroll container horizontally if applicable
        const container = el.closest('.ant-steps') as HTMLElement | null;
        if (container) {
          const offsetLeft = el.offsetLeft - (container.clientWidth / 2) + (el.clientWidth / 2);
          container.scrollTo({ left: Math.max(0, offsetLeft), behavior: 'smooth' });
        } else {
          el.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
        }
      }
    } catch (err) {
      console.warn('highlightCurrentStep error', err);
    }
  }

  get selectedStage(): GetStageDefiModel & { updatedAt?: string | null; createdOn?: string | null } {
    return this.stages[this.index] || {
      stageDefId: '',
      templateId: '',
      stageName: '',
      description: '',
      sequenceOrder: 0,
      assignmentType: '',
      assignmentKey: '',
      status: '',
      isCritical: '',
      updatedAt: null,
      createdOn: null
    };
  }

  mapStatusToNzStep(stage: any): 'wait' | 'process' | 'finish' | 'error' {
    const s = (stage?.status || '').toString().toLowerCase();
    if (this.isInProgress(stage)) return 'process';
    if (['approved', 'completed', 'done'].includes(s)) return 'finish';
    if (['rejected', 'failed', 'error'].includes(s)) return 'error';
    return 'wait';
  }

  isCompleted(stage: any): boolean {
    const s = (stage?.status || '').toString().toLowerCase();
    return ['approved', 'completed', 'done'].includes(s);
  }
  isInProgress(stage: any): boolean {
    const s = (stage?.status || '').toString().toLowerCase();
    return ['inprogress', 'in progress', 'processing', 'active'].includes(s);
  }

  formatDate(iso?: string | null | undefined): string {
    if (!iso) return '';
    try { const d = new Date(iso); if (isNaN(d.getTime())) return String(iso); return d.toLocaleString(); } catch { return String(iso); }
  }

  // trackBy helps rendering lists
  trackByStage(index: number, item: GetStageDefiModel) { return item.stageDefId || item.sequenceOrder || index; }


  getStatusColorForStage(stage: any): string {
    const s = (stage?.status || '').toString().toLowerCase();
    if (['approved','completed','done'].includes(s)) return 'green';
    if (this.isInProgress(stage)) return 'blue';
    if (['pending'].includes(s)) return 'orange';
    if (['rejected','failed','error'].includes(s)) return 'red';
    return 'default';
  }

    // Current stage in workflow (not selected index)
  get currentStage(): GetStageDefiModel | null {
    if (!this.stages || this.stages.length === 0 || !this.currentStageOrder) return null;
    return this.stages[this.currentStageOrder - 1] || null;
  }

  // Current stage name
  get currentStageName(): string {
    return this.currentStage?.stageName || '—';
  }

  ngOnDestroy(): void {
    if (this.highlightTimer) { clearTimeout(this.highlightTimer); this.highlightTimer = null; }
    this.clearPinned();
  }
}

