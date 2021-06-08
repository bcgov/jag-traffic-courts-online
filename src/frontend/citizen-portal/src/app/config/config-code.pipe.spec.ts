import { TestBed, waitForAsync, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ConfigCodePipe } from './config-code.pipe';
import { ConfigService } from './config.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { RouterTestingModule } from '@angular/router/testing';

describe('ConfigCodePipe', () => {
  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [RouterTestingModule, HttpClientTestingModule],
        providers: [
          {
            provide: ConfigService,
            useClass: MockConfigService,
          },
        ],
      });
    })
  );

  it('create an instance of Config Code Pipe', inject(
    [ConfigService],
    (configService: ConfigService) => {
      const pipe = new ConfigCodePipe(configService);
      expect(pipe).toBeTruthy();
    }
  ));

  it('should get country name from a config code', inject(
    [ConfigService],
    (configService: ConfigService) => {
      const pipe = new ConfigCodePipe(configService);
      const prefix = pipe.transform(
        configService.countries[0].code,
        'countries'
      );
      expect(prefix).toBe(configService.countries[0].name);
    }
  ));
});
