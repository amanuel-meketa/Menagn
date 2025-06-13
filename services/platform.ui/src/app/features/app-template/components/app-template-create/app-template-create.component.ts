import { Component, TemplateRef, ViewChild, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService, NzModalRef } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';

import { AppTemplateService } from '../../services/app-template.service';
import { CreateAppTemplateModel } from '../../../../models/Application-Type/CreateAppTemplateModel';

@Component({
  selector: 'app-app-template-create',
  standalone: true,
  imports: [NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule],
  templateUrl: './app-template-create.component.html',
  styleUrl: './app-template-create.component.css'
})
export class AppTemplateCreateComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private modal = inject(NzModalService);
  private readonly message = inject(NzMessageService);
  private readonly router = inject(Router);
  private readonly appTemplateService = inject(AppTemplateService);

  validateForm = this.fb.group({
    name: ['', [Validators.required]],
    description: ['']
  });

  @ViewChild('appFormTemplate', { static: true }) appFormTemplate!: TemplateRef<any>;

  private modalRef?: NzModalRef;
  rout: any;

  /** Opens the template creation modal */
  openTemplateRegisterModal(): void {
    this.modalRef = this.modal.create({
      nzTitle: 'Register Template',
      nzContent: this.appFormTemplate,
      nzFooter: null,
      nzWidth: 600
    });
  }

  /** Closes the modal */
  cancel(): void {
    this.modal.closeAll();
  }

  /** Handles form submission */
  submitForm(): void {
    if (this.validateForm.invalid) return;

    const newTemplate: CreateAppTemplateModel = {
      name: this.validateForm.value.name?.trim() || '',
      description: this.validateForm.value.description?.trim() || ''
    };

    this.createTemplate(newTemplate);
  }

  /** Sends request to create template */
  private createTemplate(data: CreateAppTemplateModel): void {
    this.appTemplateService.createAppTemplate(data).subscribe({
      next: (response) => this.handleSuccess(response),
      error: (error) => this.handleError(error)
    });
  }

  /** Success callback for create operation */
  private handleSuccess(response: any): void {
    this.message.remove();
    this.message.success(`Template "${response.name}" created successfully!`);
    this.cancel(); // Close modal
    this.validateForm.reset(); // Reset form
    this.router.navigate(['/app-template-list']);
  }

  /** Error callback for create operation */
  private handleError(error: any): void {
    this.message.remove();
    const errorMessage = error?.message || 'Unknown error occurred';
    this.message.error(`Registration failed: ${errorMessage}`);
    console.error('Template creation failed:', error);
  }
}
