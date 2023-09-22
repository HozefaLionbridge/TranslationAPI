import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, retry, throwError } from 'rxjs';
import { Order } from './order.model';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
    private apiUrl = 'https://translationapi20230921105629.azurewebsites.net/api';
    //private apiUrl = ' https://localhost:7031/api';


  constructor(private http: HttpClient) { }

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/Translation/GetOrders`).pipe(
        retry(3), // retry up to 3 times
        catchError(this.handleError) // handle errors
      );
  }
  addOrder(order: Order): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/Translation/AddOrder`, order).pipe(
      catchError(this.handleError)
    );
  }
  //handle errors
  private handleError(error: HttpErrorResponse) {
    debugger;
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  };
}