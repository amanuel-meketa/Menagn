import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateStagesComponent } from './template-stages.component';

describe('TemplateStagesComponent', () => {
  let component: TemplateStagesComponent;
  let fixture: ComponentFixture<TemplateStagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemplateStagesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemplateStagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
