import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationTypeDetailsComponent } from './app-template-details.component';

describe('ApplicationTypeDetailsComponent', () => {
  let component: ApplicationTypeDetailsComponent;
  let fixture: ComponentFixture<ApplicationTypeDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationTypeDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationTypeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
