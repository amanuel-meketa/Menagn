import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationTypeListComponent } from './application-type-list.component';

describe('ApplicationTypeLissComponent', () => {
  let component: ApplicationTypeListComponent;
  let fixture: ComponentFixture<ApplicationTypeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationTypeListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
