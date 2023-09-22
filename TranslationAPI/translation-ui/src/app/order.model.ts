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