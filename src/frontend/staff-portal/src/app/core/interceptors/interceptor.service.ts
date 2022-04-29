import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor {
  accessToken: string = "";
  constructor(
    ) {   }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const corsReq = req.clone({
      withCredentials: true
    });

    // add authentication bearer token
     const authReq = corsReq.clone({
       headers: corsReq.headers.set("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJqT1YtdTZ1OEZzYmtMYldwOGoyU1ZLNUQ3Uko1MjN0bHJabVVaTnpTMXVFIn0.eyJleHAiOjE2NTEyMjI5NTMsImlhdCI6MTY1MTE4Njk1NCwiYXV0aF90aW1lIjoxNjUxMTg2OTUzLCJqdGkiOiI3YmMwYThmYy00N2ZhLTQwZjYtOThjZi1kNWM2YTQ1NjkzYzIiLCJpc3MiOiJodHRwczovL2Rldi5vaWRjLmdvdi5iYy5jYS9hdXRoL3JlYWxtcy9lemI4a2VqNCIsInN1YiI6ImJhMjE0NDAwLWFmZmYtNDQyMS05OGU3LWY4YzlmZWYwMmRjMiIsInR5cCI6IkJlYXJlciIsImF6cCI6InRjby1zdGFmZi1wb3J0YWwiLCJub25jZSI6ImNjYWNlNjUzZDNlZTM2ZDE0YWE3ZmFmZTNiMThjN2U2NjhjT2xEcnlpIiwic2Vzc2lvbl9zdGF0ZSI6IjhjY2FiMGYzLTA0OWMtNDI0MC1iOWM3LTA4OTQ3Nzc0YzhhZSIsImFjciI6IjEiLCJhbGxvd2VkLW9yaWdpbnMiOlsiKiJdLCJzY29wZSI6Im9wZW5pZCBlbWFpbCBwcm9maWxlIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJuYW1lIjoiTG9ycmFpbmUgRGFtZSIsInByZWZlcnJlZF91c2VybmFtZSI6ImxkYW1lQGlkaXIiLCJnaXZlbl9uYW1lIjoiTG9ycmFpbmUiLCJmYW1pbHlfbmFtZSI6IkRhbWUiLCJlbWFpbCI6ImxvcnJhaW5lLmRhbWVAZ292LmJjLmNhIn0.V8pDxU_d_iqnuXUX9AB19mXOjShv2fvdiVv-wNrY9XN6yuzAMYoRG3fFSaMr17-QH2Y-N-MIEMu4pNl2T5PHuKl40I3gmoTjanxBaDvagQCIMgc_2daCTyQixaJFHUpps8A52_2mAKQ0_tSLF05NRLXYuYjvNodbHP5h6w70-hGgu22oJfWrmDTWWPEtNG52MsU-sIl3z8EnVR1_UU_7xEWSbGl3QF6viKjjb-qlSCpiceZOTJeYC0qT4bpYEaVY9lgTDpK0SrrRFQ5uFfr2elgX0wVi2GZpY_ta9-UzE5c977fMq41ptebEpG3wZSnnzYHB0xPhJsRJrvYwOjEYOg")
    });

    return next.handle(authReq);
  }
}