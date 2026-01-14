import { TestBed } from '@angular/core/testing';

import { TemplateHub } from './template-hub';

describe('TemplateHub', () => {
  let service: TemplateHub;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TemplateHub);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
