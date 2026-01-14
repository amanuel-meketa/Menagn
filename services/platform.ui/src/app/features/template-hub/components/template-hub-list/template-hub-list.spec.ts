import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateHubList } from './template-hub-list';

describe('TemplateHubList', () => {
  let component: TemplateHubList;
  let fixture: ComponentFixture<TemplateHubList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemplateHubList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemplateHubList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
