export interface Order {
    orderId: number;
    orderName: string;
    inputFileURL: string;
    outputFileURL: string;
    statusId: number;
    submissionDate: Date;
    completedDate: Date;
  }

  // export enum OrderStatus {
  //   All = 0,
  //   InProgress = 1,
  //   Completed = 2
  // }

  export enum OrderStatus {
    Created,
    Processing,
    Completed,
    Failed
  }

  //create model for Report
  export interface Report {
    requestId: number;
    fromDate: Date;
    toDate: Date;
    emailId: string;
    statusId: number;
    orderStatusId: number;
  }