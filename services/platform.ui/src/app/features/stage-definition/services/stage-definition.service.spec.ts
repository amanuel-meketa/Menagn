import { TestBed } from '@angular/core/testing';

import { StageDefinitionService } from './stage-definition.service';

describe('StageDefinitionService', () => {
  let service: StageDefinitionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StageDefinitionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
