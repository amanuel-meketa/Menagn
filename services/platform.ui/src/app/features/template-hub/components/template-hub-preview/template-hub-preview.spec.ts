import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateHubPreview } from './template-hub-preview';

describe('TemplateHubPreview', () => {
  let component: TemplateHubPreview;
  let fixture: ComponentFixture<TemplateHubPreview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemplateHubPreview]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemplateHubPreview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
