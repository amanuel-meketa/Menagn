import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppActiveTemplatesComponent } from './app-active-templates.component';

describe('AppActiveTemplatesComponent', () => {
  let component: AppActiveTemplatesComponent;
  let fixture: ComponentFixture<AppActiveTemplatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppActiveTemplatesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppActiveTemplatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
