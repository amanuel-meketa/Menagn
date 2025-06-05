import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationTypeUpdateComponent } from './application-type-update.component';

describe('ApplicationTypeUpdateComponent', () => {
  let component: ApplicationTypeUpdateComponent;
  let fixture: ComponentFixture<ApplicationTypeUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationTypeUpdateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationTypeUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
