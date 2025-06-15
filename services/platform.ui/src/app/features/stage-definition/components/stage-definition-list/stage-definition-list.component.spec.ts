import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageDefinitionListComponent } from './stage-definition-list.component';

describe('StageDefinitionListComponent', () => {
  let component: StageDefinitionListComponent;
  let fixture: ComponentFixture<StageDefinitionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageDefinitionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageDefinitionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
