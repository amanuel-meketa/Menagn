import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StartAppInstanceComponent } from './start-app-instance.component';

describe('StartAppInstanceComponent', () => {
  let component: StartAppInstanceComponent;
  let fixture: ComponentFixture<StartAppInstanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StartAppInstanceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StartAppInstanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
