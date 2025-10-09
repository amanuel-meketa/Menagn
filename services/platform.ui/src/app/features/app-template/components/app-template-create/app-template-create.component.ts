import { Component, TemplateRef, ViewChild, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule,Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule,  NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';

import { AppTemplateService } from '../../services/app-template.service';
import { CreateAppTemplateModel } from '../../../../models/Application-Type/CreateAppTemplateModel';

@Component({
    selector: 'app-app-template-create',
    imports: [NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule],
    templateUrl: './app-template-create.component.html',
    styleUrl: './app-template-create.component.css'
})
export class AppTemplateCreateComponent {
  private fb = inject(NonNullableFormBuilder);
  private modal = inject(NzModalService);
  private message = inject(NzMessageService);
  private router = inject(Router);
  private appTemplateService = inject(AppTemplateService);

  @ViewChild('appFormTemplate', { static: true }) appFormTemplate!: TemplateRef<any>;

  validateForm = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  /** Open the modal and keep reference to it */
  openTemplateRegisterModal(): void {
    this.modal.create({
      nzTitle: 'Register Template',
      nzContent: this.appFormTemplate,
      nzFooter: null,
      nzWidth: 600,
      nzMaskClosable: false
    });
  }

  /** Submit form */
  submitForm(): void {
    if (this.validateForm.invalid) return;

    const newTemplate: CreateAppTemplateModel = {
      name: this.validateForm.value.name?.trim() || '',
      description: this.validateForm.value.description?.trim() || ''
    };

    this.appTemplateService.createAppTemplate(newTemplate).subscribe({
      next: (response) => this.onCreateSuccess(response),
      error: (error) => this.onCreateError(error)
    });
  }

  /** Handle successful creation */
  private onCreateSuccess(response: any): void {
    this.message.remove();
    this.message.success(`Template "${response.name}" created successfully!`);
    this.cancel();

    // Optionally navigate
    this.router.navigate(['/app-template-list']);
  }

  /** Handle error */
  private onCreateError(error: any): void {
    this.message.remove();
    const errMsg = error?.error?.message || error?.message || 'Unknown error occurred';
    this.message.error(`Creation failed: ${errMsg}`);
    console.error('Template creation failed:', error);
  }

  cancel(): void {
    this.modal.closeAll();
  }
}
