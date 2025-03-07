import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleAssignedUsersComponent } from './role-assigned-users.component';

describe('RoleAssignedUsersComponent', () => {
  let component: RoleAssignedUsersComponent;
  let fixture: ComponentFixture<RoleAssignedUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleAssignedUsersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoleAssignedUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
