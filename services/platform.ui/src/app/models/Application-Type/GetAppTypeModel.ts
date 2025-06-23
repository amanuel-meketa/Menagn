export interface GetAppTypeModel {
  templateId: string;
  name: string;
  description: string;
  isActive: boolean;
  totalInstances?: number;
  activeInstances?: number;
}