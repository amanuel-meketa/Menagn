import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageDefinitionUpdateComponent } from './stage-definition-update.component';

describe('StageDefinitionUpdateComponent', () => {
  let component: StageDefinitionUpdateComponent;
  let fixture: ComponentFixture<StageDefinitionUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageDefinitionUpdateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageDefinitionUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
