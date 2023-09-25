import { Component, OnInit } from '@angular/core';
import { Report } from '../order.model';
import { TranslationService } from '../translation.service';


@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent {
  report: Report = {
    requestId: 0,
    fromDate: new Date(),
    toDate: new Date(),
    emailId: '',
    statusId: 0,
    orderStatusId: 0
  };

  constructor(private reportService: TranslationService) {}

  onSubmit() {
    this.reportService.generateReport(this.report)
      .subscribe(
        data => {
          console.log('Report generated successfully:', data);
          // TODO: Display report to user
        },
        error => {
          console.error('Error generating report:', error);
          // TODO: Display error message to user
        }
      );
  }
}