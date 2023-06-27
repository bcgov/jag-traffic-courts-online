import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FileUtilsService {
  constructor() {
  }

  public readFileAsDataURL(blob: Blob): Observable<string> {
    return Observable.create(obs => {
      const reader = new FileReader();

      reader.onerror = err => obs.error(err);
      reader.onabort = err => obs.error(err);
      reader.onload = () => obs.next(reader.result);
      reader.onloadend = () => obs.complete();

      return reader.readAsDataURL(blob);
    });
  }

  public checkFileSize(fileSize: number, maxFileSizeInMB: number = 10): string {
    if (fileSize <= 0) return "File size is 0MB.";
    else if (fileSize >= (maxFileSizeInMB * 1024 * 1024)) return "File size is over " + maxFileSizeInMB + "MB."
    else return "";
  }

  public checkFileType(file: File, acceptFileTypes: string[]): string {
    if (!acceptFileTypes.includes(file.type)) return "File type must be one of JPEG, DOC/DOCX, PDF.";
    else return "";
  }
}
