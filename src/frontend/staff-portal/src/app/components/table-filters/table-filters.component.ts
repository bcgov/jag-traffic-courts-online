import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TableFilter, TableFilterConfigs, TableFilterKeys, TableFilterStatus, TableFilterStatusDefault } from '@shared/models/table-filter-options.model';
import { DisputeStatus } from 'app/api';
import { LookupsService } from 'app/services/lookups.service';
import { TableFilterService } from 'app/services/table-filter.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-table-filters',
  templateUrl: './table-filters.component.html',
  styleUrls: ['./table-filters.component.scss'],
})
export class TableFiltersComponent implements OnInit {
  @Input() tabIndex: number;
  @Input() tableFilterKeys: TableFilterKeys[] = [];
  @Input() statusFilterOptions: DisputeStatus[] = [];
  @Input() defaultStatusFilter: TableFilterStatus = TableFilterStatusDefault;
  @Input() courthouseTeamNames: string[] = [];
  @Output() onFilterChanged: EventEmitter<TableFilter> = new EventEmitter();

  dataFilters: TableFilter;
  tableFilterConfigs: TableFilterConfigs = {};
  courtHouseSettings: IDropdownSettings = {};

  constructor(
    private tableFilterService: TableFilterService,
    public lookupsService: LookupsService
  ) {
  }

  ngOnInit(): void {
    this.courtHouseSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 1,
      allowSearchFilter: true
    };
    this.tableFilterKeys?.forEach(key => {
      this.tableFilterConfigs[key] = true;
    })
    this.dataFilters = this.tableFilterService.tableFilters[this.tabIndex];
    this.dataFilters.status = this.dataFilters.status ?? this.defaultStatusFilter;
  }

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = new TableFilter();
    this.dataFilters.status = this.defaultStatusFilter;
    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      this.tableFilterService.tableFilters[this.tabIndex] = this.dataFilters;
      this.onFilterChanged.emit(this.dataFilters);
    }, 100);
  }

  // called on keyup in filter field
  onApplyFilter(filterName: string, value: any) {
    if (value === undefined && filterName.toUpperCase().includes('DATE')) {
      value = "";
    }
    if(filterName !== 'courthouseLocation') {
      const filterValue = value;
      this.dataFilters[filterName] = filterValue;
      this.tableFilterService.tableFilters[this.tabIndex] = this.dataFilters;      
    }
    this.onFilterChanged.emit(this.dataFilters); 
  }

  onSelectOrDeselectAllCourthouseLocations(selectedItems: any) {
    this.dataFilters.courthouseLocation = selectedItems;
    this.tableFilterService.tableFilters[this.tabIndex] = this.dataFilters;
    this.onFilterChanged.emit(this.dataFilters);
  }
}