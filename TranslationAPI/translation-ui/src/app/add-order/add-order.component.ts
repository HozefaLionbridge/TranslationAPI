import { Component, OnInit } from '@angular/core';
import { Order, OrderStatus } from '../order.model';
import { TranslationService } from '../translation.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-order',
  templateUrl: './add-order.component.html',
  styleUrls: ['./add-order.component.css']
})
export class AddOrderComponent implements OnInit {
  order: Order = {
    orderId: 0,
    orderName: '',
    inputFileURL: '',
    outputFileURL: '',
    statusId: 0,
    submissionDate:  new Date(),
    completedDate:  new Date(new Date().setDate(new Date().getDate() + 2)) 
  };
 
  orderStatuses = [
    { id: OrderStatus.Processing, name: 'In Progress' },
    { id: OrderStatus.Completed, name: 'Completed' }
  ];
 


  constructor(private translationService: TranslationService, private router: Router) {}

  ngOnInit() {}

  onSubmit() {
    this.translationService.addOrder(this.order).subscribe(() => {
      console.log('Order added successfully');
      alert('Order added successfully');
   
      this.router.navigate(['/order-list']);
    });
  }
  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.order.inputFileURL = reader.result as string;
      };
    }
  }
}
