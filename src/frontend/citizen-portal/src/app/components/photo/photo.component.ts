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

  //-------------------------------------------------

  public photoCaptureType = new FormControl('upload');

  // toggle webcam on/off
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

  //-------------------------------------------------
  imageSrc: string;
  myForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    file: new FormControl('', [Validators.required]),
    fileSource: new FormControl('', [Validators.required]),
  });

  // fileName = '';
  //----------------------

  @Input()
  requiredFileType: string;

  fileName = '';
  uploadProgress: number;
  uploadSub: Subscription;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    WebcamUtil.getAvailableVideoInputs().then(
      (mediaDevices: MediaDeviceInfo[]) => {
        this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;
      }
    );
  }

  //-------------------

  get f() {
    return this.myForm.controls;
  }

  onFileChangex(event) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      // this.fileName = file.name;

      // const fil2e: File;
      console.log('file', file.name, file.lastModified);

      reader.onload = () => {
        this.imageSrc = reader.result as string;

        this.myForm.patchValue({
          fileSource: reader.result,
        });
      };
    }
  }

  submit() {
    console.log(this.myForm.value);
    this.http
      .post('http://localhost:8001/upload.php', this.myForm.value)
      .subscribe((res) => {
        console.log(res);
        alert('Uploaded Successfully.');
      });
  }
  //--------------------
  onFileChange(event: any) {
    //Angular 11, for stricter type
    if (!event.target.files[0] || event.target.files[0].length == 0) {
      console.log('You must select an image');
      return;
    }

    var mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      console.log('Only images are supported');
      return;
    }

    let reader = new FileReader();
    const file: File = event.target.files[0];
    this.fileName = file.name;
    reader.readAsDataURL(file);
    console.log('file', file.name, file.lastModified);
    // reader.onload = (_event) => {
    // 	// this.msg = "";
    // 	this.url = reader.result;
    // }

    // reader.onload = () => {
    //   this.imageSrc = reader.result as string;

    //   this.myForm.patchValue({
    //     fileSource: reader.result,
    //   });
    // };

    // reader.onload = function(e) {
    //   // The file's text will be printed here
    //   console.log(e.target.result)
    // };

    reader.onload = () => {
      this.imageSrc = reader.result as string;

      this.myForm.patchValue({
        fileSource: reader.result,
      });
    };
  }
  //-----------------------

  // onFileSelected(event) {
  //   const file: File = event.target.files[0];

  //   if (file) {
  //     this.fileName = file.name;
  //     const formData = new FormData();
  //     formData.append('thumbnail', file);

  //     const upload$ = this.http
  //       .post('/api/thumbnail-upload', formData, {
  //         reportProgress: true,
  //         observe: 'events',
  //       })
  //       .pipe(finalize(() => this.reset()));

  //     this.uploadSub = upload$.subscribe((event) => {
  //       if (event.type == HttpEventType.UploadProgress) {
  //         this.uploadProgress = Math.round(100 * (event.loaded / event.total));
  //       }
  //     });
  //   }
  // }

  public onFileSelected(event) {
    const file: File = event.target.files[0];

    if (file) {
      this.fileName = file.name;
      const formData = new FormData();
      formData.append('thumbnail', file);
      const upload$ = this.http.post('/api/thumbnail-upload', formData);
      upload$.subscribe();
    }
  }

  cancelUpload() {
    this.uploadSub.unsubscribe();
    this.reset();
  }

  reset() {
    this.uploadProgress = null;
    this.uploadSub = null;
  }
  //-------------------

  public triggerSnapshot(): void {
    this.trigger.next();
  }

  public toggleWebcam($event: MatButtonToggleChange): void {
    console.log('toggleWebcam', $event);
    this.showWebcam = $event.value === 'take'; //$event.checked; //!this.showWebcam;
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
    console.info('received webcam image', webcamImage);
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
