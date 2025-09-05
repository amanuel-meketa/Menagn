import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageProgressComponent } from './stage-progress.component';

describe('StageProgressComponent', () => {
  let component: StageProgressComponent;
  let fixture: ComponentFixture<StageProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageProgressComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
