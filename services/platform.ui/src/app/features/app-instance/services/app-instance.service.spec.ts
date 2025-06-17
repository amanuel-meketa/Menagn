import { TestBed } from '@angular/core/testing';

import { AppInstanceService } from './app-instance.service';

describe('AppInstanceService', () => {
  let service: AppInstanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppInstanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
