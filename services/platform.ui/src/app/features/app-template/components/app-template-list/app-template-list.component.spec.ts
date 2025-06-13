import { ComponentFixture, TestBed } from '@angular/core/testing';

import {AppTemplateListComponent } from './app-template-list.component';

describe('ApplicationTypeLissComponent', () => {
  let component: AppTemplateListComponent;
  let fixture: ComponentFixture<AppTemplateListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppTemplateListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppTemplateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
