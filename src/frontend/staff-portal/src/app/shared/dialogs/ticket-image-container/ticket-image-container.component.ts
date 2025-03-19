import { AfterViewInit, Component, Input, OnDestroy, OnInit } from '@angular/core';

@Component({
    selector: 'app-ticket-image-container',
    templateUrl: './ticket-image-container.component.html',
    styleUrls: ['./ticket-image-container.component.scss']
})
export class TicketImageContainerComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() public imageToShow: any;
    public isTicketImageContainerVisible: boolean = false;
    ngOnInit(): void {
        window.addEventListener('keydown', this.handleKeyboardEvent.bind(this));
    }
    ngAfterViewInit(): void {
        this.makeDraggable(document.getElementById('draggable-overlay'), document.getElementById('title-bar'));
    }
    ngOnDestroy(): void {
        window.removeEventListener('keydown', this.handleKeyboardEvent.bind(this));
    }
    makeDraggable(element: HTMLElement, handle: HTMLElement): void {
        let pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    
        const dragMouseDown = (e: MouseEvent): void => {
          e.preventDefault();
          pos3 = e.clientX;
          pos4 = e.clientY;
          document.onmouseup = closeDragElement;
          document.onmousemove = elementDrag;
        };
    
        const elementDrag = (e: MouseEvent): void => {
          e.preventDefault();
          pos1 = pos3 - e.clientX;
          pos2 = pos4 - e.clientY;
          pos3 = e.clientX;
          pos4 = e.clientY;
    
          let newLeft = element.offsetLeft - pos1;
          let newTop = element.offsetTop - pos2;
    
          // Constrain within viewport
          const viewportWidth = window.innerWidth;
          const viewportHeight = window.innerHeight;
    
          if (newLeft < 0) newLeft = 0;
          if (newTop < 0) newTop = 0;
          if (newLeft + element.offsetWidth > viewportWidth) newLeft = viewportWidth - element.offsetWidth;
          if (newTop + element.offsetHeight > viewportHeight) newTop = viewportHeight - element.offsetHeight;
    
          element.style.left = `${newLeft}px`;
          element.style.top = `${newTop}px`;
        };
    
        const closeDragElement = (): void => {
          document.onmouseup = null;
          document.onmousemove = null;
        };
    
        if (handle) {
          handle.onmousedown = dragMouseDown;
        }
    }
    public openImage(event: MouseEvent): void {
        event.stopPropagation();

        if (this.isTicketImageContainerVisible == false)
            this.isTicketImageContainerVisible = true;
    }
    public closeImage(event: MouseEvent) {
        if (this.isTicketImageContainerVisible == true)
            this.isTicketImageContainerVisible = false;
        setTimeout(() => {
            this.resetSizeAndPosition();
        }, 350);
    }

    resetSizeAndPosition(): void {
        const element = document.getElementById('draggable-overlay');
        element.style.top = '30vh';
        element.style.left = '5vw';
        element.style.width = '20vw';
        element.style.height = '60%';
    }

    handleKeyboardEvent(event: KeyboardEvent) {
        if (event.ctrlKey && event.key === 'i') {
          event.preventDefault();
          this.isTicketImageContainerVisible = !this.isTicketImageContainerVisible;
        }
    }
}