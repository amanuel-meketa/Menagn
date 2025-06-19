export interface InstanceList {
  instanceId: string;
  templateId: string;
  createdBy: string;
  currentStageOrder: number;
  overallStatus: string;
  createdAt: string;
  completedAt: string | null;
  stageInstances: any[];
}
