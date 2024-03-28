import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TableFilter, TableFilterConfigs, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { DisputeStatus } from 'app/api';
import { TableFilterService } from 'app/services/table-filter.service';

@Component({
  selector: 'app-table-filters',
  templateUrl: './table-filters.component.html',
  styleUrls: ['./table-filters.component.scss'],
})
export class TableFiltersComponent implements OnInit {
  @Input() tabIndex: number;
  @Input() tableFilterKeys: TableFilterKeys[] = [];
  @Input() statusFilterOptions: DisputeStatus[] = [];
  @Input() statusFilterDefaultText: string = "-- select --";
  @Input() courthouseTeamNames: string[] = [];
  @Output() onFilterChanged: EventEmitter<TableFilter> = new EventEmitter();

  dataFilters: TableFilter;
  tableFilterConfigs: TableFilterConfigs = {};

  constructor(
    private tableFilterService: TableFilterService
  ) {
  }

  ngOnInit(): void {
    this.tableFilterKeys?.forEach(key => {
      this.tableFilterConfigs[key] = true;
    })
    this.dataFilters = this.tableFilterService.tableFilters[this.tabIndex];
    this.dataFilters.status = this.dataFilters.status ?? "";
  }

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = new TableFilter();
    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      this.tableFilterService.tableFilters[this.tabIndex] = this.dataFilters;
      this.dataFilters.status = this.dataFilters.status ?? "";
      this.onFilterChanged.emit(this.dataFilters);
    }, 100);
  }

  // called on keyup in filter field
  onApplyFilter(filterName: string, value: string) {
    const filterValue = value;
    this.dataFilters[filterName] = filterValue;
    this.tableFilterService.tableFilters[this.tabIndex] = this.dataFilters;
    this.onFilterChanged.emit(this.dataFilters);
  }
}