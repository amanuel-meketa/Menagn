export interface GetAppTypeModel {
  id: string;
  name: string;
  description: string;
  status: 'Active' | 'Inactive';
}