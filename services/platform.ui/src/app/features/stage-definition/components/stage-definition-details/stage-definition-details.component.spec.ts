import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageDefinitionDetailsComponent } from './stage-definition-details.component';

describe('StageDefinitionDetailsComponent', () => {
  let component: StageDefinitionDetailsComponent;
  let fixture: ComponentFixture<StageDefinitionDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageDefinitionDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageDefinitionDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
