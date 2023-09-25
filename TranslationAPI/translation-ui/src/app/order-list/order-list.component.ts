import { Component, OnInit } from '@angular/core';
import { Order, OrderStatus } from '../order.model';
import { TranslationService } from '../translation.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.css']
})
export class OrderListComponent implements OnInit {
  orders: Order[] = [];
  OrderStatus = OrderStatus;
  createdOrders: Order[] = [];
  completedOrders: Order[] = [];
  failedOrders: Order[] = [];
  statusFilter: OrderStatus | null = 0;

  constructor(private orderService: TranslationService, private datePipe: DatePipe) {}

  ngOnInit() {
    this.orderService.getOrders().subscribe((orders: Order[]) => {
      this.orders = orders;
      this.createdOrders = this.orders.filter(order => order.statusId < OrderStatus.Completed);
      this.completedOrders = this.orders.filter(order => order.statusId === OrderStatus.Completed);
      this.failedOrders = this.orders.filter(order => order.statusId === OrderStatus.Failed);
    });
  }
  get filteredOrders(): Order[] {
    debugger;
    if (this.statusFilter === null || this.statusFilter == 0) {
      return this.orders;
    } else {
      return this.orders.filter((order: Order) => order.statusId == this.statusFilter);
    }
  }
  formatDateTime(dateTime: string | Date | null): string {
    if (dateTime === null) {
      return '';
    } else if (typeof dateTime === 'string') {
      dateTime = new Date(dateTime);
    }
    return dateTime ? this.datePipe.transform(dateTime, 'yyyy-MM-dd') || '' : '';
  }

  onDownloadClick(order: Order) {
    this.orderService.downloadOrder(order).subscribe(
      data => {
        console.log('Order downloaded successfully:', data);
        alert('File downloaded successfully');
        // TODO: Display success message to user
      },
      error => {
        console.error('Error downloading order:', error);
        alert('Error while downloading file');
        // TODO: Display error message to user
      }
    );
  }
}