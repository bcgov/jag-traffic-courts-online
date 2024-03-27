import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-paging',
  templateUrl: './paging.component.html',
  styleUrls: ['./paging.component.scss']
})
export class PagingComponent {
  @Input() currentPage: number;
  @Input() totalPages: number;
  @Output() pageChanged: EventEmitter<number> = new EventEmitter();

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.pageChanged.emit(page);
    }
  }

  showPage(page: number): boolean {
    if(this.currentPage === 1 && page <= 3) {
      return true;
    } else if(this.currentPage === this.totalPages && page >= this.totalPages - 2) {
      return true;
    } else if(this.currentPage > 1 && this.currentPage < this.totalPages && page >= this.currentPage - 1 && page <= this.currentPage + 1) {
      return true;
    } else {
      return false;
    }
  }

  counter(i: number) {
    return new Array(i);
  }
}
