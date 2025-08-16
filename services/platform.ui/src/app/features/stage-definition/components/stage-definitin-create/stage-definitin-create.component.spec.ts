import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageDefinitinCreateComponent } from './stage-definitin-create.component';

describe('StageDefinitinCreateComponent', () => {
  let component: StageDefinitinCreateComponent;
  let fixture: ComponentFixture<StageDefinitinCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageDefinitinCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageDefinitinCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
