import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppTemplateCreateComponent } from './app-template-create.component';

describe('AppTemplateCreateComponent', () => {
  let component: AppTemplateCreateComponent;
  let fixture: ComponentFixture<AppTemplateCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppTemplateCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppTemplateCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
