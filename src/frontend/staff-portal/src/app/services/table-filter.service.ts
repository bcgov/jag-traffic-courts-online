import { Injectable } from '@angular/core';
import { TableFilter, UpdateRequestTableStatusDefault} from '@shared/models/table-filter-options.model';

@Injectable({
  providedIn: 'root',
})
export class TableFilterService { // Temp
  tableFilters: TableFilter[] = Array.from({ length: 4 }, () => new TableFilter());
  currentPage: number[] = new Array(4).fill(1);
  constructor() {
    //default status for Update Request inbox set to 'New'
    this.tableFilters[2].status = UpdateRequestTableStatusDefault;
  }
}