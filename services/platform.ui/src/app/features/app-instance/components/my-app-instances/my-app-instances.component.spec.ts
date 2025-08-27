import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyAppInstancesComponent } from './my-app-instances.component';

describe('MyAppInstancesComponent', () => {
  let component: MyAppInstancesComponent;
  let fixture: ComponentFixture<MyAppInstancesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyAppInstancesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyAppInstancesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
