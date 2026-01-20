import { GetAppTypeModel } from '../Application-Type/GetAppTypeModel';
import { CreatedBy } from '../User/CreatedBy';

export interface InstanceList {
  instanceId: string;
  currentStageOrder: number;
  allStages?: number;
  overallStatus: string;
  createdAt: string;
  completedAt: string | null;
  createdBy: CreatedBy;
  template: GetAppTypeModel;
}
