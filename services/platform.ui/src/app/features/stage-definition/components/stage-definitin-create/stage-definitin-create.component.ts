import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';

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
  imports: [ CommonModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzSelectModule, NzCardModule,NzButtonModule,
             NzAffixModule, NzCollapseModule, NzStepsModule ],
  templateUrl: './stage-definitin-create.component.html',
  styleUrl: './stage-definitin-create.component.css'
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

  get stages(): FormArray<FormGroup> {
    return this.form.get('stages') as FormArray<FormGroup>;
  }

  get stageGroups(): FormGroup[] {
    return this.stages.controls as FormGroup[];
  }

  addStage(): void {
    const index = this.stages.length + 1;
    this.stages.push(this.fb.group({
      stageName: ['', Validators.required],
      description: [''],
      sequenceOrder: [index, Validators.required],
      assignmentType: ['', Validators.required],
      assignmentKey: ['', Validators.required]
    }));
  }

  removeStage(index: number): void {
    this.stages.removeAt(index);
    this.stages.controls.forEach((group, i) =>
      group.get('sequenceOrder')!.setValue(i + 1)
    );
  }

  loadTemplates(): void {
    this.templateService.getAppTemplateList().subscribe({
      next: (data) => {
        this.templates = data.map(t => ({ id: t.templateId, name: t.name }));
      },
      error: (err) => {
        console.error('Failed to load templates', err);
        this.message.error('Could not load templates');
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.message.error('Please fill all required fields');
      return;
    }

    const payload = this.form.getRawValue();
    if (!payload.templateId) {
      this.message.error('Please select a template');
      return;
    }

    forkJoin(
      (payload.stages as AddStageDefiModel[]).map(stage =>
        this.stageService.addStage({ ...stage, templateId: payload.templateId ?? '' })
      )
    ).subscribe({
      next: response => this.handleSuccess(response),
      error: error => this.handleError(error)
    });
  }

  private handleSuccess(response: any): void {
    this.message.success('Stages created successfully!');
    console.log('Stages Created:', response);
    this.router.navigate(['/stage-list']);
    this.form.reset();
  }

  private handleError(error: any): void {
    const msg = error?.message || 'Unknown error occurred';
    this.message.error(`Operation failed: ${msg}`);
    console.error('Operation failed:', error);
  }
}
