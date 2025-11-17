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

  // Check if file content is HEIC/HEIF/HEVC based on Base64 signature
  public async checkFileContentType(ticketFile: File): Promise<string> {
    return new Promise<string>((resolve) => {
      const reader = new FileReader();

      reader.onload = () => {
        const result = reader.result as string;

        if (typeof result !== "string") {
          resolve("Unable to read file content.");
          return;
        }

        const base64Data = result.split(',')[1] || result;

        // Decode a small portion to inspect for HEIC/HEIF/HEVC signatures
        let decodedChunk = "";
        try {
          decodedChunk = atob(base64Data.slice(0, 100));
        } catch {
          resolve("Unable to decode file content.");
          return;
        }

        // Known HEIC/HEIF/HEVC brand identifiers in the 'ftyp' box
        const heicSignatures = [
          'ftypheic', // HEIC
          'ftypheix', // HEIC variant
          'ftyphevc', // HEVC-based image/video
          'ftyphevx', // HEVC variant
          'ftypmif1', // HEIF
          'ftypmsf1', // HEIF sequence
          'ftypheim', // Apple HEIC variant
          'ftypheis', // HEIF/HEVC variant
          'ftyphevm', // HEVC variant
          'ftyphevs'  // HEVC variant
        ];

        if (heicSignatures.some(sig => decodedChunk.includes(sig))) {
          resolve("Not a valid file type (HEIC/HEIF/HEVC not supported).");
          return;
        }
        // Passed the HEIC/HEIF/HEVC content check
        resolve("");
      };

      reader.onerror = () => {
        resolve("Unable to read file content.");
      };

      reader.readAsDataURL(ticketFile);
    });
  }
}
