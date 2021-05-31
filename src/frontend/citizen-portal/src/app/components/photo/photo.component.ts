import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatButtonToggleChange } from '@angular/material/button-toggle';
import { WebcamImage, WebcamInitError, WebcamUtil } from 'ngx-webcam';
import { Observable, Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-photo',
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.scss'],
})
export class PhotoComponent implements OnInit {
  public busy: Subscription;

  public photoCaptureType = new FormControl('upload');

  public showWebcam = false;
  // public allowCameraSwitch = true;
  public multipleWebcamsAvailable = false;
  public deviceId: string;
  public videoOptions: MediaTrackConstraints = {
    // width: { min: 1280, ideal: 1920 },
    // height: { min: 720, ideal: 1080 },
  };
  public errors: WebcamInitError[] = [];

  // latest snapshot
  public webcamImage: WebcamImage = null;

  // webcam snapshot trigger
  private trigger: Subject<void> = new Subject<void>();

  // switch to next / previous / specific webcam; true/false: forward/backwards, string: deviceId
  private nextWebcam: Subject<boolean | string> = new Subject<
    boolean | string
  >();

  public imageSrc: string;
  public myForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    file: new FormControl('', [Validators.required]),
    fileSource: new FormControl('', [Validators.required]),
  });

  @Input()
  requiredFileType: string;

  public fileName = '';
  public uploadProgress: number;
  public uploadSub: Subscription;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    WebcamUtil.getAvailableVideoInputs().then(
      (mediaDevices: MediaDeviceInfo[]) => {
        this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;
      }
    );
  }

  public onFileChange(event: any) {
    if (!event.target.files[0] || event.target.files[0].length === 0) {
      console.log('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      console.log('Only images are supported');
      return;
    }

    const reader = new FileReader();
    const file: File = event.target.files[0];
    this.fileName = file.name;
    reader.readAsDataURL(file);
    console.log('file', file.name, file.lastModified);

    reader.onload = () => {
      this.imageSrc = reader.result as string;

      this.myForm.patchValue({
        fileSource: reader.result,
      });
    };
  }

  public triggerSnapshot(): void {
    this.trigger.next();
  }

  public toggleWebcam($event: MatButtonToggleChange): void {
    console.log('toggleWebcam', $event);
    this.showWebcam = $event.value === 'take'; // $event.checked; //!this.showWebcam;
  }

  public handleInitError(error: WebcamInitError): void {
    if (
      error.mediaStreamError &&
      error.mediaStreamError.name === 'NotAllowedError'
    ) {
      console.warn('Camera access was not allowed by user!');
    }
    this.errors.push(error);
  }

  public showNextWebcam(directionOrDeviceId: boolean | string): void {
    // true => move forward through devices
    // false => move backwards through devices
    // string => move to device with given deviceId
    this.nextWebcam.next(directionOrDeviceId);
  }

  public handleImage(webcamImage: WebcamImage): void {
    console.log('received webcam image', webcamImage);
    this.webcamImage = webcamImage;
  }

  public cameraWasSwitched(deviceId: string): void {
    console.log('active device: ' + deviceId);
    this.deviceId = deviceId;
  }

  public get triggerObservable(): Observable<void> {
    return this.trigger.asObservable();
  }

  public get nextWebcamObservable(): Observable<boolean | string> {
    return this.nextWebcam.asObservable();
  }
}
