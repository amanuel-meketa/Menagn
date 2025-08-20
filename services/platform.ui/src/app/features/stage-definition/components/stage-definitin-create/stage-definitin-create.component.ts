import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzAffixModule } from 'ng-zorro-antd/affix';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzMessageService } from 'ng-zorro-antd/message';

import { StageDefinitionService } from '../../services/stage-definition.service';
import { AppTemplateService } from '../../../app-template/services/app-template.service';
import { AddStageDefiModel } from '../../../../models/Stage-Definition/AddStageDefiModel';

@Component({
  selector: 'app-stage-definitin-create',
  standalone: true,
  imports: [ CommonModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzSelectModule, NzCardModule,
             NzButtonModule,NzAffixModule, NzCollapseModule, NzStepsModule],
  templateUrl: './stage-definitin-create.component.html',
  styleUrls: ['./stage-definitin-create.component.css']
})
export class StageDefinitinCreateComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly stageService = inject(StageDefinitionService);
  private readonly templateService = inject(AppTemplateService);
  private readonly message = inject(NzMessageService);
  private readonly router = inject(Router);

  templates: { id: string; name: string }[] = [];

  readonly form = this.fb.group({
    templateId: ['', Validators.required],
    stages: this.fb.array<FormGroup>([])
  });

  ngOnInit(): void {
    this.loadTemplates();
    this.addStage();
  }

  // typed getters for template usage
  get stages(): FormArray {
    return this.form.get('stages') as FormArray;
  }
  get stageGroups(): FormGroup[] {
    return this.stages.controls as FormGroup[];
  }

  // stable id generator (fallback-safe)
  private makeId(): string {
    if (typeof crypto !== 'undefined' && typeof (crypto as any).randomUUID === 'function') {
      return (crypto as any).randomUUID();
    }
    return `${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 9)}`;
  }

  addStage(): void {
    this.stages.push(
      this.fb.group({
        id: [this.makeId()],
        stageName: ['', Validators.required],
        description: [''],
        assignmentType: ['', Validators.required],
        assignmentKey: ['', Validators.required]
      })
    );
  }

  removeStage(index: number): void {
    this.stages.removeAt(index);
  }

  // wrappers to prevent accidental submit (extra safety)
  onAdd(event: Event): void {
    event.preventDefault();
    this.addStage();
  }
  onRemove(event: Event, idx: number): void {
    event.preventDefault();
    this.removeStage(idx);
  }

  trackByStageId(_index: number, group: FormGroup): string {
    return group.get('id')?.value ?? String(_index);
  }

  loadTemplates(): void {
    this.templateService.getAppTemplateList().subscribe({
      next: (data) => {
        this.templates = data.map(t => ({ id: t.templateId, name: t.name }));
      },
      error: () => this.message.error('Could not load templates')
    });
  }

  // header helpers to keep template tidy and easily testable
  headerTitle(group: FormGroup, idx: number): string {
    const name = group.get('stageName')?.value || 'Unnamed';
    return `Stage ${idx + 1} • ${name}`;
  }

  assignmentBadgeText(group: FormGroup): string {
    const type = group.get('assignmentType')?.value;
    const key = group.get('assignmentKey')?.value;
    if (!type) { return 'Unassigned'; }
    return key ? `${type}: ${key}` : type;
  }

  submit(): void {
    if (this.form.invalid) {
      this.message.error('Please fix validation errors before submitting.');
      this.form.markAllAsTouched();
      return;
    }

    const templateId = this.form.get('templateId')!.value;
    if (!templateId) {
      this.message.error('Please select a template');
      return;
    }

    const payload: AddStageDefiModel[] = this.stageGroups.map((group, idx) => ({
      stageName: group.get('stageName')!.value,
      description: group.get('description')!.value,
      assignmentType: group.get('assignmentType')!.value,
      assignmentKey: group.get('assignmentKey')!.value,
      SequenceOrder: Number(idx + 1),
      templateId
    }));

    this.stageService.addStages(payload).subscribe({
      next: () => {
        this.message.success('Stages created successfully!');
        this.router.navigate(['/stage-list']);
      },
      error: (err) => {
        this.message.error(`Operation failed: ${err?.message ?? 'Unknown error'}`);
        console.error(err);
      }
    });
  }
}
